﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;

namespace Shopping_Tutorial.Repository
{
    public class DataContext : IdentityDbContext<AppUserModel>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {
        }

        public DbSet<ProductModel> Products { get; set; }
        public DbSet<BrandModel> Brands { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
    }

}
