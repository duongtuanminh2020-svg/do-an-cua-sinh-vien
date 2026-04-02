using Microsoft.AspNetCore.Mvc;
using webquanli.Data;
using System.Linq;

namespace webquanli.Controllers
{
    public class ThongBaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongBaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult DanhDauDaDoc()
        {
            var username = User.Identity.Name;

            // Tìm tất cả thông báo của người này mà chưa đọc
            var dsChuaDoc = _context.ThongBaos
                .Where(t => t.UsernameNhan == username && t.DaDoc == false)
                .ToList();

            if (dsChuaDoc.Any())
            {
                // Chuyển hết thành "Đã đọc"
                foreach (var tb in dsChuaDoc)
                {
                    tb.DaDoc = true;
                }
                _context.SaveChanges();
            }

            // F5 tải lại đúng cái trang mà người dùng đang đứng
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public IActionResult Xoa(int id)
        {
            // 1. Xác định ai đang đăng nhập
            var username = User.Identity.Name;

            // 2. Tìm đúng cái thông báo đó (và phải chắc chắn nó thuộc về người này để chống hack)
            var thongBao = _context.ThongBaos.FirstOrDefault(t => t.Id == id && t.UsernameNhan == username);

            if (thongBao != null)
            {
                // 3. Xóa khỏi CSDL và Lưu lại
                _context.ThongBaos.Remove(thongBao);
                _context.SaveChanges();
            }

            // 4. F5 tải lại đúng cái trang mà người dùng đang đứng
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}