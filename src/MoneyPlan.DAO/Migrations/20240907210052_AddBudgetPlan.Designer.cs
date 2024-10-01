﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Savings.DAO.Infrastructure;

#nullable disable

namespace Savings.DAO.Migrations
{
    [DbContext(typeof(SavingsContext))]
    [Migration("20240907210052_AddBudgetPlan")]
    partial class AddBudgetPlan
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("Savings.Model.BudgetPlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<int>("NeedsPercentage")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SavingsPercentage")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WantsPercentage")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("BudgetPlans");
                });

            modelBuilder.Entity("Savings.Model.BudgetPlanRule", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BudgetPlan")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BudgetPlanId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CategoryFilter")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CategoryText")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Income")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("BudgetPlanId");

                    b.HasIndex("CategoryId");

                    b.ToTable("BudgetPlanRules");
                });

            modelBuilder.Entity("Savings.Model.Configuration", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("CashWithdrawalCategoryID")
                        .HasColumnType("INTEGER");

                    b.Property<short>("EndPeriodRecurrencyInterval")
                        .HasColumnType("INTEGER");

                    b.Property<int>("EndPeriodRecurrencyType")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Configuration");

                    b.HasData(
                        new
                        {
                            ID = 1L,
                            CashWithdrawalCategoryID = 0L,
                            EndPeriodRecurrencyInterval = (short)1,
                            EndPeriodRecurrencyType = 2
                        });
                });

            modelBuilder.Entity("Savings.Model.FixedMoneyItem", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccountID")
                        .HasColumnType("INTEGER");

                    b.Property<decimal?>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Cash")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("CategoryID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.Property<int>("TimelineWeight")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("AccountID");

                    b.HasIndex("CategoryID");

                    b.ToTable("FixedMoneyItems");
                });

            modelBuilder.Entity("Savings.Model.MaterializedMoneyItem", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Cash")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("CategoryID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<bool>("EndPeriod")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("EndPeriodCashCarry")
                        .HasColumnType("TEXT");

                    b.Property<long?>("FixedMoneyItemID")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsRecurrent")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Projection")
                        .HasColumnType("TEXT");

                    b.Property<long?>("RecurrentMoneyItemID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TimelineWeight")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("FixedMoneyItemID")
                        .IsUnique();

                    b.HasIndex("RecurrentMoneyItemID");

                    b.ToTable("MaterializedMoneyItems");

                    b.HasData(
                        new
                        {
                            ID = 1L,
                            Amount = 0m,
                            Cash = false,
                            Date = new DateTime(2024, 8, 31, 0, 0, 0, 0, DateTimeKind.Local),
                            EndPeriod = true,
                            EndPeriodCashCarry = 0m,
                            IsRecurrent = false,
                            Projection = 0m,
                            TimelineWeight = 0,
                            Type = 0
                        });
                });

            modelBuilder.Entity("Savings.Model.MoneyAccount", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("MoneyAccounts", (string)null);
                });

            modelBuilder.Entity("Savings.Model.MoneyCategory", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Icon")
                        .HasColumnType("TEXT");

                    b.Property<long?>("ParentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("ParentId");

                    b.ToTable("MoneyCategories");

                    b.HasData(
                        new
                        {
                            ID = 1L,
                            Description = "Family"
                        },
                        new
                        {
                            ID = 16L,
                            Description = "Food & Groceries",
                            ParentId = 1L
                        },
                        new
                        {
                            ID = 2L,
                            Description = "Home"
                        },
                        new
                        {
                            ID = 17L,
                            Description = "Mortgage",
                            ParentId = 2L
                        },
                        new
                        {
                            ID = 3L,
                            Description = "Leisure Time"
                        },
                        new
                        {
                            ID = 18L,
                            Description = "Restaurant",
                            ParentId = 3L
                        },
                        new
                        {
                            ID = 19L,
                            Description = "Tobacco shop",
                            ParentId = 3L
                        },
                        new
                        {
                            ID = 20L,
                            Description = "Other",
                            ParentId = 3L
                        },
                        new
                        {
                            ID = 21L,
                            Description = "Shows, Concerts & Museums",
                            ParentId = 3L
                        },
                        new
                        {
                            ID = 22L,
                            Description = "Subscriptions",
                            ParentId = 3L
                        },
                        new
                        {
                            ID = 4L,
                            Description = "Transports"
                        },
                        new
                        {
                            ID = 23L,
                            Description = "Public Transport",
                            ParentId = 4L
                        },
                        new
                        {
                            ID = 24L,
                            Description = "Car",
                            ParentId = 4L
                        },
                        new
                        {
                            ID = 25L,
                            Description = "Loan",
                            ParentId = 4L
                        },
                        new
                        {
                            ID = 26L,
                            Description = "Fuel",
                            ParentId = 4L
                        },
                        new
                        {
                            ID = 5L,
                            Description = "Financial Trading"
                        },
                        new
                        {
                            ID = 27L,
                            Description = "Compravendita titoli",
                            ParentId = 5L
                        },
                        new
                        {
                            ID = 28L,
                            Description = "Subscriptions",
                            ParentId = 5L
                        },
                        new
                        {
                            ID = 6L,
                            Description = "Tech & Information Technologies"
                        },
                        new
                        {
                            ID = 29L,
                            Description = "Subscriptions",
                            ParentId = 6L
                        },
                        new
                        {
                            ID = 7L,
                            Description = "Other"
                        },
                        new
                        {
                            ID = 30L,
                            Description = "Insurances & Policies",
                            ParentId = 7L
                        },
                        new
                        {
                            ID = 31L,
                            Description = "Duties",
                            ParentId = 7L
                        },
                        new
                        {
                            ID = 8L,
                            Description = "Salary"
                        });
                });

            modelBuilder.Entity("Savings.Model.RecurrentMoneyItem", b =>
                {
                    b.Property<long>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<long?>("CategoryID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<int?>("MoneyAccountId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.Property<int>("OccurrencyType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RecurrencyInterval")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RecurrencyType")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("TimelineWeight")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("MoneyAccountId");

                    b.ToTable("RecurrentMoneyItems");
                });

            modelBuilder.Entity("Savings.Model.BudgetPlanRule", b =>
                {
                    b.HasOne("Savings.Model.BudgetPlan", null)
                        .WithMany("Rules")
                        .HasForeignKey("BudgetPlanId");

                    b.HasOne("Savings.Model.MoneyCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Savings.Model.FixedMoneyItem", b =>
                {
                    b.HasOne("Savings.Model.MoneyAccount", "Account")
                        .WithMany("FixedMoneyItems")
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Savings.Model.MoneyCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryID");

                    b.Navigation("Account");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Savings.Model.MaterializedMoneyItem", b =>
                {
                    b.HasOne("Savings.Model.MoneyCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryID");

                    b.HasOne("Savings.Model.FixedMoneyItem", "FixedMoneyItem")
                        .WithOne("MaterializedMoneyItem")
                        .HasForeignKey("Savings.Model.MaterializedMoneyItem", "FixedMoneyItemID");

                    b.HasOne("Savings.Model.RecurrentMoneyItem", "RecurrentMoneyItem")
                        .WithMany("MaterializedMoneyItems")
                        .HasForeignKey("RecurrentMoneyItemID");

                    b.Navigation("Category");

                    b.Navigation("FixedMoneyItem");

                    b.Navigation("RecurrentMoneyItem");
                });

            modelBuilder.Entity("Savings.Model.MoneyCategory", b =>
                {
                    b.HasOne("Savings.Model.MoneyCategory", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Savings.Model.RecurrentMoneyItem", b =>
                {
                    b.HasOne("Savings.Model.MoneyCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryID");

                    b.HasOne("Savings.Model.MoneyAccount", "MoneyAccount")
                        .WithMany("RecurrentMoneyItems")
                        .HasForeignKey("MoneyAccountId");

                    b.Navigation("Category");

                    b.Navigation("MoneyAccount");
                });

            modelBuilder.Entity("Savings.Model.BudgetPlan", b =>
                {
                    b.Navigation("Rules");
                });

            modelBuilder.Entity("Savings.Model.FixedMoneyItem", b =>
                {
                    b.Navigation("MaterializedMoneyItem");
                });

            modelBuilder.Entity("Savings.Model.MoneyAccount", b =>
                {
                    b.Navigation("FixedMoneyItems");

                    b.Navigation("RecurrentMoneyItems");
                });

            modelBuilder.Entity("Savings.Model.MoneyCategory", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("Savings.Model.RecurrentMoneyItem", b =>
                {
                    b.Navigation("MaterializedMoneyItems");
                });
#pragma warning restore 612, 618
        }
    }
}
