using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesProject.Models.Configuration
{
    public class StepConfiguration : IEntityTypeConfiguration<Step>
    {
        public void Configure(EntityTypeBuilder<Step> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasOne(s=>s.Recipe)
                .WithMany(s=>s.Steps)
                .HasForeignKey(s=>s.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
