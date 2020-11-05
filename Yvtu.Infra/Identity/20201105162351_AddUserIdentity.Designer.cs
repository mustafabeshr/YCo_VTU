﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;
using Yvtu.Infra.Identity;

namespace Yvtu.Infra.Identity
{
    [DbContext(typeof(AppIdentityDbContext))]
    [Migration("20201105162351_AddUserIdentity")]
    partial class AddUserIdentity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("ID")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnName("CONCURRENCY_STAMP")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Name")
                        .HasColumnName("NAME")
                        .HasColumnType("NVARCHAR2(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnName("NORMALIZED_NAME")
                        .HasColumnType("NVARCHAR2(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id")
                        .HasName("PK_ROLE");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("ROLE_NAME_INDEX");

                    b.ToTable("ROLE");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(10)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnName("CLAIM_TYPE")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("ClaimValue")
                        .HasColumnName("CLAIM_VALUE")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnName("ROLE_ID")
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("Id")
                        .HasName("PK_APP_ROLE_CLAIMS");

                    b.HasIndex("RoleId")
                        .HasName("IX_APP_ROLE_CLAIMS_ROLE_ID");

                    b.ToTable("APP_ROLE_CLAIMS");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(10)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnName("CLAIM_TYPE")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("ClaimValue")
                        .HasColumnName("CLAIM_VALUE")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnName("USER_ID")
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("Id")
                        .HasName("PK_APP_USER_CLAIMS");

                    b.HasIndex("UserId")
                        .HasName("IX_APP_USER_CLAIMS_USER_ID");

                    b.ToTable("APP_USER_CLAIMS");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnName("LOGIN_PROVIDER")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnName("PROVIDER_KEY")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnName("PROVIDER_DISPLAY_NAME")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnName("USER_ID")
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("PK_APP_USER_LOGINS");

                    b.HasIndex("UserId")
                        .HasName("IX_APP_USER_LOGINS_USER_ID");

                    b.ToTable("APP_USER_LOGINS");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnName("USER_ID")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("RoleId")
                        .HasColumnName("ROLE_ID")
                        .HasColumnType("NVARCHAR2(450)");

                    b.HasKey("UserId", "RoleId")
                        .HasName("PK_APP_USER_ROLES");

                    b.HasIndex("RoleId")
                        .HasName("IX_APP_USER_ROLES_ROLE_ID");

                    b.ToTable("APP_USER_ROLES");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnName("USER_ID")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnName("LOGIN_PROVIDER")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("Name")
                        .HasColumnName("NAME")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("Value")
                        .HasColumnName("VALUE")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("PK_APP_USER_TOKENS");

                    b.ToTable("APP_USER_TOKENS");
                });

            modelBuilder.Entity("Yvtu.Core.Entities.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("ID")
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnName("ACCESS_FAILED_COUNT")
                        .HasColumnType("NUMBER(10)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnName("CONCURRENCY_STAMP")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("DisplayName")
                        .HasColumnName("DISPLAY_NAME")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("Email")
                        .HasColumnName("EMAIL")
                        .HasColumnType("NVARCHAR2(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnName("EMAIL_CONFIRMED")
                        .HasColumnType("NUMBER(1)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnName("LOCKOUT_ENABLED")
                        .HasColumnType("NUMBER(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnName("LOCKOUT_END")
                        .HasColumnType("TIMESTAMP(7) WITH TIME ZONE");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnName("NORMALIZED_EMAIL")
                        .HasColumnType("NVARCHAR2(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnName("NORMALIZED_USER_NAME")
                        .HasColumnType("NVARCHAR2(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnName("PASSWORD_HASH")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnName("PHONE_NUMBER")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnName("PHONE_NUMBER_CONFIRMED")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnName("SECURITY_STAMP")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnName("TWO_FACTOR_ENABLED")
                        .HasColumnType("NUMBER(1)");

                    b.Property<string>("UserName")
                        .HasColumnName("USER_NAME")
                        .HasColumnType("NVARCHAR2(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id")
                        .HasName("PK_APP_USER");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EMAIL_INDEX");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("USER_NAME_INDEX");

                    b.ToTable("APP_USER");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Yvtu.Core.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Yvtu.Core.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Yvtu.Core.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Yvtu.Core.Entities.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
