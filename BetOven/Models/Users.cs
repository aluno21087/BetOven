using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BetOven.Models
{
    public class Users
    {
        public Users()
        {
            ListaDepositos = new HashSet<Depositos>();
            ListaApostas = new HashSet<Apostas>();
        }

        [Key]
        public int UserId { get; set; }

        //nome do utilizador que está no cartão de cidadão
        [Required(ErrorMessage = "O Nome é de preenchimento obrigatório")]
        [StringLength(40, ErrorMessage = "O {0} não pode ter mais de {1} caracteres.")]
        [RegularExpression("[A-ZÓÂÍ][a-zçáéíóúàèìòùãõäëïöüâêîôûñ] + (( | d[ao](s)? | e |-|'| d')[A-ZÓÂÍ][a-zçáéíóúàèìòùãõäëïöüâêîôûñ] +){1,3}",
                            ErrorMessage = "Deve escrever entre 2 e 4 nomes, começados por uma Maiúcula, seguidos de minúsculas.")]
        public string Nome { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        //nome do utilizador dentro do sistema de apostas
        [Required(ErrorMessage = "O Nickname é de preenchimento obrigatório")]
        [StringLength(20, ErrorMessage = "O {0} não pode ter mais de {1} caracteres")]
        [RegularExpression("^[a-zA-Z0-9]$", ErrorMessage = "Pode escrever um Nickname com letra minúsculas, maiúculas e números. Não pode haver caracteres especiais.")]
        public string Nickname { get; set; }

        public string Nacionalidade { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime Datanasc { get; set; }

        public double Saldo { get; set; }

        public ICollection<Depositos> ListaDepositos { get; set; } //lista de depósitos feitos na conta do utilizador
        public ICollection<Apostas> ListaApostas { get; set; } //lista de apostas feitas pelo utilizador
    }
}
