using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeBrewing.Models.SubscriptionViewModels
{
    public class JoinedTableViewModel

    {
        public  string FollowerUserId { get; set; }
        public string FollowedUserId { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string UserName { get; set; }


    }
}
