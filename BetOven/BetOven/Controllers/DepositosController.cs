using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BetOven.Data;
using BetOven.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BetOven.Controllers
{
    //Apenas Utilizadores com conta podem aceder aqui às Apostas
    [Authorize]
    public class DepositosController : Controller
    {
        private readonly BetOvenDB _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepositosController(BetOvenDB context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Depositos
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
            ViewBag.Saldo = util.Saldo;
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

            var user = await _userManager.GetUserAsync(User);
            var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
            ViewBag.Saldo = util.Saldo;
            return View(depositos);
        }

        // GET: Depositos/Create
        // Um utilizador normal se quiser pode criar um depósito para aumentar o seu Saldo
        public async Task<IActionResult> CreateAsync()
        {
            ViewData["UserFK"] = new SelectList(_context.Utilizadores, "UserId", "Email");
            var user = await _userManager.GetUserAsync(User);
            var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
            ViewBag.Saldo = util.Saldo;
            return View();
        }

        // POST: Depositos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NDeposito,Montante,Data,Formato_pagamento,Origem_deposito,UserFK")] Depositos depositos)
        {
            //Caso seja possível criar um Depósito, o valor do mesmo será adicionado ao Saldo 
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var util = await _context.Utilizadores.FirstOrDefaultAsync(u => u.UsernameID == user.Id);
                util.Saldo += depositos.Montante;
                _context.Update(util);
                _context.Add(depositos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserFK"] = new SelectList(_context.Utilizadores, "UserId", "Email", depositos.UserFK);
            return View(depositos);
        }

        // GET: Depositos/Edit/5
        // Um Utilizador não poderá editar o seu Depósito, visto que é moralmente incorreto
        [Authorize(Roles = "Administrativo")]
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
        [Authorize(Roles = "Administrativo")]
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
        // Tal como Editar, um utilizador não poderá nem deverá eliminar um depósito
        [Authorize(Roles = "Administrativo")]
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
        [Authorize(Roles = "Administrativo")]
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
