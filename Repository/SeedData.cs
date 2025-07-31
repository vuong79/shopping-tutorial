using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;

namespace Shopping_Tutorial.Repository
{
    public class SeedData
    {
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();
            if (!_context.Categories.Any())
            {
                var categoryMacbook = new CategoryModel { Name = "MACBOOK", Slug = "macbook", Description = "MacBook category", Status = 1 };
                var categoryPC = new CategoryModel { Name = "PC", Slug = "pc", Description = "PC category", Status = 1 };
                var categoryApple = new CategoryModel { Name = "APPLE", Slug = "apple", Description = "Apple category", Status = 1 };
                var categorySamsung = new CategoryModel { Name = "SAMSUNG", Slug = "samsung", Description = "Samsung category", Status = 1 };

                var brandApple = new BrandModel { Name = "Apple", Slug = "apple", Description = "Apple brand", Status = 1 };
                var brandSamsung = new BrandModel { Name = "Samsung", Slug = "samsung", Description = "Samsung brand", Status = 1 };

                // Add vào context
                _context.Categories.AddRange(categoryMacbook, categoryPC, categoryApple, categorySamsung);
                _context.Brands.AddRange(brandApple, brandSamsung);
                _context.SaveChanges(); // Lưu để các object có ID

                _context.Products.AddRange(
                    new ProductModel
                    {
                        Name = "Macbook Pro",
                        Slug = "macbook-pro",
                        Description = "Macbook Pro is the best",
                        Image = "1.jpg",
                        Category = categoryMacbook,
                        Brand = brandApple,
                        Price = 1200
                    },
                    new ProductModel
                    {
                        Name = "Gaming PC",
                        Slug = "gaming-pc",
                        Description = "Gaming PC is the best",
                        Image = "1.jpg",
                        Category = categoryPC,
                        Brand = brandSamsung,
                        Price = 800
                    }
                );
                _context.SaveChanges();
            }
        }
    }
}