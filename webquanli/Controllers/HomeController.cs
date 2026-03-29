using Microsoft.AspNetCore.Mvc;
using System.Linq;
using webquanli.Data;
using System.Collections.Generic;
using System;

namespace webquanli.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TongSinhVien = _context.SinhViens.Count();
            ViewBag.TongDeTai = _context.DeTais.Count();
            ViewBag.TongBaoCao = _context.BaoCaos.Count();

            if (_context.TienDos.Any())
            {
                ViewBag.TienDoTrungBinh = (int)_context.TienDos.Average(t => t.PhanTram);
            }
            else
            {
                ViewBag.TienDoTrungBinh = 0;
            }

            ViewBag.HoanThanh = _context.TienDos.Count(t => t.PhanTram == 100);
            ViewBag.DangThucHien = _context.TienDos.Count(t => t.PhanTram > 0 && t.PhanTram < 100);
            ViewBag.ChuaBatDau = _context.TienDos.Count(t => t.PhanTram == 0);

            var duLieuTuan = new List<int>();
            for (int i = 5; i >= 0; i--)
            {
                var ngayBatDau = DateTime.Now.Date.AddDays(-i * 7);
                var ngayKetThuc = ngayBatDau.AddDays(7);
                var soLuong = _context.BaoCaos.Count(b => b.NgayNop >= ngayBatDau && b.NgayNop < ngayKetThuc);
                duLieuTuan.Add(soLuong);
            }
            ViewBag.DuLieuBaoCao = string.Join(",", duLieuTuan);

            return View();
        }
    }
}