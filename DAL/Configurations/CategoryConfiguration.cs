using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {


            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.HasMany(c => c.Recipes)

                   .WithOne(r => r.Category)
                   .HasForeignKey(r => r.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

} 
