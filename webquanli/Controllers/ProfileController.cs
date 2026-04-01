using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Linq;
using webquanli.Data;
using webquanli.Models;

namespace webquanli.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // 1. Hiển thị trang Hồ sơ
        public IActionResult Index()
        {
            if (User.IsInRole("SinhVien"))
            {
                var claimSinhVienId = User.Claims.FirstOrDefault(c => c.Type == "SinhVienId")?.Value;
                if (int.TryParse(claimSinhVienId, out int sinhVienId))
                {
                    var sinhVien = _context.SinhViens.Find(sinhVienId);
                    ViewBag.Role = "SinhVien";
                    return View(sinhVien);
                }
            }
            else if (User.IsInRole("GiangVien"))
            {
                var claimGiangVienId = User.Claims.FirstOrDefault(c => c.Type == "GiangVienId")?.Value;
                if (int.TryParse(claimGiangVienId, out int gvId))
                {
                    var giangVien = _context.GiangViens.Find(gvId);
                    ViewBag.Role = "GiangVien";
                    return View(giangVien);
                }
            }
            else if (User.IsInRole("Admin"))
            {
                // Đối với Admin, ta tạo thông tin mặc định để hiển thị
                ViewBag.Role = "Admin";
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

        // 2. Chức năng Upload Ảnh Đại Diện
        [HttpPost]
        public IActionResult UploadAvatar(IFormFile fileUpload)
        {
            if (fileUpload != null && fileUpload.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "avatars");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = DateTime.Now.ToString("yyyyMMddHHmmss_") + fileUpload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    fileUpload.CopyTo(fileStream);
                }

                string avatarUrl = "/avatars/" + uniqueFileName;

                if (User.IsInRole("SinhVien"))
                {
                    var claimSinhVienId = User.Claims.FirstOrDefault(c => c.Type == "SinhVienId")?.Value;
                    if (int.TryParse(claimSinhVienId, out int sinhVienId))
                    {
                        var sv = _context.SinhViens.Find(sinhVienId);
                        if (sv != null) { sv.Avatar = avatarUrl; _context.SaveChanges(); }
                    }
                }
                else if (User.IsInRole("GiangVien"))
                {
                    var claimGiangVienId = User.Claims.FirstOrDefault(c => c.Type == "GiangVienId")?.Value;
                    if (int.TryParse(claimGiangVienId, out int gvId))
                    {
                        var gv = _context.GiangViens.Find(gvId);
                        if (gv != null) { gv.Avatar = avatarUrl; _context.SaveChanges(); }
                    }
                }
                // Lưu ý: Admin nếu muốn đổi avatar thì cần bảng lưu riêng, hiện tại tạm để cho SV/GV
            }

            return RedirectToAction("Index");
        }
    }
}