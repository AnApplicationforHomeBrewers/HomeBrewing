using HomeBrewing.Models.RecipeViewModels;
using HomeBrewing.Models.SubscriptionViewModels;
using Microsoft.EntityFrameworkCore;

namespace HomeBrewing.Models
{
    public class DatabaseContext : DbContext
    {

        public DbSet<CreateRecipeViewModel> Recipe { get; set; }
        public DbSet<IngredientViewModel> Ingredient { get; set; }
        public DbSet<SubscriptionInfoViewModel> Subscription { get; set; }
        public DbSet<UserInfoViewModel> AspNetUsers { get; set; }
        public DbSet<CommentViewModel> RecipeComment { get; set; }
        public DbSet<RecipeLikeViewModel> RecipeLike { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }


    }

    public class UserContext : DbContext
    {
        public DbSet<HomeBrewing.Models.AccountViewModels.PrivateAccountViewModel> AspNetUsers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }

    }

    }
