using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using webquanli.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace webquanli.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Nếu đã đăng nhập rồi thì không hiện trang Login nữa
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Kiểm tra tài khoản trong database
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // TỈ MỈ: Tạo "chứng minh thư" điện tử cho người dùng
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role) // Lưu quyền (Admin/GiangVien/SinhVien)
                };

                // ĐIỂM NÂNG CẤP: Lưu thêm ID thực tế vào Cookie nếu có
                if (user.GiangVienId.HasValue)
                {
                    claims.Add(new Claim("GiangVienId", user.GiangVienId.Value.ToString()));
                }
                if (user.SinhVienId.HasValue)
                {
                    claims.Add(new Claim("SinhVienId", user.SinhVienId.Value.ToString()));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Cấu hình ghi nhớ đăng nhập
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Giúp ghi nhớ đăng nhập ngay cả khi đóng trình duyệt
                };

                // Lệnh quan trọng nhất: Ký thẻ Cookie và lưu vào trình duyệt
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // ĐIỂM NÂNG CẤP: Điều hướng người dùng dựa theo vai trò
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Home"); // Admin về trang tổng quan
                }
                else if (user.Role == "GiangVien")
                {
                    return RedirectToAction("Index", "TienDo"); // Giảng viên vào trang Tiến độ
                }
                else if (user.Role == "SinhVien")
                {
                    return RedirectToAction("Index", "TienDo"); // Sinh viên vào trang Tiến độ
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Tài khoản hoặc mật khẩu không chính xác!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            // Xóa sạch Cookie khi đăng xuất
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}