using System;

namespace webquanli.Models
{
    public class DangKyDeTai
    {
        public int Id { get; set; }
        public int SinhVienId { get; set; }
        public int DeTaiId { get; set; }
        public DateTime NgayDangKy { get; set; }

        // ĐÃ THÊM: Cột trạng thái để khóa chốt đề tài
        public bool IsApproved { get; set; } = false;

        public SinhVien SinhVien { get; set; }
        public DeTai DeTai { get; set; }
    }
}