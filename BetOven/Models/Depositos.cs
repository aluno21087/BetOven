using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BetOven.Models
{
    public class Depositos
    {
        [Key]
        public int NDeposito { get; set; }
        public double Montante { get; set; }
        public DateTime Data { get; set; }
        public string Formato_pagamento { get; set; }
        public string Origem_deposito { get; set; }

        //FK para Users
        [ForeignKey("User")]
        public int UserFK { get; set; }
        public Users User { get; set; }
    }
}
