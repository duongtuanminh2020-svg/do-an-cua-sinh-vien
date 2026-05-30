using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webquanli.Data;
using webquanli.Models;
using System;
using System.Linq;

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
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);
            if (userAcc == null) return RedirectToAction("Login", "Account");

            string role = userAcc.Role?.Trim().ToLower() ?? "";
            ViewBag.Role = role;

            if (role == "sinhvien" && userAcc.SinhVienId.HasValue)
            {
                var sv = _context.SinhViens.Find(userAcc.SinhVienId.Value);
                ViewBag.TenDeTaiHienTai = sv?.TenDeTai ?? "";
                ViewBag.MoTaHienTai = sv?.MoTaDeTai ?? "";
                ViewBag.TrangThai = sv?.TrangThaiDeTai ?? "";
                ViewBag.LyDoTuChoi = sv?.LyDoTuChoi ?? "";
            }

            if (role == "giangvien" && userAcc.GiangVienId.HasValue)
            {
                var gv = _context.GiangViens.Include(g => g.BoMon).FirstOrDefault(g => g.Id == userAcc.GiangVienId.Value);
                var danhSachSV = _context.SinhViens
                    .Include(s => s.BoMon)
                    .Where(s => s.BoMonId == gv.BoMonId && s.TenDeTai != null && s.TenDeTai != "")
                    .ToList();
                ViewBag.DanhSachSV = danhSachSV;
            }

            if (role == "admin")
            {
                var danhSachSV = _context.SinhViens
                    .Include(s => s.BoMon)
                    .Where(s => s.TenDeTai != null && s.TenDeTai != "")
                    .ToList();
                ViewBag.DanhSachSV = danhSachSV;
            }

            return View();
        }

        [HttpPost]
        public IActionResult LuuDeTai(string tenDeTai, string moTa)
        {
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);
            if (userAcc == null || userAcc.Role?.Trim().ToLower() != "sinhvien" || !userAcc.SinhVienId.HasValue)
                return Unauthorized();

            var sv = _context.SinhViens.Find(userAcc.SinhVienId.Value);
            if (sv == null) return NotFound();

            if (string.IsNullOrWhiteSpace(tenDeTai))
            {
                TempData["Error"] = "Tên đề tài không được để trống!";
                return RedirectToAction("Index");
            }

            sv.TenDeTai = tenDeTai.Trim();
            sv.MoTaDeTai = moTa?.Trim();
            sv.TrangThaiDeTai = "choduyet";
            sv.LyDoTuChoi = null;

            _context.SaveChanges();
            TempData["Success"] = "Đã gửi đề tài! Vui lòng chờ giảng viên phê duyệt.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult XoaDeTai()
        {
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);
            if (userAcc == null || userAcc.Role?.Trim().ToLower() != "sinhvien" || !userAcc.SinhVienId.HasValue)
                return Unauthorized();

            var sv = _context.SinhViens.Find(userAcc.SinhVienId.Value);
            if (sv == null) return NotFound();

            var dangKy = _context.DangKyDeTais.FirstOrDefault(d => d.SinhVienId == sv.Id);
            if (dangKy != null)
            {
                bool daNopBaoCao = _context.BaoCaos.Any(b => b.DangKyDeTaiId == dangKy.Id);
                if (daNopBaoCao)
                {
                    TempData["Error"] = "Bạn đã nộp báo cáo rồi, không thể xóa đề tài!";
                    return RedirectToAction("Index");
                }
            }

            sv.TenDeTai = null;
            sv.MoTaDeTai = null;
            sv.TrangThaiDeTai = null;
            sv.LyDoTuChoi = null;
            _context.SaveChanges();

            TempData["Success"] = "Đã xóa đề tài. Bạn có thể đăng ký đề tài mới!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PheDuyet(int svId)
        {
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);
            if (userAcc == null || userAcc.Role?.Trim().ToLower() != "giangvien") return Unauthorized();

            var sv = _context.SinhViens.Find(svId);
            if (sv == null) return NotFound();

            sv.TrangThaiDeTai = "daduyet";
            sv.LyDoTuChoi = null;

            // Tạo DangKyDeTai nếu chưa có
            var dangKyCu = _context.DangKyDeTais.FirstOrDefault(d => d.SinhVienId == sv.Id);
            if (dangKyCu == null)
            {
                var deTaiMoi = new DeTai
                {
                    TenDeTai = sv.TenDeTai,
                    MoTa = sv.MoTaDeTai ?? "",
                    GiangVienId = userAcc.GiangVienId.Value
                };
                _context.DeTais.Add(deTaiMoi);
                _context.SaveChanges();

                _context.DangKyDeTais.Add(new DangKyDeTai
                {
                    SinhVienId = sv.Id,
                    DeTaiId = deTaiMoi.Id,
                    NgayDangKy = DateTime.Now,
                    IsApproved = true
                });
            }

            _context.SaveChanges();
            TempData["Success"] = $"Đã duyệt đề tài cho sinh viên {sv.TenSV}!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult TuChoi(int svId, string lyDo)
        {
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);
            if (userAcc == null || userAcc.Role?.Trim().ToLower() != "giangvien") return Unauthorized();

            var sv = _context.SinhViens.Find(svId);
            if (sv == null) return NotFound();

            sv.TrangThaiDeTai = "tuchoi";
            sv.LyDoTuChoi = lyDo?.Trim();
            _context.SaveChanges();

            TempData["Success"] = $"Đã từ chối đề tài của sinh viên {sv.TenSV}!";
            return RedirectToAction("Index");
        }

        public IActionResult XuatExcel()
        {
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);
            if (userAcc == null) return Unauthorized();

            string role = userAcc.Role?.Trim().ToLower() ?? "";
            if (role != "admin" && role != "giangvien") return Unauthorized();

            var danhSachSV = _context.SinhViens
                .Include(s => s.BoMon)
                .Where(s => s.TenDeTai != null && s.TenDeTai != "" && s.TrangThaiDeTai == "daduyet")
                .OrderBy(s => s.BoMon.TenBoMon)
                .ThenBy(s => s.TenSV)
                .ToList();

            if (role == "giangvien" && userAcc.GiangVienId.HasValue)
            {
                var gv = _context.GiangViens.Find(userAcc.GiangVienId.Value);
                if (gv != null)
                    danhSachSV = danhSachSV.Where(s => s.BoMonId == gv.BoMonId).ToList();
            }

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("MSSV,Họ và tên,Lớp,Bộ môn,Tên đề tài");
            foreach (var sv in danhSachSV)
            {
                csv.AppendLine($"{sv.MaSV},{sv.TenSV},{sv.Lop},{sv.BoMon?.TenBoMon},{sv.TenDeTai}");
            }

            var bytes = System.Text.Encoding.UTF8.GetPreamble()
                .Concat(System.Text.Encoding.UTF8.GetBytes(csv.ToString()))
                .ToArray();

            return File(bytes, "text/csv", $"DanhSachDeTai_{DateTime.Now:yyyyMMdd}.csv");
        }
    }
}
