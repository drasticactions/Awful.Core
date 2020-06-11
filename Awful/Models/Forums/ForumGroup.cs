// <copyright file="ForumGroup.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Awful.Parser.Models.Forums
{
    public class ForumGroup : List<Forum>
    {
        public ForumGroup(Category category, List<Forum> forums)
            : base(forums)
        {
            this.Category = category;
        }

        public Category Category { get; private set; }
    }
}
