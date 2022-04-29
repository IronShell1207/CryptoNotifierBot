﻿// <auto-generated />
using System;
using CryptoApi.Static.DataHandler;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CryptoApi.Migrations
{
    [DbContext(typeof(DataBaseContext))]
    [Migration("20220429133621_PairsConnectedtodb")]
    partial class PairsConnectedtodb
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.4");

            modelBuilder.Entity("CryptoApi.Objects.CryDbSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Exchange")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("IdGuid")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DataSet");
                });

            modelBuilder.Entity("CryptoApi.Objects.PricedTradingPair", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CryDbSetId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Exchange")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<string>("Quote")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CryDbSetId");

                    b.ToTable("TradingPairs");
                });

            modelBuilder.Entity("CryptoApi.Objects.PricedTradingPair", b =>
                {
                    b.HasOne("CryptoApi.Objects.CryDbSet", "CryDbSet")
                        .WithMany("pairs")
                        .HasForeignKey("CryDbSetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CryDbSet");
                });

            modelBuilder.Entity("CryptoApi.Objects.CryDbSet", b =>
                {
                    b.Navigation("pairs");
                });
#pragma warning restore 612, 618
        }
    }
}
