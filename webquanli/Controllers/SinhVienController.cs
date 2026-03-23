using Microsoft.AspNetCore.Mvc;
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

            var list = _context.SinhViens.ToList();

            return View(list);

        }

        public IActionResult Create()
        {
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

            _context.SinhViens.Remove(sv);

            _context.SaveChanges();

            return RedirectToAction("Index");

        }

    }
}