using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperShop.Data.Entities;
using System.Linq;
using System.Reflection.Emit;

namespace SuperShop.Data
{
    //responsável pela ligação à BD - herda de um identity e recebe um objecto user
    public class DataContext : IdentityDbContext<User>
    {
        //Criar tabelas
        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<OrderDetailTemp> OrderDetailsTemp { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<City> Cities { get; set; }


        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            modelbuilder.Entity<Country>()
                .HasIndex(c => c.Name)
            .IsUnique();

            modelbuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelbuilder.Entity<OrderDetailTemp>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelbuilder.Entity<OrderDetail>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelbuilder);
        }

        


        //Habilitar apagar em cascata
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    var cascadeFKs = modelBuilder.Model
        //         .GetEntityTypes()
        //         .SelectMany(t => t.GetForeignKeys())
        //         .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        //    foreach(var fk in cascadeFKs)
        //    {
        //        fk.DeleteBehavior = DeleteBehavior.Restrict;
        //    }

        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
