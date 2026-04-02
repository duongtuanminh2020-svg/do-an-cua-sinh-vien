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
using System.Security.Claims;

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
                .Include(b => b.DangKyDeTai).ThenInclude(d => d.SinhVien)
                .Include(b => b.DangKyDeTai).ThenInclude(d => d.DeTai)
                .Include(b => b.DotDoAn)
                .AsQueryable();

            if (User.IsInRole("SinhVien"))
            {
                var claimSinhVienId = User.Claims.FirstOrDefault(c => c.Type == "SinhVienId")?.Value;
                if (int.TryParse(claimSinhVienId, out int sinhVienId))
                {
                    query = query.Where(b => b.DangKyDeTai.SinhVienId == sinhVienId);
                }
            }
            else if (User.IsInRole("GiangVien"))
            {
                var claimGiangVienId = User.Claims.FirstOrDefault(c => c.Type == "GiangVienId")?.Value;
                if (int.TryParse(claimGiangVienId, out int gvId))
                {
                    var giangVien = _context.GiangViens.Find(gvId);
                    if (giangVien != null && !string.IsNullOrEmpty(giangVien.LopQuanLy))
                    {
                        query = query.Where(b => b.DangKyDeTai.SinhVien.Lop == giangVien.LopQuanLy);
                    }
                }
            }

            var danhSachBaoCao = query.ToList();
            ViewBag.DotHienTai = _context.DotDoAns.FirstOrDefault(d => d.IsActive == true);
            return View(danhSachBaoCao);
        }

        public IActionResult Upload()
        {
            var queryDangKy = _context.DangKyDeTais
                .Include(d => d.SinhVien).Include(d => d.DeTai)
                .AsQueryable();

            if (User.IsInRole("SinhVien"))
            {
                var claimSinhVienId = User.Claims.FirstOrDefault(c => c.Type == "SinhVienId")?.Value;
                if (int.TryParse(claimSinhVienId, out int sinhVienId))
                {
                    queryDangKy = queryDangKy.Where(d => d.SinhVienId == sinhVienId);
                }
            }
            else if (User.IsInRole("GiangVien"))
            {
                var claimGiangVienId = User.Claims.FirstOrDefault(c => c.Type == "GiangVienId")?.Value;
                if (int.TryParse(claimGiangVienId, out int gvId))
                {
                    var giangVien = _context.GiangViens.Find(gvId);
                    if (giangVien != null && !string.IsNullOrEmpty(giangVien.LopQuanLy))
                    {
                        queryDangKy = queryDangKy.Where(d => d.SinhVien.Lop == giangVien.LopQuanLy);
                    }
                }
            }

            var danhSachDangKy = queryDangKy
                .Select(d => new { Id = d.Id, TenHienThi = d.SinhVien.TenSV + " - " + d.DeTai.TenDeTai })
                .ToList();

            ViewBag.ListDangKy = new SelectList(danhSachDangKy, "Id", "TenHienThi");
            return View();
        }

        [HttpPost]
        public IActionResult Upload(BaoCao baoCao, IFormFile fileUpload)
        {
            baoCao.NgayNop = DateTime.Now;
            baoCao.DangKyDeTaiId = baoCao.DangKyId;

            var dotHienTai = _context.DotDoAns.FirstOrDefault(d => d.IsActive == true);
            if (dotHienTai != null) { baoCao.DotDoAnId = dotHienTai.Id; }

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

            // =============== BẮN THÔNG BÁO TỰ ĐỘNG ===============
            try
            {
                var dangKy = _context.DangKyDeTais
                    .Include(d => d.SinhVien).Include(d => d.DeTai)
                    .FirstOrDefault(d => d.Id == baoCao.DangKyId);

                if (dangKy != null && dangKy.DeTai != null && dangKy.SinhVien != null)
                {
                    // 1. Gửi cho Giảng viên
                    var gvUser = _context.Users.FirstOrDefault(u => u.GiangVienId == dangKy.DeTai.GiangVienId);
                    if (gvUser != null)
                    {
                        _context.ThongBaos.Add(new ThongBao
                        {
                            UsernameNhan = gvUser.Username,
                            TieuDe = "Sinh viên nộp báo cáo",
                            NoiDung = $"Sinh viên {dangKy.SinhVien.TenSV} vừa tải lên một file báo cáo mới cho đề tài '{dangKy.DeTai.TenDeTai}'.",
                            NgayTao = DateTime.Now,
                            DaDoc = false
                        });
                    }

                    // 2. Gửi cho Admin
                    var adminUser = _context.Users.FirstOrDefault(u => u.Role != null && u.Role.Trim().ToLower() == "admin");
                    if (adminUser != null)
                    {
                        _context.ThongBaos.Add(new ThongBao
                        {
                            UsernameNhan = adminUser.Username,
                            TieuDe = "Hệ thống: Có báo cáo mới",
                            NoiDung = $"Hệ thống vừa ghi nhận file báo cáo mới từ sinh viên {dangKy.SinhVien.TenSV} - Đề tài: '{dangKy.DeTai.TenDeTai}'.",
                            NgayTao = DateTime.Now,
                            DaDoc = false
                        });
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception) { /* Bỏ qua lỗi */ }
            // =====================================================

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var baoCao = _context.BaoCaos.Find(id);
            if (baoCao != null) { _context.BaoCaos.Remove(baoCao); _context.SaveChanges(); }
            return RedirectToAction("Index");
        }

        public IActionResult Download(int id)
        {
            var baoCao = _context.BaoCaos.Find(id);
            if (baoCao == null || string.IsNullOrEmpty(baoCao.DuongDan)) return NotFound();

            string filePath = _webHostEnvironment.WebRootPath + baoCao.DuongDan.Replace("/", "\\");
            if (!System.IO.File.Exists(filePath)) return NotFound();

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", baoCao.TenFile);
        }

        public IActionResult ChamDiem(int id)
        {
            if (!User.IsInRole("GiangVien")) return Unauthorized();

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
            if (!User.IsInRole("GiangVien")) return Unauthorized();

            var baoCao = _context.BaoCaos.Find(id);
            if (baoCao != null)
            {
                baoCao.Diem = Diem;
                baoCao.NhanXet = NhanXet;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}