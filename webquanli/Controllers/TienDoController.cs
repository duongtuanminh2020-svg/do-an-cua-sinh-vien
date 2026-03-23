using Microsoft.AspNetCore.Mvc;
using webquanli.Data;
using webquanli.Models;
using System.Linq;

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

            return View(_context.TienDos.ToList());

        }

        public IActionResult Create()
        {

            return View();

        }

        [HttpPost]

        public IActionResult Create(TienDo td)
        {

            _context.TienDos.Add(td);

            _context.SaveChanges();

            return RedirectToAction("Index");

        }

    }
}