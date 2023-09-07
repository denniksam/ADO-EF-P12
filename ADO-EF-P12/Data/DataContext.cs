using ADO_EF_P12.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO_EF_P12.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Entity.Department> Departments { get; set; }
        public DbSet<Entity.Manager> Managers { get; set; }

        public DataContext() : base()
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder         // налаштування підключення до БД
                .UseSqlServer(     // з пакету SqlServer - драйвери МS SQL
                    @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=ado-ef-p12;Integrated Security=True"            
                );                 // рядок підключення - до неіснуючої (або порожної) БД
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // налаштування самої БД - відношень (реляцій) між даними
            // їх обмеження (розміру) та унікальність, а також сідування
            // (від англ seed - зерно) - заповнення начальними даними.
            modelBuilder                          // Налаштування навігаційної
                .Entity<Manager>()                // властивості. Зазначається:
                .HasOne(m => m.MainDep)           // - назва нав. властивості (MainDep)
                .WithMany()                       // - тип відношення (один-до-багатьох)
                .HasForeignKey(m => m.IdMainDep)  // - зовнішній ключ \  рівність яких
                .HasPrincipalKey(d => d.Id);      // - керівний ключ  /  вимагається
            // ... Managers M JOIN Departments D ON M.IdMainDep = D.Id
            // Після налаштування реляцій треба зробити та застосувати міграцію

            modelBuilder
                .Entity<Manager>()
                .HasOne(m => m.SecDep)
                .WithMany()
                .HasForeignKey(m => m.IdSecDep)
                .HasPrincipalKey(d => d.Id);
        }
    }
}
