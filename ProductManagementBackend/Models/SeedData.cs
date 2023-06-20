using Microsoft.EntityFrameworkCore;


namespace ProductManagementBackend.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ProductManagementContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ProductManagementContext>>()))
            {
                // Look for any products.
                if (context.Products.Any())
                {
                    return;   // DB has been seeded
                }

                context.Products.AddRange(
                    new Product
                    {
                        Title = "PocketBook",
                        Description = "Electronic book reader",
                        Price = 199.99M
                    },

                    new Product
                    {
                        Title = "Mac Laptop",
                        Description = "The best laptop",
                        Price = 999.99M
                    },

                    new Product
                    {
                        Title = "Harry Potter",
                        Description = "Harry Potter book",
                        Price = 39.99M
                    }
                );

                // Look for any users.
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }

                context.Users.Add(new User
                {
                    Name = "Test",
                    Email = "test@gradspace.org",
                    Password = "qwer1234"
                });

                context.SaveChanges();
            }
        }
    }
}
