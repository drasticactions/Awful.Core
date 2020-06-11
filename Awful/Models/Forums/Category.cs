// <copyright file="Category.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Awful.Parser.Models.Forums
{
    public class Category
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public int Id { get; set; }

        public int Order { get; set; }

        [JsonIgnore]
        public List<Forum> ForumList { get; } = new List<Forum>();
    }
}
