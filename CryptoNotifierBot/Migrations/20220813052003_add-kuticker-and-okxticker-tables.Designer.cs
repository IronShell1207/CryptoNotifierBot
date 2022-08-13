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
    [Migration("20220813052003_add-kuticker-and-okxticker-tables")]
    partial class addkutickerandokxtickertables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.8");

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

            modelBuilder.Entity("CryptoApi.Objects.KuTickerDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("averagePrice")
                        .HasColumnType("TEXT");

                    b.Property<string>("buy")
                        .HasColumnType("TEXT");

                    b.Property<string>("changePrice")
                        .HasColumnType("TEXT");

                    b.Property<string>("changeRate")
                        .HasColumnType("TEXT");

                    b.Property<string>("high")
                        .HasColumnType("TEXT");

                    b.Property<string>("last")
                        .HasColumnType("TEXT");

                    b.Property<string>("low")
                        .HasColumnType("TEXT");

                    b.Property<string>("makerCoefficient")
                        .HasColumnType("TEXT");

                    b.Property<string>("sell")
                        .HasColumnType("TEXT");

                    b.Property<string>("symbol")
                        .HasColumnType("TEXT");

                    b.Property<string>("symbolName")
                        .HasColumnType("TEXT");

                    b.Property<string>("takerCoefficient")
                        .HasColumnType("TEXT");

                    b.Property<string>("vol")
                        .HasColumnType("TEXT");

                    b.Property<string>("volValue")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("KucoinPairs");
                });

            modelBuilder.Entity("CryptoApi.Objects.OkxTickerDB", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("askPx")
                        .HasColumnType("TEXT");

                    b.Property<string>("askSz")
                        .HasColumnType("TEXT");

                    b.Property<string>("bidPx")
                        .HasColumnType("TEXT");

                    b.Property<string>("bidSz")
                        .HasColumnType("TEXT");

                    b.Property<string>("high24h")
                        .HasColumnType("TEXT");

                    b.Property<string>("instId")
                        .HasColumnType("TEXT");

                    b.Property<string>("last")
                        .HasColumnType("TEXT");

                    b.Property<string>("lastSz")
                        .HasColumnType("TEXT");

                    b.Property<string>("low24h")
                        .HasColumnType("TEXT");

                    b.Property<string>("open24h")
                        .HasColumnType("TEXT");

                    b.Property<string>("vol24h")
                        .HasColumnType("TEXT");

                    b.Property<string>("volCcy24h")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("OkxPairs");
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
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<string>("Quote")
                        .IsRequired()
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
