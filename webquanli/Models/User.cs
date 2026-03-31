using System.ComponentModel.DataAnnotations.Schema;

namespace webquanli.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        // Cột mới: Dùng để ghi chữ "Admin", "GiangVien", hoặc "SinhVien"
        public string Role { get; set; }

        // Cột mới: Nối tài khoản này với 1 Giảng viên cụ thể
        public int? GiangVienId { get; set; }
        [ForeignKey("GiangVienId")]
        public virtual GiangVien GiangVien { get; set; }

        // Cột mới: Nối tài khoản này với 1 Sinh viên cụ thể
        public int? SinhVienId { get; set; }
        [ForeignKey("SinhVienId")]
        public virtual SinhVien SinhVien { get; set; }
    }
}