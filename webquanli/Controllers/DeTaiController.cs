using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webquanli.Data;
using webquanli.Models;
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
            var danhSachDeTai = _context.DeTais.Include(d => d.GiangVien).ToList();
            return View(danhSachDeTai);
        }

        public IActionResult Create()
        {
            // Đã đổi "Id" thành "TenGV" để hiển thị tên trên giao diện
            ViewBag.GiangVienId = new SelectList(_context.GiangViens, "Id", "TenGV");
            return View();
        }

        [HttpPost]
        public IActionResult Create(DeTai deTai)
        {
            _context.DeTais.Add(deTai);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var deTai = _context.DeTais.Find(id);
            if (deTai == null) return NotFound();

            // Đã đổi "Id" thành "TenGV" để hiển thị tên trên giao diện
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