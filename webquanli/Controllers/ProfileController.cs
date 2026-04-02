using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore; // Thêm thư viện này để dùng được lệnh Include
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
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);

            if (userAcc == null) return RedirectToAction("Index", "Home");

            string role = userAcc.Role?.Trim().ToLower() ?? "";

            if (role == "sinhvien" && userAcc.SinhVienId.HasValue)
            {
                var sinhVien = _context.SinhViens.Find(userAcc.SinhVienId.Value);
                ViewBag.Role = "SinhVien";
                return View(sinhVien);
            }
            else if (role == "giangvien" && userAcc.GiangVienId.HasValue)
            {
                // Thay lệnh Find() bằng lệnh Include để nối bảng lấy Tên Bộ Môn
                var giangVien = _context.GiangViens
                    .Include(g => g.BoMon)
                    .FirstOrDefault(g => g.Id == userAcc.GiangVienId.Value);

                ViewBag.Role = "GiangVien";
                return View(giangVien);
            }
            else if (role == "admin")
            {
                ViewBag.Role = "Admin";
                return View();
            }

            return RedirectToAction("Index", "Home");
        }

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
                var username = User.Identity.Name;
                var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);
                string role = userAcc?.Role?.Trim().ToLower() ?? "";

                if (role == "sinhvien" && userAcc.SinhVienId.HasValue)
                {
                    var sv = _context.SinhViens.Find(userAcc.SinhVienId.Value);
                    if (sv != null) { sv.Avatar = avatarUrl; _context.SaveChanges(); }
                }
                else if (role == "giangvien" && userAcc.GiangVienId.HasValue)
                {
                    var gv = _context.GiangViens.Find(userAcc.GiangVienId.Value);
                    if (gv != null) { gv.Avatar = avatarUrl; _context.SaveChanges(); }
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteAvatar()
        {
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);
            string role = userAcc?.Role?.Trim().ToLower() ?? "";

            if (role == "sinhvien" && userAcc.SinhVienId.HasValue)
            {
                var sv = _context.SinhViens.Find(userAcc.SinhVienId.Value);
                if (sv != null) { sv.Avatar = null; _context.SinhViens.Update(sv); _context.SaveChanges(); }
            }
            else if (role == "giangvien" && userAcc.GiangVienId.HasValue)
            {
                var gv = _context.GiangViens.Find(userAcc.GiangVienId.Value);
                if (gv != null) { gv.Avatar = null; _context.GiangViens.Update(gv); _context.SaveChanges(); }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu mới và xác nhận mật khẩu không trùng khớp!";
                return RedirectToAction("Index");
            }
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);
            if (userAcc == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin tài khoản!";
                return RedirectToAction("Index");
            }
            if (userAcc.Password != oldPassword)
            {
                TempData["Error"] = "Mật khẩu cũ không chính xác!";
                return RedirectToAction("Index");
            }
            userAcc.Password = newPassword;
            _context.Users.Update(userAcc);
            _context.SaveChanges();
            TempData["Success"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Index");
        }
    }
}