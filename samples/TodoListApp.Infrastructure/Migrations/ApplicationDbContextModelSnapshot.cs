﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TodoListApp.Infrastructure.Repositories;

#nullable disable

namespace TodoListApp.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("TodoListApp.Domain.Entities.TodoItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("CreatedBy")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("DeletedBy")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDone")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(false);

                    b.Property<string>("PriorityLevel")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("TodoItems");

                    b.HasData(
                        new
                        {
                            Id = new Guid("08daa6de-fabb-4e57-86e0-386a66fe8ae7"),
                            IsDeleted = false,
                            IsDone = false,
                            PriorityLevel = "Low",
                            Text = "Write project proposal 📃"
                        },
                        new
                        {
                            Id = new Guid("08daa6df-0599-4227-8e79-ac83b22305f3"),
                            IsDeleted = false,
                            IsDone = false,
                            PriorityLevel = "Medium",
                            Text = "Schedule kick-off meeting ✅"
                        },
                        new
                        {
                            Id = new Guid("08daa6df-1457-4ff3-8080-a57e71d0d80c"),
                            IsDeleted = false,
                            IsDone = false,
                            PriorityLevel = "High",
                            Text = "Review research results 🤯"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
