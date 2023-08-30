﻿// <auto-generated />
using System;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Entities.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230829220254_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Entities.Country", b =>
                {
                    b.Property<Guid>("CountryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CountryName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CountryID");

                    b.ToTable("Countries", (string)null);

                    b.HasData(
                        new
                        {
                            CountryID = new Guid("57a6d8da-40fc-48cf-a496-43046ab31c56"),
                            CountryName = "USA"
                        },
                        new
                        {
                            CountryID = new Guid("82be018d-6b88-4731-b27c-ec1c562de18b"),
                            CountryName = "UK"
                        },
                        new
                        {
                            CountryID = new Guid("31d3283b-3e8a-4648-a6d8-1e16c3a2ce0a"),
                            CountryName = "France"
                        },
                        new
                        {
                            CountryID = new Guid("1a363258-fb99-4130-ad60-40aed7b18c5a"),
                            CountryName = "Japan"
                        },
                        new
                        {
                            CountryID = new Guid("a0aeddfc-b16e-460b-b814-22409ced41e5"),
                            CountryName = "Australia"
                        });
                });

            modelBuilder.Entity("Entities.Person", b =>
                {
                    b.Property<Guid>("PersonID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<Guid?>("CountryID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<string>("Gender")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("PersonName")
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<bool>("ReceiveNewsLetters")
                        .HasColumnType("bit");

                    b.HasKey("PersonID");

                    b.ToTable("Persons", (string)null);

                    b.HasData(
                        new
                        {
                            PersonID = new Guid("c03bbe45-9aeb-4d24-99e0-4743016ffce9"),
                            Address = "4 Parkside Point",
                            CountryID = new Guid("57a6d8da-40fc-48cf-a496-43046ab31c56"),
                            DateOfBirth = new DateTime(1989, 8, 28, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "mwebsdale0@people.com.cn",
                            Gender = "Female",
                            PersonName = "Marguerite",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            PersonID = new Guid("c3abddbd-cf50-41d2-b6c4-cc7d5a750928"),
                            Address = "6 Morningstar Circle",
                            CountryID = new Guid("57a6d8da-40fc-48cf-a496-43046ab31c56"),
                            DateOfBirth = new DateTime(1990, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "ushears1@globo.com",
                            Gender = "Female",
                            PersonName = "Ursa",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            PersonID = new Guid("c6d50a47-f7e6-4482-8be0-4ddfc057fa6e"),
                            Address = "73 Heath Avenue",
                            CountryID = new Guid("82be018d-6b88-4731-b27c-ec1c562de18b"),
                            DateOfBirth = new DateTime(1995, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "fbowsher2@howstuffworks.com",
                            Gender = "Male",
                            PersonName = "Franchot",
                            ReceiveNewsLetters = true
                        },
                        new
                        {
                            PersonID = new Guid("d15c6d9f-70b4-48c5-afd3-e71261f1a9be"),
                            Address = "83187 Merry Drive",
                            CountryID = new Guid("82be018d-6b88-4731-b27c-ec1c562de18b"),
                            DateOfBirth = new DateTime(1987, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "asarvar3@dropbox.com",
                            Gender = "Male",
                            PersonName = "Angie",
                            ReceiveNewsLetters = true
                        },
                        new
                        {
                            PersonID = new Guid("89e5f445-d89f-4e12-94e0-5ad5b235d704"),
                            Address = "50467 Holy Cross Crossing",
                            CountryID = new Guid("31d3283b-3e8a-4648-a6d8-1e16c3a2ce0a"),
                            DateOfBirth = new DateTime(1995, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "ttregona4@stumbleupon.com",
                            Gender = "Gender",
                            PersonName = "Tani",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            PersonID = new Guid("2a6d3738-9def-43ac-9279-0310edc7ceca"),
                            Address = "97570 Raven Circle",
                            CountryID = new Guid("31d3283b-3e8a-4648-a6d8-1e16c3a2ce0a"),
                            DateOfBirth = new DateTime(1988, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "mlingfoot5@netvibes.com",
                            Gender = "Male",
                            PersonName = "Mitchael",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            PersonID = new Guid("29339209-63f5-492f-8459-754943c74abf"),
                            Address = "57449 Brown Way",
                            CountryID = new Guid("1a363258-fb99-4130-ad60-40aed7b18c5a"),
                            DateOfBirth = new DateTime(1983, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "mjarrell6@wisc.edu",
                            Gender = "Male",
                            PersonName = "Maddy",
                            ReceiveNewsLetters = true
                        },
                        new
                        {
                            PersonID = new Guid("ac660a73-b0b7-4340-abc1-a914257a6189"),
                            Address = "4 Stuart Drive",
                            CountryID = new Guid("1a363258-fb99-4130-ad60-40aed7b18c5a"),
                            DateOfBirth = new DateTime(1998, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "pretchford7@virginia.edu",
                            Gender = "Female",
                            PersonName = "Pegeen",
                            ReceiveNewsLetters = true
                        },
                        new
                        {
                            PersonID = new Guid("012107df-862f-4f16-ba94-e5c16886f005"),
                            Address = "413 Sachtjen Way",
                            CountryID = new Guid("a0aeddfc-b16e-460b-b814-22409ced41e5"),
                            DateOfBirth = new DateTime(1990, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "hmosco8@tripod.com",
                            Gender = "Male",
                            PersonName = "Hansiain",
                            ReceiveNewsLetters = true
                        },
                        new
                        {
                            PersonID = new Guid("cb035f22-e7cf-4907-bd07-91cfee5240f3"),
                            Address = "484 Clarendon Court",
                            CountryID = new Guid("a0aeddfc-b16e-460b-b814-22409ced41e5"),
                            DateOfBirth = new DateTime(1997, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "lwoodwing9@wix.com",
                            Gender = "Male",
                            PersonName = "Lombard",
                            ReceiveNewsLetters = false
                        },
                        new
                        {
                            PersonID = new Guid("28d11936-9466-4a4b-b9c5-2f0a8e0cbde9"),
                            Address = "2 Warrior Avenue",
                            CountryID = new Guid("57a6d8da-40fc-48cf-a496-43046ab31c56"),
                            DateOfBirth = new DateTime(1990, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "mconachya@va.gov",
                            Gender = "Female",
                            PersonName = "Minta",
                            ReceiveNewsLetters = true
                        },
                        new
                        {
                            PersonID = new Guid("a3b9833b-8a4d-43e9-8690-61e08df81a9a"),
                            Address = "9334 Fremont Street",
                            CountryID = new Guid("82be018d-6b88-4731-b27c-ec1c562de18b"),
                            DateOfBirth = new DateTime(1987, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "vklussb@nationalgeographic.com",
                            Gender = "Female",
                            PersonName = "Verene",
                            ReceiveNewsLetters = true
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
