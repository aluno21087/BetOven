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
        /// <summary>
        /// Identificador da Aposta
        /// </summary>
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// Quantia da Aposta
        /// </summary>
        public double Quantia { get; set; }

        /// <summary>
        /// Data em que foi feita a Aposta
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Estado atual da Aposta : "Perdida", "Ganha"
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Escolha feita pelo Utilizador sobre qual o resultado final de um Jogo
        /// </summary>
        public string Descricao { get; set; }

        /// <summary>
        /// Multiplicado a aplicar à quantia da Aposta, caso o utilizador ganhe a mesma
        /// </summary>
        public double Multiplicador { get; set; }

        //FK para Users 
        [ForeignKey("User")]
        public int UserFK { get; set; }
        public virtual Utilizadores User { get; set; }

        //FK para Jogos
        [ForeignKey("Jogo")]
        public int JogoFK { get; set; }
        public virtual Jogos Jogo { get; set; }

        public Apostas() 
        {
            //Atribuição de valor estático ao Multiplicador das Apostas
            Multiplicador = 2;
        }
    }
}
