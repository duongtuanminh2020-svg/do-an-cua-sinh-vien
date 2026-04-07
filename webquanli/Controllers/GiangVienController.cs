using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webquanli.Data;
using webquanli.Models;
using System.Threading.Tasks;
using System.Linq;

namespace webquanli.Controllers
{
    public class GiangVienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GiangVienController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var giangViens = await _context.GiangViens.Include(g => g.BoMon).ToListAsync();
            ViewBag.ListBoMon = await _context.BoMons.ToListAsync();
            return View(giangViens);
        }

        public IActionResult Create()
        {
            ViewBag.BoMonId = new SelectList(_context.BoMons, "Id", "TenBoMon");
            return View();
        }

        [HttpPost]
        public IActionResult Create(GiangVien gv)
        {
            if (string.IsNullOrWhiteSpace(gv.LopQuanLy)) gv.LopQuanLy = "Chưa phân công";
            _context.GiangViens.Add(gv);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var gv = _context.GiangViens.Find(id);
            if (gv == null) return NotFound();

            ViewBag.BoMonId = new SelectList(_context.BoMons, "Id", "TenBoMon", gv.BoMonId);
            return View(gv);
        }

        [HttpPost]
        public IActionResult Edit(GiangVien gv)
        {
            var existingGv = _context.GiangViens.Find(gv.Id);
            if (existingGv == null) return NotFound();

            existingGv.TenGV = gv.TenGV;
            existingGv.Email = gv.Email;
            existingGv.SDT = gv.SDT;

            // Xử lý chống văng lỗi DB và chuẩn hóa đầu vào Lớp quản lý
            if (string.IsNullOrWhiteSpace(gv.LopQuanLy))
            {
                existingGv.LopQuanLy = "Chưa phân công";
            }
            else
            {
                // Chỉ cho phép cập nhật nếu trước đó chưa phân công (bảo vệ khi đã khóa)
                if (existingGv.LopQuanLy == "Chưa phân công" || string.IsNullOrWhiteSpace(existingGv.LopQuanLy))
                {
                    existingGv.LopQuanLy = gv.LopQuanLy.Trim();
                }
            }

            if (existingGv.BoMonId == null && gv.BoMonId != null)
            {
                existingGv.BoMonId = gv.BoMonId;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var gv = _context.GiangViens.Find(id);
            if (gv == null) return NotFound();

            if (gv.BoMonId != null)
            {
                TempData["ErrorMessage"] = "BẢO MẬT HỆ THỐNG: Giảng viên này đã được phân bổ về Bộ môn (có thể đang hướng dẫn đề tài). Không được phép xóa!";
                return RedirectToAction("Index");
            }

            var userLienQuan = _context.Users.FirstOrDefault(u => u.GiangVienId == id);
            if (userLienQuan != null)
            {
                _context.Users.Remove(userLienQuan);
            }

            _context.GiangViens.Remove(gv);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}