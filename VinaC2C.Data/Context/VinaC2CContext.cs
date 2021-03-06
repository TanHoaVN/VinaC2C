﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VinaC2C.Data.Configurations;
using VinaC2C.Data.Models;

namespace VinaC2C.Data.Context
{
    public class VinaC2CContext : DbContext
    {
        public VinaC2CContext(DbContextOptions<VinaC2CContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new FeatureConfiguration());
            builder.ApplyConfiguration(new FeatureRoleConfiguration());
            builder.ApplyConfiguration(new DigitalShopConfiguration());
            builder.ApplyConfiguration(new DigitalShopRoleConfiguration());
            builder.ApplyConfiguration(new ServiceTicketConfiguration());
            builder.ApplyConfiguration(new ServiceTicketRoleConfiguration());
            builder.ApplyConfiguration(new UnitConfiguration());
            builder.ApplyConfiguration(new UnitConvertConfiguration());
            builder.ApplyConfiguration(new UserShopConfiguration());

            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                                                        .SetBasePath(Directory.GetCurrentDirectory())
                                                        .AddJsonFile("appsettings.json")
                                                        .Build();

            var connectionString = configurationRoot.GetConnectionString("VinaC2CContext");
            optionsBuilder.UseSqlServer(connectionString);
        }

        public DbSet<User> Users { get; set; } 
        public DbSet<FeatureRole> FeatureRoles { get; set; } 
        public DbSet<Feature> Features { get; set; } 
        public DbSet<DigitalShop> DigitalShops { get; set; } 
        public DbSet<DigitalShopRole> DigitalShopRoles { get; set; } 
        public DbSet<ServiceTicket> ServiceTickets { get; set; } 
        public DbSet<ServiceTicketRole> ServiceTicketRoles { get; set; } 
        public DbSet<Unit> Units { get; set; } 
        public DbSet<UnitConvert> UnitConverts { get; set; } 
        public DbSet<UserShop> UserShops { get; set; } 
    }
}
