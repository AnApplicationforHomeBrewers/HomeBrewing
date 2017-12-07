using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeBrewing.Models.SubscriptionViewModels
{
    public class UserInfoViewModel
    {


       
       [Key]
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       [Required]
       public int Id { get; set; }
       [Required]
       public string Name { get; set; }
       [Required]
       public string Surname { get; set; }
       


    }
}