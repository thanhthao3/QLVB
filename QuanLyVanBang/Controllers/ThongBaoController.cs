using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyVanBang.Models;

namespace QuanLyVanBang.Controllers
{
    public class ThongBaoController : BaseController
    {
        private readonly QuanLyVanBangDBContext _context;

        public ThongBaoController(QuanLyVanBangDBContext context)
        {
            _context = context;
        }

        // GET: ThongBao
        public async Task<IActionResult> Index()
        {
            var currentUserId = CurrentUserID;
            var thongBaos = await _context.ThongBaos
                .Include(t => t.NguoiDungNhan)
                .Where(t => t.NguoiNhan == currentUserId)
                .OrderByDescending(t => t.NgayTao)
                .ToListAsync();

            return View(thongBaos);
        }

        // POST: ThongBao/DanhDauDaXem/5
        [HttpPost]
        public async Task<IActionResult> DanhDauDaXem(int id)
        {
            var currentUserId = CurrentUserID;
            var thongBao = await _context.ThongBaos
                .FirstOrDefaultAsync(t => t.MaThongBao == id && t.NguoiNhan == currentUserId);

            if (thongBao == null)
            {
                return NotFound();
            }

            thongBao.DaXem = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: ThongBao/DanhDauTatCaDaXem
        [HttpPost]
        public async Task<IActionResult> DanhDauTatCaDaXem()
        {
            var currentUserId = CurrentUserID;
            var thongBaoChuaXem = await _context.ThongBaos
                .Where(t => t.NguoiNhan == currentUserId && !t.DaXem)
                .ToListAsync();

            foreach (var thongBao in thongBaoChuaXem)
            {
                thongBao.DaXem = true;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // GET: ThongBao/LaySoThongBaoChuaXem
        [HttpGet]
        public async Task<IActionResult> LaySoThongBaoChuaXem()
        {
            var currentUserId = CurrentUserID;
            var soThongBaoChuaXem = await _context.ThongBaos
                .CountAsync(t => t.NguoiNhan == currentUserId && !t.DaXem);

            return Json(new { count = soThongBaoChuaXem });
        }

        // GET: ThongBao/LayThongBaoMoiNhat
        [HttpGet]
        public async Task<IActionResult> LayThongBaoMoiNhat()
        {
            var currentUserId = CurrentUserID;
            var thongBaoMoiNhat = await _context.ThongBaos
                .Include(t => t.NguoiDungNhan)
                .Where(t => t.NguoiNhan == currentUserId && !t.DaXem)
                .OrderByDescending(t => t.NgayTao)
                .Take(5)
                .Select(t => new
                {
                    t.MaThongBao,
                    t.NoiDung,
                    t.NgayTao,
                    t.LoaiThongBao,
                    t.DuongDanVanBan
                })
                .ToListAsync();

            return Json(thongBaoMoiNhat);
        }

        // POST: ThongBao/XoaThongBao/5
        [HttpPost]
        public async Task<IActionResult> XoaThongBao(int id)
        {
            var currentUserId = CurrentUserID;
            var thongBao = await _context.ThongBaos
                .FirstOrDefaultAsync(t => t.MaThongBao == id && t.NguoiNhan == currentUserId);

            if (thongBao == null)
            {
                return NotFound();
            }

            _context.ThongBaos.Remove(thongBao);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: ThongBao/XoaTatCaThongBao
        [HttpPost]
        public async Task<IActionResult> XoaTatCaThongBao()
        {
            var currentUserId = CurrentUserID;
            var thongBaos = await _context.ThongBaos
                .Where(t => t.NguoiNhan == currentUserId)
                .ToListAsync();

            _context.ThongBaos.RemoveRange(thongBaos);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
} 