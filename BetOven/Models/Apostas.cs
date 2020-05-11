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
        public int nAposta { get; set; }
        public double quantia { get; set; }
        public DateTime data { get; set; }
        public string estado { get; set; }

        //FK para Users
        [ForeignKey("User")]
        public int UserFK { get; set; }
        public Users User { get; set; }
    }
}
