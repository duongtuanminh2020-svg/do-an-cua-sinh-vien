using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webquanli.Data;
using webquanli.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace webquanli.Controllers
{
    public class DeTaiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DeTaiController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var query = _context.DeTais
                .Include(d => d.GiangVien)
                    .ThenInclude(g => g.BoMon)
                .AsQueryable();

            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);

            if (userAcc != null)
            {
                string role = userAcc.Role?.Trim().ToLower() ?? "";

                if (role == "sinhvien" && userAcc.SinhVienId.HasValue)
                {
                    int svId = userAcc.SinhVienId.Value;
                    var sv = _context.SinhViens.Find(svId);
                    if (sv != null && !string.IsNullOrEmpty(sv.Nganh))
                    {
                        query = query.Where(d => d.GiangVien != null && d.GiangVien.BoMon != null && d.GiangVien.BoMon.TenBoMon.Contains(sv.Nganh));
                    }
                    ViewBag.DeTaiCuaToi = _context.DangKyDeTais.FirstOrDefault(d => d.SinhVienId == svId)?.DeTaiId ?? 0;
                }
                else if (role == "giangvien" && userAcc.GiangVienId.HasValue)
                {
                    int gvId = userAcc.GiangVienId.Value;
                    var gv = _context.GiangViens.Find(gvId);
                    if (gv != null)
                    {
                        query = query.Where(d => d.GiangVien != null && d.GiangVien.BoMonId == gv.BoMonId);
                    }
                }
            }

            var danhSachDeTai = query.ToList();
            ViewBag.DanhSachDaDangKy = _context.DangKyDeTais.Select(d => d.DeTaiId).ToList();

            return View(danhSachDeTai);
        }

        [HttpPost]
        public IActionResult DangKy(int id)
        {
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);

            string role = userAcc?.Role?.Trim().ToLower() ?? "";

            if (userAcc == null || role != "sinhvien" || !userAcc.SinhVienId.HasValue)
                return Unauthorized();

            int svId = userAcc.SinhVienId.Value;

            bool daCoDeTai = _context.DangKyDeTais.Any(d => d.SinhVienId == svId);
            if (daCoDeTai)
            {
                TempData["Error"] = "Bạn đã đăng ký một đề tài rồi!";
                return RedirectToAction("Index");
            }

            bool deTaiDaBiChon = _context.DangKyDeTais.Any(d => d.DeTaiId == id);
            if (deTaiDaBiChon)
            {
                TempData["Error"] = "Đề tài này vừa bị người khác đăng ký mất!";
                return RedirectToAction("Index");
            }

            var deTaiMuonDangKy = _context.DeTais.Include(d => d.GiangVien).ThenInclude(g => g.BoMon).FirstOrDefault(d => d.Id == id);
            var sinhVien = _context.SinhViens.Find(svId);

            if (deTaiMuonDangKy?.GiangVien?.BoMon?.TenBoMon != sinhVien.Nganh)
            {
                TempData["Error"] = "Đề tài không thuộc chuyên ngành của bạn!";
                return RedirectToAction("Index");
            }

            var dangKyMoi = new DangKyDeTai
            {
                SinhVienId = svId,
                DeTaiId = id,
                NgayDangKy = DateTime.Now
            };

            _context.DangKyDeTais.Add(dangKyMoi);
            _context.SaveChanges(); // Lưu đăng ký vào cơ sở dữ liệu

            // =============== THÊM MỚI: BẮN THÔNG BÁO (VÀ EMAIL) TỰ ĐỘNG ===============
            try
            {
                // 1. TẠO THÔNG BÁO CHO GIẢNG VIÊN VÀ ADMIN
                var taiKhoanGV = _context.Users.FirstOrDefault(u => u.GiangVienId == deTaiMuonDangKy.GiangVienId);
                if (taiKhoanGV != null)
                {
                    _context.ThongBaos.Add(new ThongBao
                    {
                        UsernameNhan = taiKhoanGV.Username,
                        TieuDe = "Sinh viên đăng ký đề tài",
                        NoiDung = $"Sinh viên {sinhVien.TenSV} (Mã SV: {sinhVien.MaSV}) vừa chốt đề tài '{deTaiMuonDangKy.TenDeTai}' của bạn.",
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });
                }

                // Báo cho Admin biết
                var adminUser = _context.Users.FirstOrDefault(u => u.Role != null && u.Role.Trim().ToLower() == "admin");
                if (adminUser != null)
                {
                    _context.ThongBaos.Add(new ThongBao
                    {
                        UsernameNhan = adminUser.Username,
                        TieuDe = "Hệ thống: Có đăng ký đề tài mới",
                        NoiDung = $"Sinh viên {sinhVien.TenSV} vừa đăng ký đề tài '{deTaiMuonDangKy.TenDeTai}'.",
                        NgayTao = DateTime.Now,
                        DaDoc = false
                    });
                }
                _context.SaveChanges(); // Chốt lưu thông báo xuống SQL

                // 2. GỬI EMAIL CHO GIẢNG VIÊN (Nếu bạn đã tạo file EmailSender ở Bước 1)
                // (Nếu chưa làm file EmailSender thì cứ để nguyên code này không sao cả, try-catch sẽ tự bỏ qua lỗi)
                string emailGiangVien = deTaiMuonDangKy?.GiangVien?.Email;
                if (!string.IsNullOrEmpty(emailGiangVien))
                {
                    string tieuDe = $"[Thông báo] Sinh viên đăng ký đề tài: {deTaiMuonDangKy.TenDeTai}";
                    string noiDung = $@"
                        <h3>Xin chào {deTaiMuonDangKy.GiangVien.TenGV},</h3>
                        <p>Hệ thống xin thông báo: Sinh viên <b>{sinhVien.TenSV}</b> (Mã SV: {sinhVien.MaSV}) vừa đăng ký thực hiện đề tài <b>{deTaiMuonDangKy.TenDeTai}</b> của thầy/cô.</p>
                        <p>Vui lòng đăng nhập vào hệ thống để kiểm tra và theo dõi tiến độ.</p>
                        <br>
                        <p>Trân trọng,<br><b>Hệ thống Quản lý Đồ án</b></p>";

                    // Nhớ đổi chữ 'webquanli' thành tên project của bạn nếu bị gạch đỏ
                    _ = webquanli.Models.EmailSender.SendEmailAsync(emailGiangVien, tieuDe, noiDung);
                }
            }
            catch (Exception)
            {
                // Bỏ qua lỗi ngầm nếu gửi email/thông báo xịt, không làm đứt mạch của sinh viên
            }
            // ===========================================================================

            TempData["Success"] = "Chốt đề tài thành công!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult HuyDangKy(int id)
        {
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);
            string role = userAcc?.Role?.Trim().ToLower() ?? "";

            if (userAcc == null || role != "sinhvien" || !userAcc.SinhVienId.HasValue)
                return Unauthorized();

            int svId = userAcc.SinhVienId.Value;

            var dangKy = _context.DangKyDeTais.FirstOrDefault(d => d.SinhVienId == svId && d.DeTaiId == id);

            if (dangKy != null)
            {
                _context.DangKyDeTais.Remove(dangKy);
                _context.SaveChanges();
                TempData["Success"] = "Đã hủy đăng ký đề tài thành công! Bây giờ bạn có thể chọn đề tài khác.";
            }
            else
            {
                TempData["Error"] = "Lỗi: Không tìm thấy thông tin đăng ký của bạn!";
            }

            return RedirectToAction("Index");
        }

        // GET: DeTai/Create
        public IActionResult Create()
        {
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);
            string role = userAcc?.Role?.Trim().ToLower() ?? "";

            var query = _context.GiangViens.AsQueryable();

            if (role == "giangvien" && userAcc.GiangVienId.HasValue)
            {
                var currentGV = _context.GiangViens.Find(userAcc.GiangVienId.Value);
                if (currentGV != null)
                {
                    query = query.Where(g => g.BoMonId == currentGV.BoMonId);
                }
            }

            ViewBag.GiangVienId = new SelectList(query.ToList(), "Id", "TenGV");
            return View();
        }

        [HttpPost]
        public IActionResult Create(DeTai deTai)
        {
            if (string.IsNullOrWhiteSpace(deTai.MoTa))
            {
                deTai.MoTa = "Chưa có mô tả cho đề tài này.";
            }

            _context.DeTais.Add(deTai);
            _context.SaveChanges();

            TempData["Success"] = "Đã thêm đề tài mới thành công!";
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var deTai = _context.DeTais.Find(id);
            if (deTai == null) return NotFound();
            ViewBag.GiangVienId = new SelectList(_context.GiangViens, "Id", "TenGV", deTai.GiangVienId);
            return View(deTai);
        }

        [HttpPost]
        public IActionResult Edit(DeTai deTai)
        {
            _context.DeTais.Update(deTai);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var deTai = _context.DeTais.Find(id);
            if (deTai != null)
            {
                _context.DeTais.Remove(deTai);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}