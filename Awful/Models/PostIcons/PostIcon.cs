// <copyright file="PostIcon.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Awful.Parser.Models.PostIcons
{
    public class PostIcon
    {
        public int Id { get; set; }

        public string ImageEndpoint { get; set; }

        public string Title { get; set; }

        public string ImageLocation { get; internal set; }
    }
}
