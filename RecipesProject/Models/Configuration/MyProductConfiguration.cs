using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesProject.Models.Configuration
{
    public class MyProductConfiguration : IEntityTypeConfiguration<MyProduct>
    {
        public void Configure(EntityTypeBuilder<MyProduct> builder)
        {
            builder.HasKey(p=>p.Id);

            builder.HasIndex(p => p.Name)
                .IsUnique();

            builder.Property(p=>p.IsPermanent)
                .HasDefaultValue(0);
        }
    }
}
