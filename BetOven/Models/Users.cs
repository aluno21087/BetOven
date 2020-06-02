using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BetOven.Models
{
    /// <summary>
    /// Representa os dados de um 'User'
    /// </summary>
    public class Users
    {
        public Users()
        {
            ListaDepositos = new HashSet<Depositos>();
            ListaApostas = new HashSet<Apostas>();
        }

        /// <summary>
        /// Identificador do User, será PK numa das tabelas da BD
        /// </summary>
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// Nome do Utilizador
        /// </summary>
        [Required(ErrorMessage = "O Nome é de preenchimento obrigatório")]
        [StringLength(40, ErrorMessage = "O {0} não pode ter mais de {1} caracteres.")]
        //[RegularExpression("[A-ZÓÂÍ][a-zçáéíóúàèìòùãõäëïöüâêîôûñ] + (( | d[ao](s)? | e |-|'| d')[A-ZÓÂÍ][a-zçáéíóúàèìòùãõäëïöüâêîôûñ] +){1,3}",
                            //ErrorMessage = "Deve escrever entre 2 e 4 nomes, começados por uma Maiúscula, seguidos de minúsculas.")]
        public string Nome { get; set; }

        /// <summary>
        /// E-mail do Utilizador
        /// </summary>
        [EmailAddress]
        [Required(ErrorMessage = "O Email é de preenchimento obrigatório")]
        public string Email { get; set; }


        /// <summary>
        /// Nome do utilizador dentro do sistema de apostas
        /// </summary>
        [Required(ErrorMessage = "O Nickname é de preenchimento obrigatório")]
        [StringLength(20, ErrorMessage = "O {0} não pode ter mais de {1} caracteres")]
        //[RegularExpression("^[a-zA-Z0-9]$", ErrorMessage = "Pode escrever um Nickname com letra minúsculas, maiúculas e números. Não pode haver caracteres especiais.")]
        public string Nickname { get; set; }

        /// <summary>
        /// Nacionalidade do Utilizador
        /// </summary>
        public string Nacionalidade { get; set; }
        
        /// <summary>
        /// Data de nascimento do Utilizador
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Display(Name = "Data de Nascimento")]
        [Required]
        public DateTime Datanasc { get; set; }

        public double Saldo { get; set; }

        public string Fotografia { get; set; }

        public virtual ICollection<Depositos> ListaDepositos { get; set; } //lista de depósitos feitos na conta do utilizador
        public virtual ICollection<Apostas> ListaApostas { get; set; } //lista de apostas feitas pelo utilizador
    }
}
