using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeBrewing.Models.RecipeViewModels
{
    public class CommentViewModel
    {

       [Key]
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       [Required]
       public int Id { get; set; }

       [Required]
       public string Comment { get; set; }

       [Required]
       public int RecipeId { get; set; }

       [Required]
       public string UserId { get; set; }
       


    }
}