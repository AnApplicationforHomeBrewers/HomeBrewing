using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeBrewing.Models.SubscriptionViewModels
{
    public class SubscriptionInfoViewModel
    {

       [Key]
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       [Required]
       public int Id { get; set; }

       [Required]
       public string FollowerUserID { get; set; }

       [Required]
       public string FollowedUserID { get; set; }

       [Required]
       public int Status { get; set; }
       


    }
}