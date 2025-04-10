using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyVanBang.Models
{
    [Table("VanBanDen")]
    public class VanBanDen
    {
        [Key]
        [Display(Name = "Mã văn bản đến")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaVanBanDen { get; set; }

        [Required]
        [Display(Name = "Mã văn bản đi")]
        public int MaVanBanDi { get; set; }

        [Required]
        [Display(Name = "Người nhận")]
        [StringLength(255)]
        public string NguoiNhan { get; set; }
        
        public DateTime NgayNhan { get; set; }
        [Required]
        [Display(Name = "Trạng thái")]
        public int TrangThai { get; set; }

        // Navigation Properties
        [ForeignKey("MaVanBanDi")]
        public virtual VanBanDi? VanBanDi { get; set; }

        [ForeignKey("NguoiNhan")]
        public virtual NguoiDung? NguoiDung { get; set; }

        public virtual ICollection<LichSuXuLy>? LichSuXuLy { get; set; }
    }
}
