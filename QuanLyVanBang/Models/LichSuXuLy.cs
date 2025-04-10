using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace QuanLyVanBang.Models
{
    [Table("LichSuXuLy")]
    public class LichSuXuLy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã lịch sử xử lý")]
        public int MaLichSuXuLy { get; set; }

        [Required]
        [Display(Name = "Mã văn bản đi")]
        public int MaVanBanDi { get; set; }

        [Required]
        [Display(Name = "Ngày duyệt")]
        public DateTime NgayDuyet { get; set; }

        [Required]
        [Display(Name = "Mã người duyệt")]
        public string MaNguoiDuyet { get; set; }

        [Required]
        [Display(Name = "Bước")]
        public int Buoc { get; set; }

        [Display(Name = "Nhận xét")]
        public string? NhanXet { get; set; }

        [Required]
        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; }

        [Required]
        [Display(Name = "Mã văn bản đến")]
        public int MaVanBanDen { get; set; }

        [ForeignKey("MaVanBanDen")]
        public virtual VanBanDen? VanBanDen { get; set; }

        // Navigation Properties
        [ForeignKey("MaVanBanDi")]
        public virtual VanBanDi? VanBanDi { get; set; }

        [ForeignKey("MaNguoiDuyet")]
        public virtual NguoiDung? NguoiDuyet { get; set; }
    }
}
