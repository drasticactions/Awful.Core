// <copyright file="SAclopediaPost.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Awful.Parser.Models.SAclopedia
{
    public class SAclopediaPost
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string PostHtml { get; set; }

        public DateTime PostedDate { get; set; }
    }
}
