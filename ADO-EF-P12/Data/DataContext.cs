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
    }
}
