using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using webquanli.Data;
using webquanli.Models;
using System.Linq;
using System;
using System.IO;

namespace webquanli.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BaoCaoController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var query = _context.BaoCaos
                .Include(b => b.DangKyDeTai).ThenInclude(d => d.SinhVien).ThenInclude(s => s.BoMon)
                .Include(b => b.DangKyDeTai).ThenInclude(d => d.DeTai)
                .Include(b => b.DotDoAn)
                .AsQueryable();

            bool isSinhVien = User.IsInRole("SinhVien") || User.IsInRole("sinhvien");
            bool isGiangVien = User.IsInRole("GiangVien") || User.IsInRole("giangvien");
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("admin");

            if (isSinhVien)
            {
                var claimSinhVienId = User.Claims.FirstOrDefault(c => c.Type == "SinhVienId")?.Value;
                if (int.TryParse(claimSinhVienId, out int sinhVienId))
                {
                    ViewBag.DanhSachDot = _context.DotDoAns.OrderByDescending(d => d.NgayBatDau).ToList();

                    ViewBag.DictBaoCao = _context.BaoCaos
                        .Include(b => b.DangKyDeTai)
                        .Where(b => b.DangKyDeTai.SinhVienId == sinhVienId && b.DotDoAnId != null)
                        .ToDictionary(b => b.DotDoAnId.Value, b => b);

                    query = query.Where(b => b.DangKyDeTai.SinhVienId == sinhVienId);
                }
            }
            else if (isGiangVien)
            {
                var claimGiangVienId = User.Claims.FirstOrDefault(c => c.Type == "GiangVienId")?.Value;
                if (int.TryParse(claimGiangVienId, out int gvId))
                {
                    query = query.Where(b => b.DangKyDeTai.DeTai.GiangVienId == gvId);
                }
                ViewBag.ListDot = _context.DotDoAns.OrderByDescending(d => d.NgayBatDau).ToList();
            }
            else if (isAdmin)
            {
                ViewBag.ListDot = _context.DotDoAns.OrderByDescending(d => d.NgayBatDau).ToList();
                ViewBag.ListBoMon = _context.BoMons.ToList();
            }

            var danhSachBaoCao = query.OrderByDescending(b => b.NgayNop).ToList();
            return View(danhSachBaoCao);
        }

        public IActionResult Upload(int dotId)
        {
            var dot = _context.DotDoAns.Find(dotId);
            if (dot == null)
            {
                TempData["Error"] = "Không tìm thấy đợt báo cáo!";
                return RedirectToAction("Index");
            }

            var now = DateTime.Now;
            if (now < dot.NgayBatDau || now > dot.NgayKetThuc)
            {
                TempData["Error"] = "Hiện tại không nằm trong thời gian cho phép nộp bài của đợt này!";
                return RedirectToAction("Index");
            }

            var queryDangKy = _context.DangKyDeTais
                .Include(d => d.SinhVien).Include(d => d.DeTai)
                .AsQueryable();

            bool isSinhVien = User.IsInRole("SinhVien") || User.IsInRole("sinhvien");

            if (isSinhVien)
            {
                var claimSinhVienId = User.Claims.FirstOrDefault(c => c.Type == "SinhVienId")?.Value;
                if (int.TryParse(claimSinhVienId, out int sinhVienId))
                {
                    bool daNop = _context.BaoCaos.Any(b => b.DangKyDeTai.SinhVienId == sinhVienId && b.DotDoAnId == dotId);
                    if (daNop)
                    {
                        TempData["Error"] = "Bạn đã nộp báo cáo cho đợt này rồi. Vui lòng xóa file cũ nếu muốn nộp lại.";
                        return RedirectToAction("Index");
                    }
                    queryDangKy = queryDangKy.Where(d => d.SinhVienId == sinhVienId);
                }
            }

            var danhSachDangKy = queryDangKy
                .Select(d => new { Id = d.Id, TenHienThi = d.SinhVien.TenSV + " - " + d.DeTai.TenDeTai })
                .ToList();

            ViewBag.ListDangKy = new SelectList(danhSachDangKy, "Id", "TenHienThi");
            ViewBag.DotId = dotId;
            ViewBag.TenDot = dot.TenDot;

            return View();
        }

        [HttpPost]
        public IActionResult Upload(BaoCao baoCao, IFormFile fileUpload, int dotId)
        {
            baoCao.NgayNop = DateTime.Now;
            baoCao.DangKyDeTaiId = baoCao.DangKyId;
            baoCao.DotDoAnId = dotId;

            if (fileUpload != null && fileUpload.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = DateTime.Now.ToString("yyyyMMddHHmmss_") + fileUpload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    fileUpload.CopyTo(fileStream);
                }

                baoCao.TenFile = fileUpload.FileName;
                baoCao.DuongDan = "/uploads/" + uniqueFileName;
            }

            _context.BaoCaos.Add(baoCao);
            _context.SaveChanges();

            try
            {
                var dot = _context.DotDoAns.Find(dotId);
                var dangKy = _context.DangKyDeTais.Include(d => d.SinhVien).Include(d => d.DeTai).FirstOrDefault(d => d.Id == baoCao.DangKyId);
                if (dangKy != null && dangKy.DeTai != null && dangKy.SinhVien != null)
                {
                    var gvUser = _context.Users.FirstOrDefault(u => u.GiangVienId == dangKy.DeTai.GiangVienId);
                    if (gvUser != null)
                    {
                        _context.ThongBaos.Add(new ThongBao
                        {
                            UsernameNhan = gvUser.Username,
                            TieuDe = "Sinh viên nộp báo cáo",
                            NoiDung = $"Sinh viên {dangKy.SinhVien.TenSV} vừa tải lên file cho '{dot?.TenDot}'.",
                            NgayTao = DateTime.Now,
                            DaDoc = false
                        });
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception) { }

            TempData["Success"] = "Đã nộp báo cáo thành công!";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var baoCao = _context.BaoCaos.Find(id);
            if (baoCao != null)
            {
                _context.BaoCaos.Remove(baoCao);
                _context.SaveChanges();
                TempData["Success"] = "Đã xóa file báo cáo thành công. Bạn có thể nộp lại file mới.";
            }
            return RedirectToAction("Index");
        }

        // ĐÃ SỬA: Nâng cấp hàm Download để bắt lỗi 404 mượt mà
        public IActionResult Download(int id)
        {
            var baoCao = _context.BaoCaos.Find(id);
            if (baoCao == null || string.IsNullOrEmpty(baoCao.DuongDan))
            {
                TempData["Error"] = "Không tìm thấy thông tin file trong cơ sở dữ liệu!";
                return RedirectToAction("Index");
            }

            // Dùng Path.Combine để ghép đường dẫn an toàn
            // Cắt dấu '/' ở đầu để tránh hệ thống hiểu lầm là thư mục gốc của máy tính
            string relativePath = baoCao.DuongDan.TrimStart('/');
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

            // Nếu file vật lý đã bị xóa khỏi máy, báo lỗi lịch sự thay vì hiện trang 404
            if (!System.IO.File.Exists(filePath))
            {
                TempData["Error"] = "File vật lý không còn tồn tại trên hệ thống (Có thể đã bị xóa hoặc mất file gốc).";
                return RedirectToAction("Index");
            }

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", baoCao.TenFile);
        }

        public IActionResult ChamDiem(int id)
        {
            bool isGiangVien = User.IsInRole("GiangVien") || User.IsInRole("giangvien");
            if (!isGiangVien) return Unauthorized();

            var baoCao = _context.BaoCaos
                .Include(b => b.DangKyDeTai).ThenInclude(d => d.SinhVien)
                .Include(b => b.DangKyDeTai).ThenInclude(d => d.DeTai)
                .FirstOrDefault(b => b.Id == id);

            if (baoCao == null) return NotFound();
            return View(baoCao);
        }

        [HttpPost]
        public IActionResult ChamDiem(int id, double Diem, string NhanXet)
        {
            bool isGiangVien = User.IsInRole("GiangVien") || User.IsInRole("giangvien");
            if (!isGiangVien) return Unauthorized();

            var baoCao = _context.BaoCaos.Find(id);
            if (baoCao != null)
            {
                baoCao.Diem = Diem;
                baoCao.NhanXet = NhanXet;
                _context.SaveChanges();
                TempData["Success"] = "Đã chấm điểm báo cáo thành công!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult TaoDotBaoCao(string TenDot, DateTime NgayBatDau, DateTime NgayKetThuc)
        {
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("admin");
            if (!isAdmin) return Unauthorized();

            var dotMoi = new DotDoAn
            {
                TenDot = TenDot,
                NgayBatDau = NgayBatDau,
                NgayKetThuc = NgayKetThuc,
                IsActive = true
            };

            _context.DotDoAns.Add(dotMoi);
            _context.SaveChanges();

            TempData["Success"] = $"Đã tạo đợt báo cáo: {TenDot}";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EditDotBaoCao(int Id, string TenDot, DateTime NgayBatDau, DateTime NgayKetThuc)
        {
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("admin");
            if (!isAdmin) return Unauthorized();

            var dot = _context.DotDoAns.Find(Id);
            if (dot != null)
            {
                dot.TenDot = TenDot;
                dot.NgayBatDau = NgayBatDau;
                dot.NgayKetThuc = NgayKetThuc;
                _context.SaveChanges();
                TempData["Success"] = $"Đã cập nhật thông tin đợt: {TenDot}";
            }
            return RedirectToAction("Index");
        }
    }
}