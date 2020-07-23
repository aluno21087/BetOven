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
    public class ApostasController : Controller
    {
        private readonly BetOvenDB _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApostasController(BetOvenDB context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Apostas
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
            ViewBag.Saldo = util.Saldo;
            var context = _context.Apostas.Include(a => a.Jogo).Include(a => a.User).Where(a => a.UserFK == util.UserId);
            return View(await context.ToListAsync());
        }

        // GET: Apostas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //caso o ID seja nulo retorna uma página de NotFound
            if (id == null)
            {
                return NotFound();
            }

            var apostas = await _context.Apostas
                .Include(a => a.Jogo)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (apostas == null)
            {
                return NotFound();
            }

            //para poder ver a ViewBag do Saldo do Utilizador
            var user = await _userManager.GetUserAsync(User);
            var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
            ViewBag.Saldo = util.Saldo;

            //retorno da View das Apostas
            return View(apostas);
        }

        // GET: Apostas/Create
        public async Task<IActionResult> CreateAsync()
        {
            ViewData["JogoFK"] = new SelectList(_context.Set<Jogos>(), "Njogo", "Njogo");
            ViewData["UserFK"] = new SelectList(_context.Set<Utilizadores>(), "UserId", "Email");

            //para poder ver a ViewBag do Saldo do Utilizador
            var user = await _userManager.GetUserAsync(User);
            var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
            ViewBag.Saldo = util.Saldo;
            return View();
        }

        // POST: Apostas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Quantia,Estado,Descricao,Multiplicador,UserFK,JogoFK")] Apostas apostas)
        {
            //caso o modelo seja válido e se faça uma aposta, retira-se o valor da aposta ao Saldo do utilizador
            if (ModelState.IsValid)
            {
                apostas.Data = DateTime.Now;
                var user = await _userManager.GetUserAsync(User);
                var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
                util.Saldo -= apostas.Quantia;
                apostas.UserFK = util.UserId;
                _context.Update(util);
                _context.Add(apostas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JogoFK"] = new SelectList(_context.Set<Jogos>(), "Njogo", "Njogo", apostas.JogoFK);
            ViewData["UserFK"] = new SelectList(_context.Set<Utilizadores>(), "UserId", "Email", apostas.UserFK);
            return View(apostas);
        }

        // GET: Apostas/Edit/5
        //Apenas um Administrador poderá, ou não, editar uma aposta
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apostas = await _context.Apostas.FindAsync(id);
            if (apostas == null)
            {
                return NotFound();
            }

            ViewData["JogoFK"] = new SelectList(_context.Set<Jogos>(), "Njogo", "Njogo", apostas.JogoFK);
            ViewData["UserFK"] = new SelectList(_context.Set<Utilizadores>(), "UserId", "Email", apostas.UserFK);
            return View(apostas);

        }

        // POST: Apostas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Quantia,Data,Estado,Descricao,Multiplicador,UserFK,JogoFK")] Apostas apostas)
        {
            if (id != apostas.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(apostas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApostasExists(apostas.ID))
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

            ViewData["JogoFK"] = new SelectList(_context.Set<Jogos>(), "Njogo", "Njogo", apostas.JogoFK);
            ViewData["UserFK"] = new SelectList(_context.Set<Utilizadores>(), "UserId", "Email", apostas.UserFK);
            return View(apostas);
        }

        // GET: Apostas/Delete/5
        //Um utilizador normal não poderá eliminar apostas, apenas um Administrador
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apostas = await _context.Apostas
                .Include(a => a.Jogo)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (apostas == null)
            {
                return NotFound();
            }

            return View(apostas);
        }

        // POST: Apostas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apostas = await _context.Apostas.FindAsync(id);
            _context.Apostas.Remove(apostas);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApostasExists(int id)
        {
            return _context.Apostas.Any(e => e.ID == id);
        }
    }
}
