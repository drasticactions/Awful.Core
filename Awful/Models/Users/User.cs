using System;
using System.Collections.Generic;
using System.Text;

namespace Awful.Models.Users
{
    public class User
    {
        public string Username { get; set; }

        public string AvatarLink { get; set; }

        public string AvatarTitle { get; set; }

        public string AvatarHtml { get; set; }

        public DateTime DateJoined { get; set; }

        public bool IsMod { get; set; }

        public bool IsAdmin { get; set; }

        public string Roles { get; set; }

        public bool IsCurrentUserPost { get; set; }

        public long Id { get; set; }
    }
}
