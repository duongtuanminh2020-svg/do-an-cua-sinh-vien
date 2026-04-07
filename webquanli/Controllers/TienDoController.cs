using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webquanli.Data;
using System.Linq;
using System;
using System.Collections.Generic;

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
            // Bắt lỗi hoa thường cho phân quyền
            bool isSinhVien = User.IsInRole("SinhVien") || User.IsInRole("sinhvien");
            bool isGiangVien = User.IsInRole("GiangVien") || User.IsInRole("giangvien");
            bool isAdmin = User.IsInRole("Admin") || User.IsInRole("admin");

            int tongSoDot = _context.DotDoAns.Count(d => d.IsActive);
            ViewBag.TongSoDot = tongSoDot;

            var query = _context.DangKyDeTais
                .Include(d => d.SinhVien).ThenInclude(s => s.BoMon)
                .Include(d => d.DeTai)
                .AsQueryable();

            if (isSinhVien)
            {
                var claimSinhVienId = User.Claims.FirstOrDefault(c => c.Type == "SinhVienId")?.Value;
                if (int.TryParse(claimSinhVienId, out int sinhVienId))
                {
                    query = query.Where(d => d.SinhVienId == sinhVienId);

                    int soBaoCaoDaCham = _context.BaoCaos.Count(b => b.DangKyDeTai.SinhVienId == sinhVienId && b.Diem != null);
                    ViewBag.SoBaoCaoDaCham = soBaoCaoDaCham;
                    ViewBag.PhanTramTong = (tongSoDot > 0) ? (int)Math.Round((double)soBaoCaoDaCham / tongSoDot * 100) : 0;

                    // MỚI: Lấy danh sách toàn bộ đợt để vẽ Timeline
                    var danhSachDot = _context.DotDoAns.OrderBy(d => d.NgayBatDau).ToList();
                    ViewBag.DanhSachDot = danhSachDot;

                    var now = DateTime.Now;
                    // Lọc ra đợt ĐANG MỞ (Hiện tại nằm giữa Bắt đầu và Kết thúc)
                    ViewBag.DotHienTai = danhSachDot.FirstOrDefault(d => d.NgayBatDau <= now && d.NgayKetThuc >= now);
                    // Lọc ra đợt SẮP MỞ (Tương lai)
                    ViewBag.DotSapToi = danhSachDot.FirstOrDefault(d => d.NgayBatDau > now);

                    // Lấy ID các đợt mà sinh viên này ĐÃ NỘP để bôi xanh trên Timeline
                    ViewBag.SubmittedDotIds = _context.BaoCaos
                        .Where(b => b.DangKyDeTai.SinhVienId == sinhVienId && b.DotDoAnId != null)
                        .Select(b => b.DotDoAnId.Value)
                        .ToList();
                }
            }
            else if (isGiangVien)
            {
                var claimGiangVienId = User.Claims.FirstOrDefault(c => c.Type == "GiangVienId")?.Value;
                if (int.TryParse(claimGiangVienId, out int gvId))
                {
                    query = query.Where(d => d.DeTai.GiangVienId == gvId);
                }
            }
            else if (isAdmin)
            {
                ViewBag.ListBoMon = _context.BoMons.ToList();
            }

            var danhSachDangKy = query.ToList();

            var dictPhanTram = new Dictionary<int, int>();
            foreach (var dk in danhSachDangKy)
            {
                int soBaoCao = _context.BaoCaos.Count(b => b.DangKyDeTaiId == dk.Id && b.Diem != null);
                dictPhanTram[dk.Id] = (tongSoDot > 0) ? (int)Math.Round((double)soBaoCao / tongSoDot * 100) : 0;
            }
            ViewBag.DictPhanTram = dictPhanTram;

            return View(danhSachDangKy);
        }
    }
}