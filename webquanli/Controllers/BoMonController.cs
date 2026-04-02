using Microsoft.AspNetCore.Mvc;
using System.Linq;
using webquanli.Data;
using webquanli.Models;
using static Azure.Core.HttpHeader;

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

        // 4. Xóa bộ môn
        public IActionResult Delete(int id)
        {
            var boMon = _context.BoMons.Find(id);
            if (boMon != null)
            {
                _context.BoMons.Remove(boMon);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}