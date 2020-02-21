using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.SCIM;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mca.AzureAd.Scim.SqlProvider
{
    class UserConfiguration : IEntityTypeConfiguration<Core2EnterpriseUser>
    {
        public void Configure(EntityTypeBuilder<Core2EnterpriseUser> builder)
        {
            builder.ToTable("User");
            builder.Property(i => i.Identifier)
                .HasColumnName("UserIdentifier");

            builder.HasKey(i => i.Identifier);

            builder.OwnsOne(i => i.Metadata);
            builder.OwnsOne(i => i.Name);

            builder.OwnsOne(i => i.EnterpriseExtension, (h) =>
            {
                h.OwnsOne(o => o.Manager);
            });

            // Roles is a read only property
            builder.Ignore(i => i.Roles);
        }
    }

    #region 1:Many relationships

    class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address");
            builder.Property<Guid>("AddressIdentifier");

            builder.HasKey("AddressIdentifier");
            builder.HasOne<Core2EnterpriseUser>()
                .WithMany(u => u.Addresses)
                .HasForeignKey("UserIdentifier")
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

    class EmailConfiguration : IEntityTypeConfiguration<ElectronicMailAddress>
    {
        public void Configure(EntityTypeBuilder<ElectronicMailAddress> builder)
        {
            builder.ToTable("Email");
            builder.Property<Guid>("EmailIdentifier");

            builder.HasKey("EmailIdentifier");
            builder.HasOne<Core2EnterpriseUser>()
                .WithMany(u => u.ElectronicMailAddresses)
                .HasForeignKey("UserIdentifier")
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

    class ImConfiguration : IEntityTypeConfiguration<InstantMessaging>
    {
        public void Configure(EntityTypeBuilder<InstantMessaging> builder)
        {
            builder.ToTable("InstantMessaging");
            builder.Property<Guid>("InstantMessagingIdentifier");

            builder.HasKey("InstantMessagingIdentifier");
            builder.HasOne<Core2EnterpriseUser>()
                .WithMany(u => u.InstantMessagings)
                .HasForeignKey("UserIdentifier")
                .OnDelete(DeleteBehavior.Cascade);
        }

    }

    class PhoneConfiguration : IEntityTypeConfiguration<PhoneNumber>
    {
        public void Configure(EntityTypeBuilder<PhoneNumber> builder)
        {
            builder.ToTable("Phone");
            builder.Property<Guid>("PhoneIdentifier");

            builder.HasKey("PhoneIdentifier");
            builder.HasOne<Core2EnterpriseUser>()
                .WithMany(u => u.PhoneNumbers)
                .HasForeignKey("UserIdentifier")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    #endregion

}

