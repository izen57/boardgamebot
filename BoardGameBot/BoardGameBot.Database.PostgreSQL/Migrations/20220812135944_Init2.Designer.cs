﻿// <auto-generated />
using System;
using BoardGameBot.Database.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BoardGameBot.Database.PostgreSQL.Migrations
{
    [DbContext(typeof(BoardGameContext))]
    [Migration("20220812135944_Init2")]
    partial class Init2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BoardGameBot.Database.PostgreSQL.Models.Game", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("Complexity")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Genre")
                        .HasColumnType("text");

                    b.Property<string>("LetsPlay")
                        .HasColumnType("text");

                    b.Property<int>("Played")
                        .HasColumnType("integer");

                    b.Property<string>("Players")
                        .HasColumnType("text");

                    b.Property<string>("Rules")
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Games");
                });

            modelBuilder.Entity("BoardGameBot.Database.PostgreSQL.Models.GameOwner", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("GroupAdminId")
                        .HasColumnType("bigint");

                    b.Property<long?>("GroupMemberId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("TGRef")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GroupAdminId");

                    b.HasIndex("GroupMemberId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("GameOwners");
                });

            modelBuilder.Entity("BoardGameBot.Database.PostgreSQL.Models.Group", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("BoardGameBot.Database.PostgreSQL.Models.Poll", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("DayInterval")
                        .HasMaxLength(1)
                        .HasColumnType("integer");

                    b.Property<long?>("GroupId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Time")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Polls");
                });

            modelBuilder.Entity("GameGameOwner", b =>
                {
                    b.Property<long>("GameOwnersId")
                        .HasColumnType("bigint");

                    b.Property<long>("GamesId")
                        .HasColumnType("bigint");

                    b.HasKey("GameOwnersId", "GamesId");

                    b.HasIndex("GamesId");

                    b.ToTable("GameGameOwner");
                });

            modelBuilder.Entity("BoardGameBot.Database.PostgreSQL.Models.GameOwner", b =>
                {
                    b.HasOne("BoardGameBot.Database.PostgreSQL.Models.Group", "GroupAdmin")
                        .WithMany("Admins")
                        .HasForeignKey("GroupAdminId");

                    b.HasOne("BoardGameBot.Database.PostgreSQL.Models.Group", "GroupMember")
                        .WithMany("Members")
                        .HasForeignKey("GroupMemberId");

                    b.Navigation("GroupAdmin");

                    b.Navigation("GroupMember");
                });

            modelBuilder.Entity("BoardGameBot.Database.PostgreSQL.Models.Poll", b =>
                {
                    b.HasOne("BoardGameBot.Database.PostgreSQL.Models.Group", "Group")
                        .WithMany("Polls")
                        .HasForeignKey("GroupId");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("GameGameOwner", b =>
                {
                    b.HasOne("BoardGameBot.Database.PostgreSQL.Models.GameOwner", null)
                        .WithMany()
                        .HasForeignKey("GameOwnersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BoardGameBot.Database.PostgreSQL.Models.Game", null)
                        .WithMany()
                        .HasForeignKey("GamesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BoardGameBot.Database.PostgreSQL.Models.Group", b =>
                {
                    b.Navigation("Admins");

                    b.Navigation("Members");

                    b.Navigation("Polls");
                });
#pragma warning restore 612, 618
        }
    }
}
