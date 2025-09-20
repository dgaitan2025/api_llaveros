using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace parcial2.Models;

public partial class dbparcialContext : DbContext
{
    public dbparcialContext(DbContextOptions<dbparcialContext> options)
        : base(options)
    {
    }

    public virtual DbSet<vehiculo> vehiculos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<vehiculo>(entity =>
        {
            entity.HasKey(e => e.idvehiculo).HasName("PRIMARY");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
