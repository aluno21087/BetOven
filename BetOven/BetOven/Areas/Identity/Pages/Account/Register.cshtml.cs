using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using BetOven.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using BetOven.Models;

namespace BetOven.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _caminho;
        private readonly BetOvenDB _context;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment caminho,
            BetOvenDB context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _caminho = caminho;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }


        public class InputModel
        {
            [Required(ErrorMessage = "O Nome é de preenchimento obrigatório")]
            [StringLength(40, ErrorMessage = "O {0} não pode ter mais de {1} carateres.")]
            
            public string Nome { get; set; }

            [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
            [StringLength(20, ErrorMessage = "O {0} não pode ter mais de {1} caracteres")]
            public string Nickname { get; set; }

            [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            
            [StringLength(100, ErrorMessage = "A {0} deve ter entre {2} e {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "A password e aconfirmation não coincidem")]
            public string ConfirmPassword { get; set; }

            [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            [Display(Name = "Data de Nascimento")]
            [Required(ErrorMessage = "A {0} é de preenchimento obrigatório")]
            public Nullable<System.DateTime> Datanasc { get; set; }

            public string Fotografia { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(IFormFile fotoUser, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {

                string caminhoCompleto = "";
                string nomeFoto = "";
                bool haImagem = false;

                if (fotoUser == null) { nomeFoto = "../noUser.png"; }
                else
                {
                    if (fotoUser.ContentType == "image/jpeg" || fotoUser.ContentType == "image/jpg" || fotoUser.ContentType == "image/png")
                    {

                        Guid g;
                        g = Guid.NewGuid();
                        string extensao = Path.GetExtension(fotoUser.FileName).ToLower();
                        string nome = g.ToString() + extensao;


                        caminhoCompleto = Path.Combine(_caminho.WebRootPath, "Imagens/Users", nome);


                        nomeFoto = nome;


                        haImagem = true;
                    }
                    else
                    {
                        // há imagem, mas não é do tipo correto
                        nomeFoto = "../noUser.png";
                    }
                }

                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Nome = Input.Nome,
                    Fotografia = nomeFoto,
                    Timestamp = DateTime.Now
                };


                var utilizador = new Utilizadores
                {
                    Nome = Input.Nome,
                    Email = Input.Email,
                    Nickname = Input.Nickname,
                    Saldo = 0,
                    Fotografia = nomeFoto,
                    UsernameID = user.Id
                };

                _context.Add(utilizador);
                await _context.SaveChangesAsync();


                var result = await _userManager.CreateAsync(user, Input.Password);


                if (result.Succeeded)
                {


                    if (haImagem)
                    {
                        using var stream = new FileStream(caminhoCompleto, FileMode.Create);
                        await fotoUser.CopyToAsync(stream);
                    }



                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                else
                {
                    Console.WriteLine(result.Errors);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
