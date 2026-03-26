using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            return View(_context.DeTais.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(DeTai dt)
        {
            _context.DeTais.Add(dt);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var deTai = _context.DeTais.Find(id);
            if (deTai == null)
            {
                return NotFound();
            }

            ViewBag.DanhSachGiangVien = new SelectList(_context.GiangViens.ToList(), "Id", "TenGV");

            return View(deTai);
        }

        [HttpPost]
        public IActionResult Edit(int id, DeTai dt)
        {
            if (id != dt.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.DeTais.Update(dt);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DanhSachGiangVien = new SelectList(_context.GiangViens.ToList(), "Id", "TenGV");
            return View(dt);
        }

        // HÀM XÓA ĐỀ TÀI (ĐÃ ĐƯỢC THÊM VÀO ĐÂY)
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