using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyVanBang.Models;

namespace QuanLyVanBang.Controllers
{
    public class ChucVuController : Controller
    {
        private readonly QuanLyVanBangDBContext _context;

        public ChucVuController(QuanLyVanBangDBContext context)
        {
            _context = context;
        }

        // GET: ChucVu
        public async Task<IActionResult> Index()
        {
            return View(await _context.ChucVus.ToListAsync());
        }

        // GET: ChucVu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var chucVu = await _context.ChucVus
                .FirstOrDefaultAsync(m => m.MaChucVu == id);
            if (chucVu == null)
            {
                return NotFound();
            }
            return View(chucVu);
        }

        // GET: ChucVu/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChucVu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenChucVu,KiHieu,MoTa")] ChucVu chucVu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chucVu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chucVu);
        }

        // GET: ChucVu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var chucVu = await _context.ChucVus.FindAsync(id);
            if (chucVu == null)
            {
                return NotFound();
            }
            return View(chucVu);
        }

        // POST: ChucVu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaChucVu,TenChucVu,KiHieu,MoTa")] ChucVu chucVu)
        {
            if (id != chucVu.MaChucVu)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chucVu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChucVuExists(chucVu.MaChucVu))
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
            return View(chucVu);
        }

        // GET: ChucVu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var chucVu = await _context.ChucVus
                .FirstOrDefaultAsync(m => m.MaChucVu == id);
            if (chucVu == null)
            {
                return NotFound();
            }
            return View(chucVu);
        }

        // POST: ChucVu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chucVu = await _context.ChucVus.FindAsync(id);
            _context.ChucVus.Remove(chucVu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChucVuExists(int id)
        {
            return _context.ChucVus.Any(e => e.MaChucVu == id);
        }
    }
} 