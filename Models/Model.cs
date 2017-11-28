using HomeBrewing.Models.RecipeViewModels;
using Microsoft.EntityFrameworkCore;

namespace HomeBrewing.Models
{
    public class DatabaseContext : DbContext
    {

        public DbSet<CreateRecipeViewModel> Recipe { get; set; }
        public DbSet<IngredientViewModel> Ingredient { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }

    }
}
