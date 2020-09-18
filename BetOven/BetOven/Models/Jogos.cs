using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BetOven.Models
{
    /// <summary>
    /// Representa os dados de um 'Jogo'
    /// </summary>
    public class Jogos
    {
        public Jogos()
        {
            ListaApostas = new HashSet<Apostas>();
        }


        /// <summary>
        /// Identificador do Jogo, será PK na tabela Jogos
        /// </summary>
        [Key]
        [Display(Name = "Número do Jogo")]
        public int Njogo { get; set; }

        /// <summary>
        /// Nome da primeira equipa
        /// </summary>
        [Display(Name = "Equipa A")]
        public string EquipaA { get; set; }

        /// <summary>
        /// Fotografia da primeira equipa
        /// </summary>
        public string FotoA { get; set; }

        /// <summary>
        /// Nome da segunda equipa
        /// </summary>
        [Display(Name = "Equipa B")]
        public string EquipaB { get; set; }

        /// <summary>
        /// Fotografia da segunda equipa
        /// </summary>
        public string FotoB { get; set; }

        /// <summary>
        /// Resultado do Jogo
        /// </summary>
        public string Resultado { get; set; }

        /// <summary>
        /// Data em que se inicia o Jogo
        /// </summary>
        [DataType(DataType.Date)]
        [Display(Name = "Data de início")]
        public DateTime Datainiciojogo { get; set; }

        public virtual ICollection<Apostas> ListaApostas { get; set; } //lista de apostas feitas num jogo
    }
}
