using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using XUnitApi.Models;

namespace XUnitApi.Data;

public partial class ApiDbContext : DbContext
{
    public ApiDbContext()
    {
    }

    public ApiDbContext(DbContextOptions<ApiDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aotable> Aotables { get; set; }

    public virtual DbSet<Form> Forms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=VM-104; database=PAS;User Id=Training;Password=May2022#;Trusted_Connection=False;TrustServerCertificate=True;MultipleActiveResultSets=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Aotable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_AOTable_Id");

            entity.ToTable("AOTable");

            entity.HasIndex(e => e.Name, "ix_AOTable_Name");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Boundary).HasDefaultValueSql("((0))");
            entity.Property(e => e.Cache).HasDefaultValueSql("((0))");
            entity.Property(e => e.Comment).HasMaxLength(2048);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.History).HasDefaultValueSql("((0))");
            entity.Property(e => e.Identifier).HasDefaultValueSql("((0))");
            entity.Property(e => e.Log).HasDefaultValueSql("((0))");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Notify).HasDefaultValueSql("((0))");
            entity.Property(e => e.Type).HasMaxLength(128);
        });

        modelBuilder.Entity<Form>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_Form_Id");

            entity.ToTable("Form");

            entity.HasIndex(e => e.RatebookId, "ix_Form_RatebookId");

            entity.HasIndex(e => e.TableId, "ix_Form_TableId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AddChangeDeleteFlag)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.BtnCndAdd).HasMaxLength(128);
            entity.Property(e => e.BtnCndCopy).HasMaxLength(128);
            entity.Property(e => e.BtnCndDelete).HasMaxLength(128);
            entity.Property(e => e.BtnCndModify).HasMaxLength(128);
            entity.Property(e => e.BtnCndRenumber).HasMaxLength(128);
            entity.Property(e => e.BtnCndView).HasMaxLength(128);
            entity.Property(e => e.BtnCndViewDetail).HasMaxLength(128);
            entity.Property(e => e.BtnLblAdd).HasMaxLength(128);
            entity.Property(e => e.BtnLblCopy).HasMaxLength(128);
            entity.Property(e => e.BtnLblDelete).HasMaxLength(128);
            entity.Property(e => e.BtnLblModify).HasMaxLength(128);
            entity.Property(e => e.BtnLblRenumber).HasMaxLength(128);
            entity.Property(e => e.BtnLblView).HasMaxLength(128);
            entity.Property(e => e.BtnLblViewDetail).HasMaxLength(128);
            entity.Property(e => e.BtnResAdd).HasMaxLength(128);
            entity.Property(e => e.BtnResCopy).HasMaxLength(128);
            entity.Property(e => e.BtnResDelete).HasMaxLength(128);
            entity.Property(e => e.BtnResModify).HasMaxLength(128);
            entity.Property(e => e.BtnResRenumber).HasMaxLength(128);
            entity.Property(e => e.BtnResView).HasMaxLength(128);
            entity.Property(e => e.BtnResViewDetail).HasMaxLength(128);
            entity.Property(e => e.Comment).HasMaxLength(2048);
            entity.Property(e => e.Condition).HasColumnType("ntext");
            entity.Property(e => e.HelpText).HasColumnType("ntext");
            entity.Property(e => e.Hidden).HasDefaultValueSql("((0))");
            entity.Property(e => e.HidePremium).HasDefaultValueSql("((0))");
            entity.Property(e => e.MaxOccurs).HasDefaultValueSql("((99))");
            entity.Property(e => e.MinOccurs).HasDefaultValueSql("((0))");
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.Property(e => e.Number).HasMaxLength(128);
            entity.Property(e => e.SubSequence).HasDefaultValueSql("((1))");
            entity.Property(e => e.TabCondition).HasColumnType("ntext");
            entity.Property(e => e.TabResourceName).HasMaxLength(128);
            entity.Property(e => e.TemplateFile).HasMaxLength(128);
            entity.Property(e => e.Type).HasMaxLength(128);

            entity.HasOne(d => d.Table).WithMany(p => p.Forms)
                .HasForeignKey(d => d.TableId)
                .HasConstraintName("fk_Form_Table");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
