using Microsoft.AspNetCore.Mvc;
using System.Linq;
using webquanli.Data;
using webquanli.Models;

namespace webquanli.Controllers
{
    public class BoMonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BoMonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Hiển thị danh sách bộ môn
        public IActionResult Index()
        {
            var danhSachBoMon = _context.BoMons.ToList();
            return View(danhSachBoMon);
        }

        // 2. Mở form thêm mới
        public IActionResult Create()
        {
            return View();
        }

        // 3. Lưu bộ môn mới vào Database
        [HttpPost]
        public IActionResult Create(BoMon boMon)
        {
            if (ModelState.IsValid)
            {
                _context.BoMons.Add(boMon);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(boMon);
        }

        // ==========================================
        // TÍNH NĂNG MỚI: CHỈNH SỬA BỘ MÔN
        // ==========================================
        public IActionResult Edit(int id)
        {
            var boMon = _context.BoMons.Find(id);
            if (boMon == null) return NotFound();
            return View(boMon);
        }

        [HttpPost]
        public IActionResult Edit(BoMon boMon)
        {
            if (ModelState.IsValid)
            {
                var existingBoMon = _context.BoMons.Find(boMon.Id);
                if (existingBoMon != null)
                {
                    // Chỉ cập nhật tên, ID giữ nguyên để không ảnh hưởng SV/GV
                    existingBoMon.TenBoMon = boMon.TenBoMon;
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(boMon);
        }

        // ==========================================
        // 4. XÓA BỘ MÔN (CÓ LỚP BẢO VỆ CHỐNG HACK)
        // ==========================================
        public IActionResult Delete(int id)
        {
            var boMon = _context.BoMons.Find(id);
            if (boMon == null) return NotFound();

            // LỚP BẢO VỆ BACKEND: Kiểm tra xem có ai đang ở trong bộ môn này không
            bool hasSinhVien = _context.SinhViens.Any(s => s.BoMonId == id);
            bool hasGiangVien = _context.GiangViens.Any(g => g.BoMonId == id);

            if (hasSinhVien || hasGiangVien)
            {
                TempData["ErrorMessage"] = "BẢO MẬT HỆ THỐNG: Bộ môn này đang có Sinh viên hoặc Giảng viên trực thuộc. Không được phép xóa để bảo vệ an toàn dữ liệu!";
                return RedirectToAction("Index");
            }

            _context.BoMons.Remove(boMon);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}