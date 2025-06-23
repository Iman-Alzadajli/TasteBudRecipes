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
    public class RatingConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.Property(r => r.Score)
                    .IsRequired();

            builder.Property(r => r.Comment)
                   .HasMaxLength(300);

            builder.Property(r => r.RatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            // Relationship with Recipe using NoAction to prevent cascade conflicts
            builder.HasOne(r => r.Recipe)
                   .WithMany(r => r.Ratings)
                   .HasForeignKey(r => r.RecipeId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Relationship with User useing NoAction to prevent cascade conflicts
            builder.HasOne(r => r.User)
                   .WithMany(u => u.Ratings)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.NoAction); 

            builder.HasIndex(r => new { r.UserId, r.RecipeId })
                   .IsUnique()
                   .HasDatabaseName("IX_Ratings_User_Recipe");
        }
    }
}