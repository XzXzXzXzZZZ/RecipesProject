using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesProject.Models.Configuration
{
    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {
            builder.HasKey(r => r.Id);

            builder.HasIndex(r => r.Title);

            builder.Property(r => r.Difficulty)
                .HasDefaultValue(0);

            builder.Property(r => r.Servings)
                .HasDefaultValue(4);

            builder.Property(r => r.IsFavorite)
                .HasDefaultValue(0);
           
        }
    }
}
