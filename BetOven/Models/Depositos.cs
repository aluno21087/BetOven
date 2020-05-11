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
        public int nDeposito { get; set; }
        public double montante { get; set; }
        public DateTime data { get; set; }
        public string formato_pagamento { get; set; }
        public string origem_deposito { get; set; }

        [ForeignKey("User")]
        public int UserFK { get; set; }
        public Users User { get; set; }
    }
}
