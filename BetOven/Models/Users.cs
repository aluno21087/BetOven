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
        public string nome { get; set; }
        public string email { get; set; }
        public string nickname { get; set; }
        public string nacionalidade { get; set; }
        public DateTime datanasc { get; set; }
        public double saldo { get; set; }

        public ICollection<Depositos> ListaDepositos { get; set; }
        public ICollection<Apostas> ListaApostas { get; set; }
    }
}
