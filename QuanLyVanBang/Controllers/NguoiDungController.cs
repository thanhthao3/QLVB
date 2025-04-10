using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyVanBang.Models;

namespace QuanLyVanBang.Controllers
{
    public class NguoiDungController : Controller
    {
        private readonly QuanLyVanBangDBContext _context;

        public NguoiDungController(QuanLyVanBangDBContext context)
        {
            _context = context;
        }

        // GET: NguoiDung
        public async Task<IActionResult> Index()
        {
            return View(await _context.NguoiDungs.ToListAsync());
        }

        // GET: NguoiDung/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(m => m.MaNguoiDung == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }
            return View(nguoiDung);
        }

        // GET: NguoiDung/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NguoiDung/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNguoiDung,HoTen,Email,NgaySinh,GioiTinh,DiaChi,SDT,MaChucVu,MaDonVi,TenDangNhap,MatKhau,Role")] NguoiDung nguoiDung)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nguoiDung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nguoiDung);
        }

        // GET: NguoiDung/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }
            return View(nguoiDung);
        }

        // POST: NguoiDung/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaNguoiDung,HoTen,Email,NgaySinh,GioiTinh,DiaChi,SDT,MaChucVu,MaDonVi,TenDangNhap,MatKhau,Role")] NguoiDung nguoiDung)
        {
            if (id != nguoiDung.MaNguoiDung)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nguoiDung);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoiDungExists(nguoiDung.MaNguoiDung))
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
            return View(nguoiDung);
        }

        // GET: NguoiDung/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(m => m.MaNguoiDung == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }
            return View(nguoiDung);
        }

        // POST: NguoiDung/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            _context.NguoiDungs.Remove(nguoiDung);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NguoiDungExists(string id)
        {
            return _context.NguoiDungs.Any(e => e.MaNguoiDung == id);
        }
    }
} 