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
    public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
    {
        public void Configure(EntityTypeBuilder<Recipe> builder)
        {

            builder.Property(r => r.Title)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(r => r.Description)
                   .HasMaxLength(500);

            builder.Property(r => r.Ingredients)
                   .HasColumnType("nvarchar(max)");

            builder.Property(r => r.Instructions)
                   .HasColumnType("nvarchar(max)");

            builder.Property(r => r.CreatedDate)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(r => r.Category)
                   .WithMany(c => c.Recipes)
                   .HasForeignKey(r => r.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(r => r.User)
                   .WithMany(u => u.Recipes)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.NoAction); 
        }
    }
}