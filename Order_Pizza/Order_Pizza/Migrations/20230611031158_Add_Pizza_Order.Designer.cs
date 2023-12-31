﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Order_Pizza.Data;

#nullable disable

namespace Order_Pizza.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230611031158_Add_Pizza_Order")]
    partial class Add_Pizza_Order
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Order_Pizza.Areas.Admin.Models.Pizza", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Image_Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Pizzas");
                });

            modelBuilder.Entity("Order_Pizza.Areas.Customer.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Date_Time_Insert")
                        .HasColumnType("datetime2");

                    b.Property<int>("Pizza_Id")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Pizza_Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Order_Pizza.Areas.Customer.Models.Order", b =>
                {
                    b.HasOne("Order_Pizza.Areas.Admin.Models.Pizza", "Pizza")
                        .WithMany()
                        .HasForeignKey("Pizza_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pizza");
                });
#pragma warning restore 612, 618
        }
    }
}
