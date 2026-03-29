using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webquanli.Data;
using webquanli.Models;
using System;
using System.Linq;

namespace webquanli.Controllers
{
    public class TienDoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TienDoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var danhSachTienDo = _context.TienDos.ToList();
            return View(danhSachTienDo);
        }

        public IActionResult Create()
        {
            var danhSach = _context.DangKyDeTais
                .Include(d => d.SinhVien)
                .Include(d => d.DeTai)
                .Select(d => new {
                    Id = d.Id,
                    TenHienThi = d.SinhVien.TenSV + " - " + d.DeTai.TenDeTai
                }).ToList();

            ViewBag.DanhSachDangKy = new SelectList(danhSach, "Id", "TenHienThi");
            return View();
        }

        [HttpPost]
        public IActionResult Create(TienDo td)
        {
            ModelState.Clear();
            td.NgayCapNhat = DateTime.Now;
            _context.TienDos.Add(td);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var tienDo = _context.TienDos.Find(id);
            if (tienDo != null)
            {
                _context.TienDos.Remove(tienDo);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}