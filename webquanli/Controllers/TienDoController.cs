using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webquanli.Data;
using webquanli.Models;
using System.Linq;
using System;
using System.Security.Claims;

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
            var query = _context.TienDos
                .Include(t => t.DangKyDeTai).ThenInclude(d => d.SinhVien)
                .Include(t => t.DangKyDeTai).ThenInclude(d => d.DeTai)
                .AsQueryable();

            if (User.IsInRole("SinhVien"))
            {
                var claimSinhVienId = User.Claims.FirstOrDefault(c => c.Type == "SinhVienId")?.Value;
                if (int.TryParse(claimSinhVienId, out int sinhVienId))
                {
                    query = query.Where(t => t.DangKyDeTai.SinhVienId == sinhVienId);
                }
            }
            else if (User.IsInRole("GiangVien"))
            {
                var claimGiangVienId = User.Claims.FirstOrDefault(c => c.Type == "GiangVienId")?.Value;
                if (int.TryParse(claimGiangVienId, out int gvId))
                {
                    // Lấy thông tin giảng viên để biết họ quản lý lớp nào
                    var giangVien = _context.GiangViens.Find(gvId);
                    if (giangVien != null && !string.IsNullOrEmpty(giangVien.LopQuanLy))
                    {
                        // Lọc Tiến độ: Chỉ lấy nhóm có sinh viên thuộc Lớp của giảng viên này
                        query = query.Where(t => t.DangKyDeTai.SinhVien.Lop == giangVien.LopQuanLy);
                    }
                }
            }

            var danhSachTienDo = query.OrderByDescending(t => t.NgayCapNhat).ToList();
            return View(danhSachTienDo);
        }

        public IActionResult Create(int? id)
        {
            var queryDangKy = _context.DangKyDeTais
                .Include(d => d.SinhVien).Include(d => d.DeTai)
                .AsQueryable();

            if (User.IsInRole("SinhVien"))
            {
                var claimSinhVienId = User.Claims.FirstOrDefault(c => c.Type == "SinhVienId")?.Value;
                if (int.TryParse(claimSinhVienId, out int sinhVienId))
                {
                    queryDangKy = queryDangKy.Where(d => d.SinhVienId == sinhVienId);
                }
            }
            else if (User.IsInRole("GiangVien"))
            {
                var claimGiangVienId = User.Claims.FirstOrDefault(c => c.Type == "GiangVienId")?.Value;
                if (int.TryParse(claimGiangVienId, out int gvId))
                {
                    var giangVien = _context.GiangViens.Find(gvId);
                    if (giangVien != null && !string.IsNullOrEmpty(giangVien.LopQuanLy))
                    {
                        queryDangKy = queryDangKy.Where(d => d.SinhVien.Lop == giangVien.LopQuanLy);
                    }
                }
            }

            var danhSachDangKy = queryDangKy
                .Select(d => new { Id = d.Id, TenHienThi = d.SinhVien.TenSV + " - " + d.DeTai.TenDeTai })
                .ToList();

            TienDo model = new TienDo();
            if (id.HasValue)
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

            if (tienDo.Id == 0) { _context.TienDos.Add(tienDo); }
            else { _context.TienDos.Update(tienDo); }

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