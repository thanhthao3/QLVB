using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyVanBang.Models;

namespace QuanLyVanBang.Controllers
{
    public class LoaiVanBanController : Controller
    {
        private readonly QuanLyVanBangDBContext _context;

        public LoaiVanBanController(QuanLyVanBangDBContext context)
        {
            _context = context;
        }

        // GET: LoaiVanBan
        public async Task<IActionResult> Index()
        {
            return View(await _context.LoaiVanBans.ToListAsync());
        }

        // GET: LoaiVanBan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var loaiVanBan = await _context.LoaiVanBans
                .FirstOrDefaultAsync(m => m.MaLoaiVanBan == id);
            if (loaiVanBan == null)
            {
                return NotFound();
            }
            return View(loaiVanBan);
        }

        // GET: LoaiVanBan/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoaiVanBan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenLoai,KiHieu,MoTa")] LoaiVanBan loaiVanBan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaiVanBan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiVanBan);
        }

        // GET: LoaiVanBan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var loaiVanBan = await _context.LoaiVanBans.FindAsync(id);
            if (loaiVanBan == null)
            {
                return NotFound();
            }
            return View(loaiVanBan);
        }

        // POST: LoaiVanBan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLoaiVanBan,TenLoai,KiHieu,MoTa")] LoaiVanBan loaiVanBan)
        {
            if (id != loaiVanBan.MaLoaiVanBan)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiVanBan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiVanBanExists(loaiVanBan.MaLoaiVanBan))
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
            return View(loaiVanBan);
        }

        // GET: LoaiVanBan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var loaiVanBan = await _context.LoaiVanBans
                .FirstOrDefaultAsync(m => m.MaLoaiVanBan == id);
            if (loaiVanBan == null)
            {
                return NotFound();
            }
            return View(loaiVanBan);
        }

        // POST: LoaiVanBan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loaiVanBan = await _context.LoaiVanBans.FindAsync(id);
            _context.LoaiVanBans.Remove(loaiVanBan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaiVanBanExists(int id)
        {
            return _context.LoaiVanBans.Any(e => e.MaLoaiVanBan == id);
        }
    }
}
