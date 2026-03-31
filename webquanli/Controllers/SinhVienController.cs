using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webquanli.Data;
using webquanli.Models;
using System.Linq;
using System.Security.Claims;

namespace webquanli.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var query = _context.SinhViens.AsQueryable();

            if (User.IsInRole("GiangVien"))
            {
                var claimGiangVienId = User.Claims.FirstOrDefault(c => c.Type == "GiangVienId")?.Value;
                if (int.TryParse(claimGiangVienId, out int gvId))
                {
                    var giangVien = _context.GiangViens.Find(gvId);
                    if (giangVien != null && !string.IsNullOrEmpty(giangVien.LopQuanLy))
                    {
                        query = query.Where(sv => sv.Lop == giangVien.LopQuanLy);
                    }
                }
            }

            var list = query.ToList();
            return View(list);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(SinhVien sv)
        {
            _context.SinhViens.Add(sv);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var sv = _context.SinhViens.Find(id);
            if (sv == null) return NotFound();
            return View(sv);
        }

        [HttpPost]
        public IActionResult Edit(SinhVien sv)
        {
            _context.SinhViens.Update(sv);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // HÀM XÓA ĐÃ ĐƯỢC CHỐNG LỖI CƠ SỞ DỮ LIỆU
        public IActionResult Delete(int id)
        {
            var sv = _context.SinhViens.Find(id);
            if (sv == null) return NotFound();

            // 1. Phải xóa tài khoản đăng nhập của sinh viên này trước
            var userLienQuan = _context.Users.FirstOrDefault(u => u.SinhVienId == id);
            if (userLienQuan != null)
            {
                _context.Users.Remove(userLienQuan);
                _context.SaveChanges(); // Chốt xóa tài khoản ngay lập tức
            }

            // 2. Sau đó mới xóa sinh viên
            _context.SinhViens.Remove(sv);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}