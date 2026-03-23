using Microsoft.AspNetCore.Mvc;
using webquanli.Data;
using webquanli.Models;
using System.IO;
using System.Linq;

namespace webquanli.Controllers
{
    public class BaoCaoController : Controller
    {

        private readonly ApplicationDbContext _context;

        public BaoCaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            return View(_context.BaoCaos.ToList());

        }

        public IActionResult Upload()
        {

            return View();

        }

        [HttpPost]

        public IActionResult Upload(IFormFile file)
        {

            var path = Path.Combine("wwwroot/files", file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {

                file.CopyTo(stream);

            }

            return RedirectToAction("Index");

        }

    }
}