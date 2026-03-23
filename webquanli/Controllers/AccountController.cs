using Microsoft.AspNetCore.Mvc;
using webquanli.Data;
using System.Linq;

namespace webquanli.Controllers
{
    public class AccountController : Controller
    {

        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        public IActionResult Login(string username, string password)
        {

            var user = _context.Users
                .FirstOrDefault(x => x.Username == username && x.Password == password);

            if (user != null)
            {

                HttpContext.Session.SetString("user", user.Username);

                return RedirectToAction("Index", "Home");

            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu";

            return View();

        }

        public IActionResult Logout()
        {

            HttpContext.Session.Clear();

            return RedirectToAction("Login");

        }

    }
}