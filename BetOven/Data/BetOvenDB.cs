using BetOven.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BetOven.Data
{
    public class BetOvenDB : DbContext
    {
        /// <summary>
        /// Construtor da classe que serve para liga esta classe à Base de Dados
        /// </summary>
        /// <param name="options"></param>
        public BetOvenDB(DbContextOptions<BetOvenDB> options) : base(options) { }

        //adição das tabelas à BD
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Apostas> Apostas { get; set; }
        public virtual DbSet<Jogos> Jogos { get; set; }
        public virtual DbSet<Depositos> Depositos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apostas>()
                .HasKey(k => new { k.UserFK, k.JogoFK });
        }
    }
}
