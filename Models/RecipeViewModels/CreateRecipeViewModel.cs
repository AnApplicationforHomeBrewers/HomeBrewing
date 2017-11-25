using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeBrewing.Models.RecipeViewModels
{
    public class CreateRecipeViewModel
    {
       
       [Required]
       public string UserId { get; set; }
       [Key]
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       [Required]
       public int Id { get; set; }
       [Required]
       public string Title { get; set; }
       [Required]
       public string Requirements { get; set; }
       [Required]
       public string Details { get; set; }
        
       public DateTime CreatedDate { get; set; }
        
      public DateTime EditedDate { get; set; }
    }
}
