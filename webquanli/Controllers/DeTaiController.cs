using Microsoft.AspNetCore.Mvc;
using webquanli.Data;
using webquanli.Models;
using System.Linq;

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

            return View(_context.DeTais.ToList());

        }

        public IActionResult Create()
        {

            return View();

        }

        [HttpPost]

        public IActionResult Create(DeTai dt)
        {

            _context.DeTais.Add(dt);

            _context.SaveChanges();

            return RedirectToAction("Index");

        }

    }
}