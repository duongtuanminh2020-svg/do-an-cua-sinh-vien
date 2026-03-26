using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using webquanli.Data;
using webquanli.Models;

namespace webquanli.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BaoCaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var baoCaos = await _context.BaoCaos
                .Include(b => b.DangKyDeTai)
                    .ThenInclude(dk => dk.SinhVien)
                .Include(b => b.DangKyDeTai)
                    .ThenInclude(dk => dk.DeTai)
                .ToListAsync();

            return View(baoCaos);
        }

        public IActionResult Upload()
        {
            var danhSachDangKy = _context.DangKyDeTais
                .Include(dk => dk.SinhVien)
                .Include(dk => dk.DeTai)
                .Select(dk => new
                {
                    Id = dk.Id,
                    HienThi = dk.SinhVien.TenSV + " - " + dk.DeTai.TenDeTai
                }).ToList();

            ViewBag.DanhSachDangKy = new SelectList(danhSachDangKy, "Id", "HienThi");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(Microsoft.AspNetCore.Http.IFormFile fileTaiLen, int DangKyId)
        {
            if (fileTaiLen != null && fileTaiLen.Length > 0)
            {
                string tenFileMoi = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fileTaiLen.FileName;
                string duongDanThuMuc = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                string duongDanFull = Path.Combine(duongDanThuMuc, tenFileMoi);

                if (!Directory.Exists(duongDanThuMuc))
                {
                    Directory.CreateDirectory(duongDanThuMuc);
                }

                using (var stream = new FileStream(duongDanFull, FileMode.Create))
                {
                    await fileTaiLen.CopyToAsync(stream);
                }

                BaoCao baoCaoMoi = new BaoCao();

                baoCaoMoi.DangKyId = DangKyId;
                baoCaoMoi.DangKyDeTai = await _context.DangKyDeTais.FindAsync(DangKyId);
                baoCaoMoi.TenFile = fileTaiLen.FileName;
                baoCaoMoi.DuongDan = "/uploads/" + tenFileMoi;
                baoCaoMoi.NgayNop = DateTime.Now;

                _context.BaoCaos.Add(baoCaoMoi);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var danhSachDangKy = _context.DangKyDeTais
                .Include(dk => dk.SinhVien)
                .Include(dk => dk.DeTai)
                .Select(dk => new
                {
                    Id = dk.Id,
                    HienThi = dk.SinhVien.TenSV + " - " + dk.DeTai.TenDeTai
                }).ToList();

            ViewBag.DanhSachDangKy = new SelectList(danhSachDangKy, "Id", "HienThi");
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var baoCao = await _context.BaoCaos.FindAsync(id);
            if (baoCao != null)
            {
                string duongDanFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", baoCao.DuongDan.TrimStart('/'));
                if (System.IO.File.Exists(duongDanFile))
                {
                    System.IO.File.Delete(duongDanFile);
                }

                _context.BaoCaos.Remove(baoCao);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}