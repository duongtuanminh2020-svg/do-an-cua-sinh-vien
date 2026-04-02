using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webquanli.Data;
using webquanli.Models;
using System.Linq;

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

            // ĐỌC QUYỀN TRỰC TIẾP TỪ DATABASE
            var username = User.Identity.Name;
            var userAcc = _context.Users.FirstOrDefault(u => u.Username == username);

            if (userAcc != null)
            {
                string role = userAcc.Role?.Trim().ToLower() ?? "";

                // NGHIỆP VỤ: Nếu là Giảng viên, chỉ hiện sinh viên cùng ngành
                if (role == "giangvien" && userAcc.GiangVienId.HasValue)
                {
                    var giangVien = _context.GiangViens
                        .Include(g => g.BoMon)
                        .FirstOrDefault(g => g.Id == userAcc.GiangVienId.Value);

                    if (giangVien?.BoMon != null)
                    {
                        string tenBoMon = giangVien.BoMon.TenBoMon.Trim().ToLower();
                        query = query.Where(sv => sv.Nganh != null && sv.Nganh.ToLower().Contains(tenBoMon));
                    }
                }
            }

            var list = query.ToList();
            return View(list);
        }

        // GET: SinhVien/Create
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

        // GET: SinhVien/Edit/5
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
            _context.SinhViens.Update(sv);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var sv = _context.SinhViens.Find(id);
            if (sv == null) return NotFound();

            var userLienQuan = _context.Users.FirstOrDefault(u => u.SinhVienId == id);
            if (userLienQuan != null)
            {
                _context.Users.Remove(userLienQuan);
                _context.SaveChanges();
            }

            _context.SinhViens.Remove(sv);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}