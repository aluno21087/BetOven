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
    //Para fazer depósitos é necessário ter conta e ser "autorizado"
    [Authorize]
    public class DepositosController : Controller
    {
        /// <summary>
        /// esta é a variável que identifica a nossa Base de Dados do projeto
        /// </summary>
        private readonly BetOvenDB _context;

        /// <summary>
        /// variável que tem o objetivo de recolher os dados de uma pessoa que está autenticada
        /// </summary>
        private readonly UserManager<ApplicationUser> _userManager;

        public DepositosController(BetOvenDB context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Depositos
        /// <summary>
        /// lista os dados dos Depósitos no ecrã
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
            ViewBag.Saldo = util.Saldo;
            var betOvenDB = _context.Depositos.Include(a => a.User).Where(a => a.UserFK == util.UserId);
            return View(await betOvenDB.ToListAsync());
        }

        // GET: Depositos/Details/5
        /// <summary>
        /// mostra os detalhes de um deposito
        /// mostra:
        /// o montante do deposito;
        /// o formato de pagamento;
        /// a origem do deposito;
        /// o utilizador que executou o deposito
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                //caso o id seja nulo retorna-se à página Index dos Depositos
                return RedirectToAction("Index", "Depositos");
            }

            var depositos = await _context.Depositos
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.NDeposito == id);

            if (depositos == null)
            {
                //caso nao existam "depositos" com detalhes retorna-se à página Index dos Depositos
                return RedirectToAction("Index", "Depositos");
            }

            //para poder ver a ViewBag do Saldo do Utilizador
            var user = await _userManager.GetUserAsync(User);
            var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
            ViewBag.Saldo = util.Saldo;

            //retorna a vista dos depositos
            return View(depositos);
        }

        // GET: Depositos/Create
        /// <summary>
        /// Um utilizador normal se quiser pode criar um depósito para aumentar o seu Saldo
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> CreateAsync()
        {
            ViewData["UserFK"] = new SelectList(_context.Utilizadores, "UserId", "Email");

            //para poder ver a ViewBag do Saldo do Utilizador
            var user = await _userManager.GetUserAsync(User);
            var util = await _context.Utilizadores.FirstOrDefaultAsync(a => a.UsernameID == user.Id);
            ViewBag.Saldo = util.Saldo;
            return View();
        }

        // POST: Depositos/Create
        /// <summary>
        /// criação de um deposito para/do utilizador
        /// ao fazer/criar um deposito, este tem de referir:
        /// o montante a depositar;
        /// um método de pagamento;
        /// a origem (banco) do depósito.
        /// </summary>
        /// <param name="depositos"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NDeposito,Montante,Data,Formato_pagamento,Origem_deposito,UserFK")] Depositos depositos)
        {
            //Caso seja possível criar um Depósito, o valor do mesmo será adicionado ao Saldo 
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var util = await _context.Utilizadores.FirstOrDefaultAsync(u => u.UsernameID == user.Id);
                depositos.UserFK = util.UserId;
                util.Saldo += depositos.Montante; //é imediatamente adicionado, o valor depositado, à carteira do utilizador
                _context.Update(util);
                _context.Add(depositos);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserFK"] = new SelectList(_context.Utilizadores, "UserId", "Email", depositos.UserFK);
            return View(depositos);
        }

        // GET: Depositos/Edit/5
        /// <summary>
        /// Um Utilizador não poderá editar o seu Depósito, visto que é moralmente incorreto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                //caso o id seja "null" retorna-se à página Index dos Depositos
                return RedirectToAction("Index", "Depositos");
            }

            var depositos = await _context.Depositos.FindAsync(id);

            if (depositos == null)
            {
                //caso o id seja "null" retorna-se à página Index dos Depositos
                return RedirectToAction("Index", "Depositos");
            }
            ViewData["UserFK"] = new SelectList(_context.Utilizadores, "UserId", "Email", depositos.UserFK);
            return View(depositos);
        }

        // POST: Depositos/Edit/5
        /// <summary>
        /// serve para editar um deposito, se necessário
        /// um utilizador não o poderá fazer, e portanto apenas um administrador
        /// tem esse "privilégio"
        /// </summary>
        /// <param name="id"></param>
        /// <param name="depositos"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Edit(int id, [Bind("NDeposito,Montante,Data,Formato_pagamento,Origem_deposito,UserFK")] Depositos depositos)
        {
            if (id != depositos.NDeposito)
            {
                //caso o id seja diferente do id do deposito retorna-se à página Index dos Depositos
                return RedirectToAction("Index", "Depositos");
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
                        //caso nao seja possivel aceder ao deposito retorna-se à página Index dos Depositos
                        return RedirectToAction("Index", "Depositos");
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
        /// <summary>
        /// Tal como Editar, um utilizador não poderá nem deverá eliminar um depósito
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrativo")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                //caso o id seja nulo retorna-se à página Index dos Depositos
                return RedirectToAction("Index", "Depositos");
            }

            var depositos = await _context.Depositos
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.NDeposito == id);

            if (depositos == null)
            {
                //caso nao seja possivel aceder aos depositos por estes serem nulos retorna-se à página Index dos Depositos
                return RedirectToAction("Index", "Depositos");
            }
            return View(depositos);
        }

        // POST: Depositos/Delete/5
        /// <summary>
        /// serve para apagar um deposito
        /// um utilizador normal não o poderá fazer
        /// por isso cabe ao Administrador executar tal ação,
        /// se necessário como é óbvio
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
