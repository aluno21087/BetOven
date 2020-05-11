using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BetOven.Models
{
    public class Jogos
    {
        [Key]
        public int njogo { get; set; }
        public string equipaA { get; set; }
        public string equipaB { get; set; }
        public string Resultado { get; set; }
        public DateTime datainiciojogo { get; set; }
    }
}
