﻿using BlogApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApp.Persistence.Configurations
{
    public class AppUserTokenConfiguraiton : IEntityTypeConfiguration<AppUserToken>
    {
        public void Configure(EntityTypeBuilder<AppUserToken> b)
        {
            // Composite primary key consisting of the UserId, LoginProvider and Name
            b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            // Limit the size of the composite key columns due to common DB restrictions
            b.Property(t => t.LoginProvider).HasMaxLength(256);
            b.Property(t => t.Name).HasMaxLength(256);

            // Maps to the AspNetUserTokens table
            b.ToTable("AppUserTokens");
        }
    }
}
