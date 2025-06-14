﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ServiPuntos.Infrastructure.Data;

#nullable disable

namespace ServiPuntos.Infrastructure.Migrations
{
    [DbContext(typeof(ServiPuntosDbContext))]
    [Migration("20250517222814_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PromocionUbicacion", b =>
                {
                    b.Property<Guid>("PromocionesId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UbicacionesId")
                        .HasColumnType("uuid");

                    b.HasKey("PromocionesId", "UbicacionesId");

                    b.HasIndex("UbicacionesId");

                    b.ToTable("PromocionUbicacion");
                });

            modelBuilder.Entity("ServiPuntos.Core.Entities.ProductoCanjeable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("CostoEnPuntos")
                        .HasColumnType("integer");

                    b.Property<string>("Descripcion")
                        .HasColumnType("text");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("ProductosCanjeables");
                });

            modelBuilder.Entity("ServiPuntos.Core.Entities.ProductoUbicacion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("Activo")
                        .HasColumnType("boolean");

                    b.Property<Guid>("ProductoCanjeableId")
                        .HasColumnType("uuid");

                    b.Property<int>("StockDisponible")
                        .HasColumnType("integer");

                    b.Property<Guid>("UbicacionId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ProductoCanjeableId");

                    b.HasIndex("UbicacionId");

                    b.ToTable("ProductoUbicaciones");
                });

            modelBuilder.Entity("ServiPuntos.Core.Entities.Promocion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Descripcion")
                        .HasColumnType("text");

                    b.Property<int?>("DescuentoEnPuntos")
                        .HasColumnType("integer");

                    b.Property<DateTime>("FechaFin")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("FechaInicio")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Promociones");
                });

            modelBuilder.Entity("ServiPuntos.Core.Entities.Tenant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Color")
                        .HasColumnType("text");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("FechaModificacion")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LogoUrl")
                        .HasColumnType("text");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("ValorPunto")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Tenants");
                });

            modelBuilder.Entity("ServiPuntos.Core.Entities.Ubicacion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("CambioAceite")
                        .HasColumnType("boolean");

                    b.Property<string>("Ciudad")
                        .HasColumnType("text");

                    b.Property<string>("Departamento")
                        .HasColumnType("text");

                    b.Property<string>("Direccion")
                        .HasColumnType("text");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("FechaModificacion")
                        .HasColumnType("timestamp with time zone");

                    b.Property<TimeSpan>("HoraApertura")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("HoraCierre")
                        .HasColumnType("interval");

                    b.Property<bool>("Lavado")
                        .HasColumnType("boolean");

                    b.Property<string>("Nombre")
                        .HasColumnType("text");

                    b.Property<decimal>("PrecioDiesel")
                        .HasColumnType("numeric");

                    b.Property<decimal>("PrecioNaftaPremium")
                        .HasColumnType("numeric");

                    b.Property<decimal>("PrecioNaftaSuper")
                        .HasColumnType("numeric");

                    b.Property<string>("Telefono")
                        .HasColumnType("text");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Ubicaciones");
                });

            modelBuilder.Entity("ServiPuntos.Core.Entities.Usuario", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Apellido")
                        .HasColumnType("text");

                    b.Property<int>("Ci")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("FechaModificacion")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("FechaNacimiento")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Nombre")
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Puntos")
                        .HasColumnType("integer");

                    b.Property<int>("Rol")
                        .HasColumnType("integer");

                    b.Property<int>("Telefono")
                        .HasColumnType("integer");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("PromocionUbicacion", b =>
                {
                    b.HasOne("ServiPuntos.Core.Entities.Promocion", null)
                        .WithMany()
                        .HasForeignKey("PromocionesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ServiPuntos.Core.Entities.Ubicacion", null)
                        .WithMany()
                        .HasForeignKey("UbicacionesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ServiPuntos.Core.Entities.ProductoUbicacion", b =>
                {
                    b.HasOne("ServiPuntos.Core.Entities.ProductoCanjeable", "ProductoCanjeable")
                        .WithMany("DisponibilidadesPorUbicacion")
                        .HasForeignKey("ProductoCanjeableId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ServiPuntos.Core.Entities.Ubicacion", "Ubicacion")
                        .WithMany("ProductosLocales")
                        .HasForeignKey("UbicacionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProductoCanjeable");

                    b.Navigation("Ubicacion");
                });

            modelBuilder.Entity("ServiPuntos.Core.Entities.Ubicacion", b =>
                {
                    b.HasOne("ServiPuntos.Core.Entities.Tenant", "Tenant")
                        .WithMany("Ubicaciones")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ServiPuntos.Core.Entities.Usuario", b =>
                {
                    b.HasOne("ServiPuntos.Core.Entities.Tenant", "Tenant")
                        .WithMany("Usuarios")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tenant");
                });

            modelBuilder.Entity("ServiPuntos.Core.Entities.ProductoCanjeable", b =>
                {
                    b.Navigation("DisponibilidadesPorUbicacion");
                });

            modelBuilder.Entity("ServiPuntos.Core.Entities.Tenant", b =>
                {
                    b.Navigation("Ubicaciones");

                    b.Navigation("Usuarios");
                });

            modelBuilder.Entity("ServiPuntos.Core.Entities.Ubicacion", b =>
                {
                    b.Navigation("ProductosLocales");
                });
#pragma warning restore 612, 618
        }
    }
}
