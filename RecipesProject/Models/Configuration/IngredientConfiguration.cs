using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesProject.Models.Configuration
{
    public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
    {
        public void Configure(EntityTypeBuilder<Ingredient> builder)
        {
            builder.HasKey(i => i.Id);

            builder.HasOne(i=>i.Recipe)
                .WithMany(i => i.Ingredients)
                .HasForeignKey(i=>i.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
