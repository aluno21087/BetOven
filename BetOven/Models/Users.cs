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
        public string nome { get; set; }
        [EmailAddress]
        public string email { get; set; }
        //nome do utilizador dentro do sistema de apostas
        public string nickname { get; set; }
        public string nacionalidade { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime datanasc { get; set; }
        public double saldo { get; set; }

        public ICollection<Depositos> ListaDepositos { get; set; } //lista de depósitos feitos na conta do utilizador
        public ICollection<Apostas> ListaApostas { get; set; } //lista de apostas feitas pelo utilizador
    }
}
