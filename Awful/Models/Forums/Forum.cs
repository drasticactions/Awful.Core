using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Awful.Models.Forums
{
    public class Forum
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public int CurrentPage { get; set; }

        public bool IsSubforum { get; set; }

        public int TotalPages { get; set; }

        public int ForumId { get; set; }

        public int ForumOrder { get; set; }

        public int CategoryId { get; set; }

        [JsonIgnore]

        public Category Category { get; set; }

        public bool IsInUserBookmarks { get; set; }
    }
}
