using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyVanBang.Models
{
    [Table("DonVi")]
    public class DonVi
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Mã đơn vị")]
        public int MaDonVi { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Tên đơn vị")]
        public string TenDonVi { get; set; }

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [StringLength(10)]
        [Display(Name = "Kí hiệu")]
        public string? KiHieu { get; set; }
        public virtual ICollection<NguoiDung>? NguoiDungs { get; set; }
        public virtual ICollection<VanBanDi>? VanBanDis { get; set; }

    }
}
