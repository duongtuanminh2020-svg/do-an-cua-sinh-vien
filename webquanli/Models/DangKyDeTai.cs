namespace webquanli.Models
{
    public class DangKyDeTai
    {
        public int Id { get; set; }
        public int SinhVienId { get; set; }
        public int DeTaiId { get; set; }
        public DateTime NgayDangKy { get; set; }

        public SinhVien SinhVien { get; set; }
        public DeTai DeTai { get; set; }
    }
}