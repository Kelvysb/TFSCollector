﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TFSCollector.Model;

namespace TFSCollector.Migrations
{
    [DbContext(typeof(WorkItemContext))]
    partial class WorkItemContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("TFSCollector.Model.WorkItemLocal", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AreaPath");

                    b.Property<string>("AssignedTo");

                    b.Property<string>("BoardColumn");

                    b.Property<bool>("BoardColumnDone");

                    b.Property<string>("ChangedBy");

                    b.Property<DateTime>("ChangedDate");

                    b.Property<string>("ClosedBy");

                    b.Property<DateTime>("ClosedDate");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("IterationPath");

                    b.Property<int>("Priority");

                    b.Property<string>("Reason");

                    b.Property<string>("ReproSteps");

                    b.Property<int>("Rev");

                    b.Property<string>("Severity");

                    b.Property<string>("State");

                    b.Property<DateTime>("StateChangeDate");

                    b.Property<string>("SystemInfo");

                    b.Property<string>("Tags");

                    b.Property<string>("TeamProject");

                    b.Property<string>("Title");

                    b.Property<string>("Url");

                    b.Property<string>("ValueArea");

                    b.Property<string>("WorkItemType");

                    b.HasKey("Id");

                    b.ToTable("WorkItens");
                });
#pragma warning restore 612, 618
        }
    }
}
