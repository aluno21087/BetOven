using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BetOven.Models
{
    /// <summary>
    /// Representa os dados de um 'Depósito'
    /// </summary>
    public class Depositos
    {
        /// <summary>
        /// Identificador do Depósito, será PK na tabela Depositos
        /// </summary>
        [Key]
        [Display(Name = "Número do Depósito")]
        public int NDeposito { get; set; }

        /// <summary>
        /// Montante a ser depositado
        /// </summary>
        //[RegularExpression("[0-9]+[€]")]
        public double Montante { get; set; }

        /// <summary>
        /// Data em que o depósito foi efetuado
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }

        /// <summary>
        /// Formato em que o pagamento do deposito foi efetuado
        /// Exs.: Paypal, Multibanco, Cartão Virtual (MB WAY), Cartão de Crédito, etc...
        /// </summary>
        [Display(Name = "Formato de Pagamento")]
        public string Formato_pagamento { get; set; }

        /// <summary>
        /// Hipótese de o depósito ter sido feito pelo utilizador ou pelo sistema
        /// </summary>
        [Display(Name = "Origem do Depósito")]
        public string Origem_deposito { get; set; }

        //FK para Users
        [ForeignKey("User")]
        [Display(Name = "Utilizador")]
        public int UserFK { get; set; }
        public virtual Utilizadores User { get; set; }
    }
}
