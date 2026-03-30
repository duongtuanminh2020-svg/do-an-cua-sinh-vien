using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webquanli.Data;
using webquanli.Models;
using System.Linq;
using System;

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
            var danhSachTienDo = _context.TienDos
                .Include(t => t.DangKyDeTai).ThenInclude(d => d.SinhVien)
                .Include(t => t.DangKyDeTai).ThenInclude(d => d.DeTai)
                .OrderByDescending(t => t.NgayCapNhat).ToList();
            return View(danhSachTienDo);
        }

        // Hàm này dùng chung cho cả Thêm và Sửa
        public IActionResult Create(int? id)
        {
            var danhSachDangKy = _context.DangKyDeTais
                .Include(d => d.SinhVien).Include(d => d.DeTai)
                .Select(d => new { Id = d.Id, TenHienThi = d.SinhVien.TenSV + " - " + d.DeTai.TenDeTai }).ToList();

            TienDo model = new TienDo();
            if (id.HasValue) // Nếu có ID -> Đang đi Sửa
            {
                model = _context.TienDos.Find(id.Value);
                if (model == null) return NotFound();
                ViewBag.Title = "Chỉnh sửa tiến độ";
            }
            else { ViewBag.Title = "Cập nhật tiến độ mới"; }

            ViewBag.ListDangKy = new SelectList(danhSachDangKy, "Id", "TenHienThi", model.DangKyId);
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(TienDo tienDo)
        {
            tienDo.NgayCapNhat = DateTime.Now;
            if (tienDo.PhanTram < 0) tienDo.PhanTram = 0;
            if (tienDo.PhanTram > 100) tienDo.PhanTram = 100;

            if (tienDo.Id == 0) { _context.TienDos.Add(tienDo); } // ID = 0 là Thêm mới
            else { _context.TienDos.Update(tienDo); } // Có ID là Cập nhật

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var tienDo = _context.TienDos.Find(id);
            if (tienDo != null) { _context.TienDos.Remove(tienDo); _context.SaveChanges(); }
            return RedirectToAction("Index");
        }
    }
}