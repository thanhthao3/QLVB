using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyVanBang.Models;

namespace QuanLyVanBang.Controllers
{
    public class LichSuXuLyController : BaseController
    {
        private readonly QuanLyVanBangDBContext _context;

        public LichSuXuLyController(QuanLyVanBangDBContext context)
        {
            _context = context;
        }

        // GET: LichSuXuLy
        public async Task<IActionResult> Index()
        {
            // Lấy ID người dùng đăng nhập
            var currentUserId = CurrentUserID;

            // Lấy danh sách lịch sử xử lý mà người dùng là người duyệt
            var lichSuXuLys = await _context.LichSuXuLys
                .Where(l => l.MaNguoiDuyet == currentUserId)
                .Include(l => l.VanBanDi)
                    .ThenInclude(vbd => vbd.NguoiGui)
                .Include(l => l.VanBanDi)
                    .ThenInclude(vbd => vbd.LoaiVanBan)
                .Include(l => l.VanBanDen)
                .Include(l => l.NguoiDuyet)
                .OrderByDescending(l => l.NgayDuyet)
                .ToListAsync();

            return View(lichSuXuLys);
        }

        // GET: LichSuXuLy/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var lichSuXuLy = await _context.LichSuXuLys
                .FirstOrDefaultAsync(m => m.MaLichSuXuLy == id);
            if (lichSuXuLy == null)
            {
                return NotFound();
            }
            return View(lichSuXuLy);
        }

        // GET: LichSuXuLy/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LichSuXuLy/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaVanBanDi,NgayDuyet,MaNguoiDuyet,Buoc,NhanXet,TrangThai")] LichSuXuLy lichSuXuLy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lichSuXuLy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lichSuXuLy);
        }

        // GET: LichSuXuLy/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var lichSuXuLy = await _context.LichSuXuLys.FindAsync(id);
            if (lichSuXuLy == null)
            {
                return NotFound();
            }
            return View(lichSuXuLy);
        }

        // POST: LichSuXuLy/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLichSuXuLy,MaVanBanDi,NgayDuyet,MaNguoiDuyet,Buoc,NhanXet,TrangThai")] LichSuXuLy lichSuXuLy)
        {
            if (id != lichSuXuLy.MaLichSuXuLy)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichSuXuLy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichSuXuLyExists(lichSuXuLy.MaLichSuXuLy))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(lichSuXuLy);
        }

        // GET: LichSuXuLy/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var lichSuXuLy = await _context.LichSuXuLys
                .FirstOrDefaultAsync(m => m.MaLichSuXuLy == id);
            if (lichSuXuLy == null)
            {
                return NotFound();
            }
            return View(lichSuXuLy);
        }

        // POST: LichSuXuLy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lichSuXuLy = await _context.LichSuXuLys.FindAsync(id);
            _context.LichSuXuLys.Remove(lichSuXuLy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LichSuXuLyExists(int id)
        {
            return _context.LichSuXuLys.Any(e => e.MaLichSuXuLy == id);
        }
    }
} 