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
    //Authorização refere-se como um processo que determina o que um utilizador é capaz de fazer  
    //Neste caso específico, apenas Utilizadores com conta podem aceder às Apostas
    [Authorize]
    public class ApostasController : Controller
    {

        /// <summary>
        /// esta é a variável que identifica a nossa Base de Dados do projeto
        /// </summary>
        private readonly BetOvenDB _context;

        /// <summary>
        /// variável que tem o objetivo de recolher os dados de uma pessoa que está autenticada
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        public ApostasController(BetOvenDB context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Apostas
        /// <summary>
        /// lista os dados das Apostas no ecrã
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
            ViewBag.Saldo = util.Saldo;
            var context = _context.Apostas.Include(a => a.Jogo).Include(a => a.User).Where(a => a.UserFK == util.UserId);
            return View(await context.ToListAsync());
        }

        // GET: Apostas/Details/5
        /// <summary>
        /// mostra os detalhes de uma Aposta
        /// mostra:
        /// a quantia apostada;
        /// a data em que foi executada a aposta;
        /// o estado da aposta;
        /// a equipa em que apostou;
        /// o multiplicador;
        /// o utilizador;
        /// o ID do jogo.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                //caso o id seja nulo retorna-se à página Index das Apostas
                return RedirectToAction("Index", "Apostas");
            }

            var apostas = await _context.Apostas
                .Include(a => a.Jogo)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (apostas == null)
            {
                //caso não exista uma aposta para observar os detalhes retorna-se à página Index das Apostas
                return RedirectToAction("Index", "Apostas");
            }

            //para poder ver a ViewBag do Saldo do Utilizador
            var user = await _userManager.GetUserAsync(User);
            var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
            ViewBag.Saldo = util.Saldo;

            //retorno da View das Apostas
            return View(apostas);
        }

        // GET: Apostas/Create
        /// <summary>
        /// para poder criar uma aposta, neste caso é o GET da criação
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// criação de apostas por parte do utilizador
        /// ao criar uma aposta, este tem de:
        /// referir uma quantia;
        /// selecionar o resultado da aposta;
        /// escolher qual o jogo em que fará a aposta.
        /// </summary>
        /// <param name="apostas"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Quantia,Estado,Descricao,Multiplicador,UserFK,JogoFK")] Apostas apostas)
        {
            //caso o modelo seja válido e se faça uma aposta, retira-se o valor da aposta ao Saldo do utilizador
            if (ModelState.IsValid)
            {
                apostas.Data = DateTime.Now; // a data das apostas é definida automaticamente
                var user = await _userManager.GetUserAsync(User);
                var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
                util.Saldo -= apostas.Quantia; // é retirado o valor da aposta ao saldo do utilizador
                apostas.UserFK = util.UserId;
                _context.Update(util); // o utilizador é atualizado na base de dados
                _context.Add(apostas); // é adicionada uma aposta à base de dados
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JogoFK"] = new SelectList(_context.Set<Jogos>(), "Njogo", "Njogo", apostas.JogoFK);
            ViewData["UserFK"] = new SelectList(_context.Set<Utilizadores>(), "UserId", "Email", apostas.UserFK);
            return View(apostas);
        }

        // GET: Apostas/Edit/5
        /// <summary>
        /// apenas um Administrador poderá editar uma aposta
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                //caso o id seja "null" retorna-se à página Index das Apostas
                return RedirectToAction("Index", "Apostas");
            }

            var apostas = await _context.Apostas.FindAsync(id);
            if (apostas == null)
            {
                //caso não exista uma aposta para "editar" retorna-se à página Index das Apostas
                return RedirectToAction("Index", "Apostas");
            }

            ViewData["JogoFK"] = new SelectList(_context.Set<Jogos>(), "Njogo", "Njogo", apostas.JogoFK);
            ViewData["UserFK"] = new SelectList(_context.Set<Utilizadores>(), "UserId", "Email", apostas.UserFK);
            return View(apostas);

        }

        // POST: Apostas/Edit/5
        /// <summary>
        /// edita os dados de uma Aposta
        /// caso ocorra alguma gralha ou incidente, 
        /// um Administrador poderá editar a Aposta
        /// </summary>
        /// <param name="id"></param>
        /// <param name="apostas"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Quantia,Data,Estado,Descricao,Multiplicador,UserFK,JogoFK")] Apostas apostas)
        {
            if (id != apostas.ID)
            {
                //caso o id seja diferente do id da Aposta retorna-se à página Index das Apostas
                return RedirectToAction("Index", "Apostas");
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
                        //caso a aposta não "exista" retorna-se à página Index das Apostas
                        return RedirectToAction("Index", "Apostas");
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
        /// <summary>
        /// um utilizador normal não poderá eliminar apostas, apenas um Administrador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                //caso o id seja nulo, retorna-se à página Index das Apostas
                return RedirectToAction("Index", "Apostas");
            }

            var apostas = await _context.Apostas
                .Include(a => a.Jogo)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (apostas == null)
            {
                //caso "apostas" seja nulo, retorna-se à página Index das Apostas
                return RedirectToAction("Index", "Apostas");
            }

            return View(apostas);
        }

        // POST: Apostas/Delete/5
        /// <summary>
        /// serve para apagar uma aposta
        /// tem o privilégio de apenas um Administrador poder apagar 
        /// não há segurança na eventualidade de um utilizador poder apagar apostas
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
