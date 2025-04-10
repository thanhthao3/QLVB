using System.ComponentModel.DataAnnotations;

namespace QuanLyVanBang.Models
{
    public class ProcessConfirmViewModel
    {
        public int MaVanBanDen { get; set; }
        public string SoHieuVanBan { get; set; }
        public string TrichYeu { get; set; }
        public string NguoiGui { get; set; }
        public DateTime NgayGui { get; set; }
    }
}