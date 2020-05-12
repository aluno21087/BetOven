using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BetOven.Models
{
    public class Apostas
    {
        [Key]
        public int NAposta { get; set; }
        public double Quantia { get; set; }
        public DateTime Data { get; set; }
        public string Estado { get; set; }

        //FK para Users
        [ForeignKey("User")]
        public int UserFK { get; set; }
        public Users User { get; set; }
    }
}
