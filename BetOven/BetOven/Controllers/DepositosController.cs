using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BetOven.Data;
using BetOven.Models;

namespace BetOven.Controllers
{
    public class DepositosController : Controller
    {
        private readonly BetOvenDB _context;

        public DepositosController(BetOvenDB context)
        {
            _context = context;
        }

        // GET: Depositos
        public async Task<IActionResult> Index()
        {
            var betOvenDB = _context.Depositos.Include(d => d.User);
            return View(await betOvenDB.ToListAsync());
        }

        // GET: Depositos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depositos = await _context.Depositos
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.NDeposito == id);
            if (depositos == null)
            {
                return NotFound();
            }

            return View(depositos);
        }

        // GET: Depositos/Create
        public IActionResult Create()
        {
            ViewData["UserFK"] = new SelectList(_context.Utilizadores, "UserId", "Email");
            return View();
        }

        // POST: Depositos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NDeposito,Montante,Data,Formato_pagamento,Origem_deposito,UserFK")] Depositos depositos)
        {
            if (ModelState.IsValid)
            {
                _context.Add(depositos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserFK"] = new SelectList(_context.Utilizadores, "UserId", "Email", depositos.UserFK);
            return View(depositos);
        }

        // GET: Depositos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depositos = await _context.Depositos.FindAsync(id);
            if (depositos == null)
            {
                return NotFound();
            }
            ViewData["UserFK"] = new SelectList(_context.Utilizadores, "UserId", "Email", depositos.UserFK);
            return View(depositos);
        }

        // POST: Depositos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NDeposito,Montante,Data,Formato_pagamento,Origem_deposito,UserFK")] Depositos depositos)
        {
            if (id != depositos.NDeposito)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(depositos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepositosExists(depositos.NDeposito))
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
            ViewData["UserFK"] = new SelectList(_context.Utilizadores, "UserId", "Email", depositos.UserFK);
            return View(depositos);
        }

        // GET: Depositos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depositos = await _context.Depositos
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.NDeposito == id);
            if (depositos == null)
            {
                return NotFound();
            }

            return View(depositos);
        }

        // POST: Depositos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var depositos = await _context.Depositos.FindAsync(id);
            _context.Depositos.Remove(depositos);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepositosExists(int id)
        {
            return _context.Depositos.Any(e => e.NDeposito == id);
        }
    }
}
