// <copyright file="PostIconCategory.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Awful.Parser.Models.PostIcons
{
    public class PostIconCategory
    {
        public PostIconCategory(string category, List<PostIcon> list)
        {
            this.List = list;
            this.Category = category;
        }

        public virtual ICollection<PostIcon> List { get; private set; }

        public string Category { get; private set; }
    }
}
