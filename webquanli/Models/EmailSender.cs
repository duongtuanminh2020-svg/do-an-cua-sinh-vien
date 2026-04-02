using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

// Lưu ý: Chỗ này phải ghi đúng tên webquanli.Models để Controller tìm thấy
namespace webquanli.Models
{
    public static class EmailSender
    {
        public static async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Tạm thời cứ để nguyên email ảo này để test code hết báo lỗi trước nhé.
            // Sau khi code chạy ngon lành, bạn mới thay email thật và mật khẩu ứng dụng của bạn vào đây.
            string fromEmail = "email-cua-ban@gmail.com";
            string appPassword = "xxxx xxxx xxxx xxxx";

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(fromEmail, "Hệ thống Quản lý Đồ án");
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(fromEmail, appPassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
        }
    }
}