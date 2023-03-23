﻿// <auto-generated />
using System;
using EQToolApis.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EQToolApis.Migrations
{
    [DbContext(typeof(EQToolContext))]
    partial class EQToolContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EQToolApis.DB.Models.EQAuctionPlayer", b =>
                {
                    b.Property<int>("EQAuctionPlayerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EQAuctionPlayerId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<byte>("Server")
                        .HasColumnType("tinyint");

                    b.HasKey("EQAuctionPlayerId");

                    b.HasIndex("Server");

                    b.ToTable("EQAuctionPlayers");
                });

            modelBuilder.Entity("EQToolApis.DB.Models.EQTunnelAuctionItem", b =>
                {
                    b.Property<long>("EQTunnelAuctionItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("EQTunnelAuctionItemId"));

                    b.Property<int?>("AuctionPrice")
                        .HasColumnType("int");

                    b.Property<long>("EQTunnelMessageId")
                        .HasColumnType("bigint");

                    b.Property<int>("EQitemId")
                        .HasColumnType("int");

                    b.Property<byte>("Server")
                        .HasColumnType("tinyint");

                    b.HasKey("EQTunnelAuctionItemId");

                    b.HasIndex("EQTunnelMessageId");

                    b.HasIndex("EQitemId");

                    b.ToTable("EQTunnelAuctionItems");
                });

            modelBuilder.Entity("EQToolApis.DB.Models.EQTunnelMessage", b =>
                {
                    b.Property<long>("EQTunnelMessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("EQTunnelMessageId"));

                    b.Property<byte>("AuctionType")
                        .HasColumnType("tinyint");

                    b.Property<long>("DiscordMessageId")
                        .HasColumnType("bigint");

                    b.Property<int>("EQAuctionPlayerId")
                        .HasColumnType("int");

                    b.Property<byte>("Server")
                        .HasColumnType("tinyint");

                    b.Property<DateTimeOffset>("TunnelTimestamp")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("EQTunnelMessageId");

                    b.HasIndex("AuctionType");

                    b.HasIndex("DiscordMessageId");

                    b.HasIndex("EQAuctionPlayerId");

                    b.HasIndex("Server");

                    b.HasIndex("TunnelTimestamp");

                    b.HasIndex("Server", "AuctionType");

                    b.HasIndex("TunnelTimestamp", "AuctionType");

                    b.ToTable("EQTunnelMessages");
                });

            modelBuilder.Entity("EQToolApis.DB.Models.EQitem", b =>
                {
                    b.Property<int>("EQitemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EQitemId"));

                    b.Property<string>("ItemName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<DateTimeOffset>("LastWTBSeen")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset>("LastWTSSeen")
                        .HasColumnType("datetimeoffset");

                    b.Property<byte>("Server")
                        .HasColumnType("tinyint");

                    b.Property<int>("TotalWTBAuctionAverage")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTBAuctionCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTBLast30DaysAverage")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTBLast30DaysCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTBLast60DaysAverage")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTBLast60DaysCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTBLast6MonthsAverage")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTBLast6MonthsCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTBLast90DaysAverage")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTBLast90DaysCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTBLastYearAverage")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTBLastYearCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTSAuctionAverage")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTSAuctionCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTSLast30DaysAverage")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTSLast30DaysCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTSLast60DaysAverage")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTSLast60DaysCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTSLast6MonthsAverage")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTSLast6MonthsCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTSLast90DaysAverage")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTSLast90DaysCount")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTSLastYearAverage")
                        .HasColumnType("int");

                    b.Property<int>("TotalWTSLastYearCount")
                        .HasColumnType("int");

                    b.HasKey("EQitemId");

                    b.HasIndex("Server");

                    b.HasIndex("ItemName", "Server");

                    b.ToTable("EQitems");
                });

            modelBuilder.Entity("EQToolApis.DB.Models.EqToolException", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Exception")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("nvarchar(24)");

                    b.HasKey("Id");

                    b.ToTable("EqToolExceptions");
                });

            modelBuilder.Entity("EQToolApis.DB.Models.Player", b =>
                {
                    b.Property<string>("Name")
                        .HasMaxLength(24)
                        .HasColumnType("nvarchar(24)");

                    b.Property<byte>("Server")
                        .HasColumnType("tinyint");

                    b.Property<byte>("Level")
                        .HasColumnType("tinyint");

                    b.Property<byte>("PlayerClass")
                        .HasColumnType("tinyint");

                    b.HasKey("Name", "Server");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("EQToolApis.DB.Models.EQTunnelAuctionItem", b =>
                {
                    b.HasOne("EQToolApis.DB.Models.EQTunnelMessage", "EQTunnelMessage")
                        .WithMany("EQTunnelAuctionItems")
                        .HasForeignKey("EQTunnelMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("EQToolApis.DB.Models.EQitem", "EQitem")
                        .WithMany("EQTunnelAuctionItems")
                        .HasForeignKey("EQitemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EQTunnelMessage");

                    b.Navigation("EQitem");
                });

            modelBuilder.Entity("EQToolApis.DB.Models.EQTunnelMessage", b =>
                {
                    b.HasOne("EQToolApis.DB.Models.EQAuctionPlayer", "EQAuctionPlayer")
                        .WithMany()
                        .HasForeignKey("EQAuctionPlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EQAuctionPlayer");
                });

            modelBuilder.Entity("EQToolApis.DB.Models.EQTunnelMessage", b =>
                {
                    b.Navigation("EQTunnelAuctionItems");
                });

            modelBuilder.Entity("EQToolApis.DB.Models.EQitem", b =>
                {
                    b.Navigation("EQTunnelAuctionItems");
                });
#pragma warning restore 612, 618
        }
    }
}
