using Microsoft.EntityFrameworkCore;
using RecipesProject.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesProject.Models
{
    public class DBContext:DbContext
    {
        public DbSet<Recipe> Recipes { get; set; } = null!;
        public DbSet<Step> Steps { get; set; } = null!;
        public DbSet<Ingredient> Ingredients { get; set; } = null!;
        public DbSet<MyProduct> MyProducts { get; set; } = null!;
        public DbSet<PermanentProduct> PermanentProducts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=HOME-PC\\MSSQLSERVER01;Database=RecipesDataBase;Integrated Security=True;TrustServerCertificate=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RecipeConfiguration());
            modelBuilder.ApplyConfiguration(new StepConfiguration());
            modelBuilder.ApplyConfiguration(new IngredientConfiguration());
            modelBuilder.ApplyConfiguration(new MyProductConfiguration());
        }
    }
}
