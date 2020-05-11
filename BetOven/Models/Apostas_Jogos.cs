using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BetOven.Models
{
    public class Apostas_Jogos
    {
        [Key]
        //FK para Apostas
        [ForeignKey("Aposta")]
        public int ApostaFK { get; set; }
        public Apostas Aposta { get; set; }

        //FK para Jogos
        [ForeignKey("Jogo")]
        public int JogoFK { get; set; }
        public Jogos Jogo { get; set; }

        public string descricao { get; set; }
        public double multiplicador { get; set; }
    }
}
