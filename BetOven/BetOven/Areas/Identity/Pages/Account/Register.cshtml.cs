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

namespace BetOven.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// variável que contém os dados do 'ambiente' do servidor. 
        /// Em particular, onde estão os ficheiros guardados, no disco rígido do servidor
        /// </summary>
        private readonly IWebHostEnvironment _caminho;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IWebHostEnvironment caminho)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _caminho = caminho;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class DateMinimumAgeAttribute : ValidationAttribute
        {
            public DateMinimumAgeAttribute(int minimumAge)
            {
                MinimumAge = minimumAge;
                ErrorMessage = "{0} must be someone at least {1} years of age";
            }

            public override bool IsValid(object value)
            {
                DateTime date;
                if ((value != null && DateTime.TryParse(value.ToString(), out date)))
                {
                    return date.AddYears(MinimumAge) < DateTime.Now;
                }

                return false;
            }

            public override string FormatErrorMessage(string name)
            {
                return string.Format(ErrorMessageString, name, MinimumAge);
            }

            public int MinimumAge { get; }
        }

        public class InputModel
        {
            [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
            [StringLength(40, ErrorMessage = "O {0} não pode ter mais de {1} caracteres.")]
            public string Nome { get; set; }

            [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
            [StringLength(20, ErrorMessage = "O {0} não pode ter mais de {1} caracteres")]
            public string Nickname { get; set; }

            [Required(ErrorMessage = "O {0} é de preenchimento obrigatório")]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [DateMinimumAge(18, ErrorMessage = "Tens de ter mais de {0} anos.")]
            [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
            [Display(Name = "Data de Nascimento")]
            [Required]
            public Nullable<System.DateTime> Datanasc { get; set; }

            public string Fotografia { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }




        public async Task<IActionResult> OnPostAsync(IFormFile fotoUser, string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();   //para registar de outras formas (Google, Facebook, etc)
            if (ModelState.IsValid)
            {

                // variáveis auxiliares
                string caminhoCompleto = "";
                string nomeFoto = "";
                bool haImagem = false;

                if (fotoUser == null) { nomeFoto = "noUser.png"; }
                else
                {
                    if (fotoUser.ContentType == "image/jpeg" || fotoUser.ContentType == "image/jpg" || fotoUser.ContentType == "image/png")
                    {
                        // o ficheiro é uma imagem válida
                        // preparar a imagem para ser guardada no disco rígido
                        // e o seu nome associado ao Utilizador
                        Guid g;
                        g = Guid.NewGuid();
                        string extensao = Path.GetExtension(fotoUser.FileName).ToLower();
                        string nome = g.ToString() + extensao;

                        // onde guardar o ficheiro
                        caminhoCompleto = Path.Combine(_caminho.WebRootPath, "Imagens\\Users", nome);

                        // associar o nome do ficheiro ao Utilizador 
                        nomeFoto = nome;

                        // assinalar que existe imagem e é preciso guardá-la no disco rígido
                        haImagem = true;
                    }
                    else
                    {
                        // há imagem, mas não é do tipo correto
                        nomeFoto = "noUser.png";
                    }
                }

                //criação de um novo utilizador
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    Nome = Input.Nome,
                    Fotografia = nomeFoto,
                    Timestamp = DateTime.Now
                };

                // vai escrever esses dados na Base de Dados
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    //se tive sucesso, vou guardar a imagem no disco rígido do servidor
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
