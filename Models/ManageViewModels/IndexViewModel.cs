﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBrewing.Models.ManageViewModels
{
    public class IndexViewModel
    {
        
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public int PrivateAccount { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        public string StatusMessage { get; set; }
    }
}
