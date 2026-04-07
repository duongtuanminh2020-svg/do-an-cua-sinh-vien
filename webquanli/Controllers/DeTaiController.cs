using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webquanli.Data;
using webquanli.Models;
using System;
using System.Linq;
using System.Collections.Generic;

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
            var query = _context.DeTais
                .Include(d => d.GiangVien).ThenInclude(g => g.BoMon)
                .AsQueryable();

            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);

            if (userAcc != null)
            {
                string role = userAcc.Role?.Trim().ToLower() ?? "";

                if (role == "sinhvien" && userAcc.SinhVienId.HasValue)
                {
                    int svId = userAcc.SinhVienId.Value;
                    var sv = _context.SinhViens.Find(svId);
                    if (sv != null && !string.IsNullOrEmpty(sv.Nganh))
                    {
                        query = query.Where(d => d.GiangVien != null && d.GiangVien.BoMon != null && d.GiangVien.BoMon.TenBoMon.Contains(sv.Nganh));
                    }
                    ViewBag.DeTaiCuaToi = _context.DangKyDeTais.FirstOrDefault(d => d.SinhVienId == svId)?.DeTaiId ?? 0;
                }
                else if (role == "giangvien" && userAcc.GiangVienId.HasValue)
                {
                    int gvId = userAcc.GiangVienId.Value;
                    var gv = _context.GiangViens.Find(gvId);
                    if (gv != null)
                    {
                        query = query.Where(d => d.GiangVien != null && d.GiangVien.BoMonId == gv.BoMonId);
                    }
                }
            }

            var danhSachDeTai = query.ToList();

            var dictDangKy = _context.DangKyDeTais
                .Include(d => d.SinhVien)
                .ToDictionary(d => d.DeTaiId, d => d);
            ViewBag.DictDangKy = dictDangKy;

            return View(danhSachDeTai);
        }

        [HttpPost]
        public IActionResult DangKy(int id)
        {
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);
            if (userAcc == null || userAcc.Role?.Trim().ToLower() != "sinhvien" || !userAcc.SinhVienId.HasValue)
                return Unauthorized();

            int svId = userAcc.SinhVienId.Value;

            if (_context.DangKyDeTais.Any(d => d.SinhVienId == svId))
            {
                TempData["Error"] = "Bạn đã đăng ký một đề tài rồi!";
                return RedirectToAction("Index");
            }
            if (_context.DangKyDeTais.Any(d => d.DeTaiId == id))
            {
                TempData["Error"] = "Đề tài này vừa bị người khác đăng ký mất!";
                return RedirectToAction("Index");
            }

            var deTai = _context.DeTais.Include(d => d.GiangVien).ThenInclude(g => g.BoMon).FirstOrDefault(d => d.Id == id);
            var sinhVien = _context.SinhViens.Find(svId);

            _context.DangKyDeTais.Add(new DangKyDeTai
            {
                SinhVienId = svId,
                DeTaiId = id,
                NgayDangKy = DateTime.Now,
                IsApproved = false // Mặc định là chờ duyệt
            });
            _context.SaveChanges();

            TempData["Success"] = "Đã gửi yêu cầu đăng ký đề tài! Vui lòng chờ giảng viên phê duyệt.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult HuyDangKy(int id)
        {
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);
            if (userAcc == null || userAcc.Role?.Trim().ToLower() != "sinhvien" || !userAcc.SinhVienId.HasValue)
                return Unauthorized();

            var dangKy = _context.DangKyDeTais.FirstOrDefault(d => d.SinhVienId == userAcc.SinhVienId.Value && d.DeTaiId == id);
            if (dangKy != null)
            {
                if (dangKy.IsApproved)
                {
                    TempData["Error"] = "Đề tài đã được giảng viên chốt, bạn không thể tự ý hủy. Vui lòng liên hệ giảng viên!";
                    return RedirectToAction("Index");
                }

                _context.DangKyDeTais.Remove(dangKy);
                _context.SaveChanges();
                TempData["Success"] = "Đã rút lại yêu cầu đăng ký thành công!";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult PheDuyet(int id)
        {
            if (!User.IsInRole("GiangVien")) return Unauthorized();
            var dangKy = _context.DangKyDeTais.Include(d => d.SinhVien).FirstOrDefault(d => d.DeTaiId == id);
            if (dangKy != null)
            {
                dangKy.IsApproved = true;
                _context.SaveChanges();
                TempData["Success"] = $"Đã chốt đề tài cho sinh viên {dangKy.SinhVien.TenSV}. Sinh viên này sẽ không thể tự hủy đề tài nữa.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult GoDuyet(int id)
        {
            if (!User.IsInRole("GiangVien") && !User.IsInRole("Admin")) return Unauthorized();
            var dangKy = _context.DangKyDeTais.Include(d => d.SinhVien).FirstOrDefault(d => d.DeTaiId == id);
            if (dangKy != null)
            {
                string tenSV = dangKy.SinhVien.TenSV;
                _context.DangKyDeTais.Remove(dangKy);
                _context.SaveChanges();
                TempData["Success"] = $"Đã gỡ đề tài của {tenSV}. Đề tài hiện đã trống và sinh viên này có thể đăng ký đề tài khác.";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            var query = _context.GiangViens.AsQueryable();

            if (userAcc?.Role?.Trim().ToLower() == "giangvien" && userAcc.GiangVienId.HasValue)
            {
                var currentGV = _context.GiangViens.Find(userAcc.GiangVienId.Value);
                if (currentGV != null) query = query.Where(g => g.BoMonId == currentGV.BoMonId);
            }
            ViewBag.GiangVienId = new SelectList(query.ToList(), "Id", "TenGV");
            return View();
        }

        [HttpPost]
        public IActionResult Create(DeTai deTai)
        {
            if (string.IsNullOrWhiteSpace(deTai.MoTa)) deTai.MoTa = "Chưa có mô tả cho đề tài này.";
            _context.DeTais.Add(deTai);
            _context.SaveChanges();
            TempData["Success"] = "Đã thêm đề tài mới thành công!";
            return RedirectToAction("Index");
        }

        // ==============================================================
        // ĐÃ NÂNG CẤP BẢO MẬT: Hàm Edit (GET) - Truyền tên Giảng Viên ra View
        // ==============================================================
        public IActionResult Edit(int id)
        {
            var deTai = _context.DeTais.Include(d => d.GiangVien).FirstOrDefault(d => d.Id == id);
            if (deTai == null) return NotFound();
            ViewBag.GiangVienId = new SelectList(_context.GiangViens, "Id", "TenGV", deTai.GiangVienId);
            return View(deTai);
        }

        // ==============================================================
        // ĐÃ NÂNG CẤP BẢO MẬT: Hàm Edit (POST) - Khóa quyền đổi Giảng Viên
        // ==============================================================
        [HttpPost]
        public IActionResult Edit(DeTai deTai)
        {
            var existingDeTai = _context.DeTais.Find(deTai.Id);
            if (existingDeTai == null) return NotFound();

            // Cập nhật các trường được phép
            existingDeTai.TenDeTai = deTai.TenDeTai;
            existingDeTai.MoTa = string.IsNullOrWhiteSpace(deTai.MoTa) ? "Chưa có mô tả cho đề tài này." : deTai.MoTa;

            // Khóa chốt: Chỉ Admin mới được can thiệp đổi người hướng dẫn
            if (User.IsInRole("Admin"))
            {
                existingDeTai.GiangVienId = deTai.GiangVienId;
            }

            _context.SaveChanges();
            TempData["Success"] = "Cập nhật đề tài thành công!";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var deTai = _context.DeTais.Find(id);
            if (deTai != null) { _context.DeTais.Remove(deTai); _context.SaveChanges(); }
            return RedirectToAction("Index");
        }
    }
}