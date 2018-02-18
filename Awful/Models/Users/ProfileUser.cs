using System;
using System.Collections.Generic;
using System.Text;

namespace Awful.Models.Users
{
    public class ProfileUser
    {
        public string Username { get; set; }

        public string AvatarLink { get; set; }

        public string UserPicLink { get; set; }

        public DateTime DateJoined { get; set; }

        public string IcqContactString { get; set; }

        public string AimContactString { get; set; }

        public string YahooContactString { get; set; }

        public string HomePageString { get; set; }

        public int PostCount { get; set; }

        public string LastPostDate { get; set; }

        public string Location { get; set; }

        public string AboutUser { get; set; }

        public long Id { get; set; }

        public string PostRate { get; set; }

        public string SellerRating { get; set; }
    }
}
