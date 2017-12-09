using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeBrewing.Models.SubscriptionViewModels
{
    public class UserInfoViewModel
    {

       [Required]
       public string Id { get; set; }
       [Required]
       public string Name { get; set; }
       [Required]
       public string Surname { get; set; }
    
       [Required]
       public int PrivateAccount { get; set; }
       


    }
}