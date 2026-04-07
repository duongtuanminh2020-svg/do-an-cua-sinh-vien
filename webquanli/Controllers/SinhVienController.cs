using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webquanli.Data;
using webquanli.Models;
using System.Linq;
using System.Collections.Generic;

namespace webquanli.Controllers
{
    public class SinhVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SinhVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var query = _context.SinhViens.Include(s => s.BoMon).AsQueryable();
            ViewBag.ListBoMon = _context.BoMons.ToList();

            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);

            if (userAcc != null)
            {
                string role = userAcc.Role?.Trim().ToLower() ?? "";

                // NGHIỆP VỤ: Nếu là Giảng viên, áp dụng bộ lọc chia tải
                if (role == "giangvien" && userAcc.GiangVienId.HasValue)
                {
                    var giangVien = _context.GiangViens.FirstOrDefault(g => g.Id == userAcc.GiangVienId.Value);

                    if (giangVien != null && giangVien.BoMonId.HasValue)
                    {
                        // LỌC LẦN 1: Bắt buộc phải cùng chuyên ngành (Bộ môn)
                        query = query.Where(sv => sv.BoMonId == giangVien.BoMonId);

                        // Lấy dữ liệu lên RAM để tiến hành cắt chuỗi lớp
                        var listSv = query.ToList();

                        // LỌC LẦN 2: Chia tải theo Lớp quản lý (VD: "D2, D3, D4")
                        if (!string.IsNullOrWhiteSpace(giangVien.LopQuanLy) && giangVien.LopQuanLy != "Chưa phân công")
                        {
                            // Tách chuỗi thành một danh sách các lớp và xóa khoảng trắng thừa
                            var dsLopCuaGiangVien = giangVien.LopQuanLy.Split(',')
                                                                       .Select(l => l.Trim().ToLower())
                                                                       .ToList();

                            // Chỉ giữ lại những sinh viên mà lớp của họ có trong danh sách trên
                            listSv = listSv.Where(sv => !string.IsNullOrEmpty(sv.Lop) &&
                                                        dsLopCuaGiangVien.Contains(sv.Lop.Trim().ToLower())).ToList();
                        }
                        else
                        {
                            // Nếu Thầy/Cô chưa được phân công lớp nào thì danh sách trống trơn
                            listSv = new List<SinhVien>();
                        }

                        return View(listSv);
                    }
                }
            }

            // Dành cho Admin: Thấy tất cả
            return View(query.ToList());
        }

        public IActionResult Create()
        {
            ViewData["BoMonId"] = new SelectList(_context.BoMons, "Id", "TenBoMon");
            return View();
        }

        [HttpPost]
        public IActionResult Create(SinhVien sv)
        {
            _context.SinhViens.Add(sv);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var sv = _context.SinhViens.Find(id);
            if (sv == null) return NotFound();

            ViewData["BoMonId"] = new SelectList(_context.BoMons, "Id", "TenBoMon", sv.BoMonId);
            return View(sv);
        }

        [HttpPost]
        public IActionResult Edit(SinhVien sv)
        {
            var existingSv = _context.SinhViens.Find(sv.Id);
            if (existingSv == null) return NotFound();

            existingSv.TenSV = sv.TenSV;
            existingSv.Email = sv.Email;

            if (existingSv.BoMonId == null && sv.BoMonId != null)
            {
                existingSv.BoMonId = sv.BoMonId;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var sv = _context.SinhViens.Find(id);
            if (sv == null) return NotFound();

            if (sv.BoMonId != null)
            {
                TempData["ErrorMessage"] = "BẢO MẬT HỆ THỐNG: Sinh viên này đã có dữ liệu ràng buộc (Đã đăng ký Bộ môn). Không được phép xóa!";
                return RedirectToAction("Index");
            }

            var userLienQuan = _context.Users.FirstOrDefault(u => u.SinhVienId == id);
            if (userLienQuan != null)
            {
                _context.Users.Remove(userLienQuan);
            }

            _context.SinhViens.Remove(sv);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}