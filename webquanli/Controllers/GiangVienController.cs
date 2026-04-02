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

        // --- 1. HIỂN THỊ DANH SÁCH ---
        public async Task<IActionResult> Index()
        {
            var giangViens = await _context.GiangViens.Include(g => g.BoMon).ToListAsync();
            return View(giangViens);
        }

        // --- 2. THÊM MỚI ---
        public IActionResult Create()
        {
            // Truyền danh sách bộ môn ra View để làm Dropdown (danh sách thả xuống)
            ViewBag.BoMonId = new SelectList(_context.BoMons, "Id", "TenBoMon");
            return View();
        }

        [HttpPost]
        public IActionResult Create(GiangVien gv)
        {
            _context.GiangViens.Add(gv);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // --- 3. CẬP NHẬT (SỬA) ---
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
            _context.GiangViens.Update(gv);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // --- 4. XÓA ---
        public IActionResult Delete(int id)
        {
            var gv = _context.GiangViens.Find(id);
            if (gv != null)
            {
                _context.GiangViens.Remove(gv);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}