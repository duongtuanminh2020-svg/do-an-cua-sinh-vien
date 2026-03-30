using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using webquanli.Data;
using webquanli.Models;
using System.Linq;
using System;
using System.IO;

namespace webquanli.Controllers
{
    public class BaoCaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BaoCaoController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var danhSachBaoCao = _context.BaoCaos
                .Include(b => b.DangKyDeTai).ThenInclude(d => d.SinhVien)
                .Include(b => b.DangKyDeTai).ThenInclude(d => d.DeTai)
                .ToList();
            return View(danhSachBaoCao);
        }

        public IActionResult Upload()
        {
            var danhSachDangKy = _context.DangKyDeTais
                .Include(d => d.SinhVien)
                .Include(d => d.DeTai)
                .Select(d => new { Id = d.Id, TenHienThi = d.SinhVien.TenSV + " - " + d.DeTai.TenDeTai })
                .ToList();

            ViewBag.ListDangKy = new SelectList(danhSachDangKy, "Id", "TenHienThi");
            return View();
        }

        [HttpPost]
        public IActionResult Upload(BaoCao baoCao, IFormFile fileUpload)
        {
            baoCao.NgayNop = DateTime.Now;
            baoCao.DangKyDeTaiId = baoCao.DangKyId; // Giải quyết lỗi khóa ngoại

            // PHẦN CODE MỚI: Xử lý lưu file THẬT vào máy tính
            if (fileUpload != null && fileUpload.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = DateTime.Now.ToString("yyyyMMddHHmmss_") + fileUpload.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    fileUpload.CopyTo(fileStream);
                }

                baoCao.TenFile = fileUpload.FileName;
                baoCao.DuongDan = "/uploads/" + uniqueFileName;
            }

            _context.BaoCaos.Add(baoCao);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // PHẦN CODE MỚI: Xử lý nút Xóa (Sửa lỗi 404 Not Found)
        public IActionResult Delete(int id)
        {
            var baoCao = _context.BaoCaos.Find(id);
            if (baoCao != null)
            {
                _context.BaoCaos.Remove(baoCao);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // PHẦN CODE MỚI: Xử lý nút Tải Về
        public IActionResult Download(int id)
        {
            var baoCao = _context.BaoCaos.Find(id);
            if (baoCao == null || string.IsNullOrEmpty(baoCao.DuongDan)) return NotFound();

            string filePath = _webHostEnvironment.WebRootPath + baoCao.DuongDan.Replace("/", "\\");
            if (!System.IO.File.Exists(filePath)) return NotFound();

            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/octet-stream", baoCao.TenFile);
        }
    }
}