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
        public int Njogo { get; set; }
        public string EquipaA { get; set; }
        public string EquipaB { get; set; }
        public string Resultado { get; set; }
        public DateTime Datainiciojogo { get; set; }
    }
}
