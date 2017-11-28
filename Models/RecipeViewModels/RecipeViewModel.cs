using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBrewing.Models.RecipeViewModels
{
    public class RecipeViewModel
    {
        public CreateRecipeViewModel createRecipeViewModel { get; set; }
        public IngredientViewModel ingredientViewModel { get; set; }
    }
}
