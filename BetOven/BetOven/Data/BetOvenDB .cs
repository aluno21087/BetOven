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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // insert DB seed
            modelBuilder.Entity<Utilizadores>().HasData(
               new Utilizadores { UserId = 1, Nome = "Primeiro Cliente", Email = "primeiro.cliente@gmail.com", Nickname = "primeiro", Fotografia = "noUser.png" },
               new Utilizadores { UserId = 2, Nome = "Segundo Cliente", Email = "segundo.cliente@gmail.com", Nickname = "segundo", Fotografia = "noUser.png" },
               new Utilizadores { UserId = 3, Nome = "Terceiro Cliente", Email = "terceiro.cliente@gmail.com", Nickname = "terceiro", Fotografia = "noUser.png" },
               new Utilizadores { UserId = 4, Nome = "Quarto Cliente", Email = "quarto.cliente@gmail.com", Nickname = "quarto", Fotografia = "noUser.png" },
               new Utilizadores { UserId = 5, Nome = "Quinto Cliente", Email = "quinto.cliente@gmail.com", Nickname = "quinto", Fotografia = "noUser.png" },
               new Utilizadores { UserId = 6, Nome = "Sexto Cliente", Email = "sexto.cliente@gmail.com", Nickname = "sexto", Fotografia = "noUser.png" }
                );

            modelBuilder.Entity<Apostas>().HasData(
               new Apostas { ID = 1, Quantia = 10, Data = DateTime.Now, Multiplicador = 2, UserFK = 1, JogoFK = 1 },
               new Apostas { ID = 2, Quantia = 10, Data = DateTime.Now, Multiplicador = 2, UserFK = 2, JogoFK = 2 },
               new Apostas { ID = 3, Quantia = 10, Data = DateTime.Now, Multiplicador = 2, UserFK = 3, JogoFK = 3 },
               new Apostas { ID = 4, Quantia = 10, Data = DateTime.Now, Multiplicador = 2, UserFK = 4, JogoFK = 1 },
               new Apostas { ID = 5, Quantia = 10, Data = DateTime.Now, Multiplicador = 2, UserFK = 5, JogoFK = 2 },
               new Apostas { ID = 6, Quantia = 10, Data = DateTime.Now, Multiplicador = 2, UserFK = 6, JogoFK = 3 },
               new Apostas { ID = 7, Quantia = 10, Data = DateTime.Now, Multiplicador = 2, UserFK = 7, JogoFK = 3 },
               new Apostas { ID = 8, Quantia = 10, Data = DateTime.Now, Multiplicador = 2, UserFK = 8, JogoFK = 1 },
               new Apostas { ID = 9, Quantia = 10, Data = DateTime.Now, Multiplicador = 2, UserFK = 9, JogoFK = 2 },
               new Apostas { ID = 10, Quantia = 10, Data = DateTime.Now, Multiplicador = 2, UserFK = 10, JogoFK = 3 }
               );

            modelBuilder.Entity<Jogos>().HasData(
               new Jogos { Njogo = 1, EquipaA = "Benfica", EquipaB = "Porto", Datainiciojogo = DateTime.Now, FotoA = "noTeam.jpg", FotoB = "noTeam.jpg" },
               new Jogos { Njogo = 2, EquipaA = "Manchester United", EquipaB = "Manchester City", Datainiciojogo = DateTime.Now, FotoA = "noTeam.jpg", FotoB = "noTeam.jpg" },
               new Jogos { Njogo = 3, EquipaA = "Real Madrid", EquipaB = "Barcelona", Datainiciojogo = DateTime.Now, FotoA = "noTeam.jpg", FotoB = "noTeam.jpg" }
               );

            modelBuilder.Entity<Depositos>().HasData(
               new Depositos { NDeposito = 1, Montante = 100, Formato_pagamento = "Master Card", Origem_deposito = "Santander Totta", UserFK = 1 },
               new Depositos { NDeposito = 2, Montante = 100, Formato_pagamento = "Master Card", Origem_deposito = "Santander Totta", UserFK = 2 },
               new Depositos { NDeposito = 3, Montante = 100, Formato_pagamento = "Master Card", Origem_deposito = "Santander Totta", UserFK = 3 },
               new Depositos { NDeposito = 4, Montante = 100, Formato_pagamento = "Master Card", Origem_deposito = "Santander Totta", UserFK = 4 },
               new Depositos { NDeposito = 5, Montante = 100, Formato_pagamento = "Master Card", Origem_deposito = "Santander Totta", UserFK = 5 },
               new Depositos { NDeposito = 6, Montante = 100, Formato_pagamento = "Master Card", Origem_deposito = "Santander Totta", UserFK = 6 },
               new Depositos { NDeposito = 7, Montante = 100, Formato_pagamento = "Master Card", Origem_deposito = "Santander Totta", UserFK = 1 },
               new Depositos { NDeposito = 8, Montante = 100, Formato_pagamento = "Master Card", Origem_deposito = "Santander Totta", UserFK = 2 },
               new Depositos { NDeposito = 9, Montante = 100, Formato_pagamento = "Master Card", Origem_deposito = "Santander Totta", UserFK = 3 }
               );
        }

        //adição das tabelas à BD
        public virtual DbSet<Utilizadores> Utilizadores { get; set; }
        public virtual DbSet<Apostas> Apostas { get; set; }
        public virtual DbSet<Jogos> Jogos { get; set; }
        public virtual DbSet<Depositos> Depositos { get; set; }
    }
}
