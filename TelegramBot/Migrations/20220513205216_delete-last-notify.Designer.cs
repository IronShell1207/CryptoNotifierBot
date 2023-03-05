﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TelegramBot.Static;

#nullable disable

namespace TelegramBot.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20220513205216_delete-last-notify")]
    partial class deletelastnotify
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.4");

            modelBuilder.Entity("TelegramBot.Objects.BannedUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BanReason")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("TelegramId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("BannedUsers");
                });

            modelBuilder.Entity("TelegramBot.Objects.BlackListedPairs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Base")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Quote")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("BlackListedPairs");
                });

            modelBuilder.Entity("TelegramBot.Objects.BreakoutSub", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("BinanceSub")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("BitgetSub")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("BlackListEnable")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("GateioSub")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("KucoinSub")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("OkxSub")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("S120MinUpdates")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("S15MinUpdates")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("S1920MinUpdates")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("S240MinUpdates")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("S2MinUpdates")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("S30MinUpdates")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("S45MinUpdates")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("S480MinUpdates")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("S5MinUpdates")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("S60MinUpdates")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("S960MinUpdates")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Subscribed")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TelegramId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("WhitelistInsteadBlack")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("BreakoutSubs");
                });

            modelBuilder.Entity("TelegramBot.Objects.CryptoPair", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Enabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ExchangePlatform")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("GainOrFall")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.Property<int>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PairBase")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PairQuote")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<string>("Screenshot")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TriggerOnce")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Triggered")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("CryptoPairs");
                });

            modelBuilder.Entity("TelegramBot.Objects.NotifyMyPos", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("ProcentNotify")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("PositionsNotify");
                });

            modelBuilder.Entity("TelegramBot.Objects.PositionPair", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Base")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Entry")
                        .HasColumnType("REAL");

                    b.Property<double>("Leverage")
                        .HasColumnType("REAL");

                    b.Property<double>("Margin")
                        .HasColumnType("REAL");

                    b.Property<int>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Quote")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("StopLoss")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("TelegramBot.Objects.Takes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Price")
                        .HasColumnType("REAL");

                    b.Property<bool>("Triggered")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("PositionTakes");
                });

            modelBuilder.Entity("TelegramBot.Objects.UserConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("AntifloodIntervalAmplification")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CryptoNotifyStyle")
                        .HasColumnType("TEXT");

                    b.Property<bool>("DisplayTaskEditButtonsInNotifications")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("MorningReport")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("NightModeEnable")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NightModeEndsTime")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NightModeStartTime")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("NotesEnable")
                        .HasColumnType("INTEGER");

                    b.Property<int>("NoticationsInterval")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("RemoveLatestNotifyBeforeNew")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShowMarketEvents")
                        .HasColumnType("INTEGER");

                    b.Property<long>("TelegramId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TelegramBot.Objects.BannedUser", b =>
                {
                    b.HasOne("TelegramBot.Objects.UserConfig", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("TelegramBot.Objects.BlackListedPairs", b =>
                {
                    b.HasOne("TelegramBot.Objects.BreakoutSub", "Sub")
                        .WithMany("BlackListedPairsList")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sub");
                });

            modelBuilder.Entity("TelegramBot.Objects.CryptoPair", b =>
                {
                    b.HasOne("TelegramBot.Objects.UserConfig", "User")
                        .WithMany("pairs")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("TelegramBot.Objects.BreakoutSub", b =>
                {
                    b.Navigation("BlackListedPairsList");
                });

            modelBuilder.Entity("TelegramBot.Objects.UserConfig", b =>
                {
                    b.Navigation("pairs");
                });
#pragma warning restore 612, 618
        }
    }
}