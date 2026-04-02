namespace webquanli.Models
{

    public class DeTai
    {

        public int Id { get; set; }

        public string TenDeTai { get; set; }

        public string MoTa { get; set; }

        public int GiangVienId { get; set; }

        public GiangVien GiangVien { get; set; }

    }

}