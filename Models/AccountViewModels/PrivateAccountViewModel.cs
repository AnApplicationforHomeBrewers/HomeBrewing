using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBrewing.Models.AccountViewModels
{
    public class PrivateAccountViewModel
    {
        [Key]
        [Required]
        public string UserName { get; set; }
        [Required]
        public int PrivateAccount { get; set; }
        [Required]
        public string Id { get; set; }

    }
}
