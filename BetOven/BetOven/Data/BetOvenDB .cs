using System;
using System.Collections.Generic;
using System.Text;
using BetOven.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BetOven.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; }

        public string Fotografia { get; set; }

        public DateTime Timestamp { get; set; }
    }




    /// <summary>
    /// criação da BD do projeto
    /// </summary>
    public class BetOvenDB : IdentityDbContext<ApplicationUser>
    {
        public BetOvenDB (DbContextOptions<BetOvenDB > options)
            : base(options)
        {
        }

        //adição das tabelas à BD
        public virtual DbSet<Utilizadores> Utilizadores { get; set; }
        public virtual DbSet<Apostas> Apostas { get; set; }
        public virtual DbSet<Jogos> Jogos { get; set; }
        public virtual DbSet<Depositos> Depositos { get; set; }
    }
}
