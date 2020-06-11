﻿// <copyright file="Post.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Awful.Parser.Models.Users;

namespace Awful.Parser.Models.Posts
{
    public class Post
    {
        public User User { get; set; }

        public string PostDate { get; set; }

        public string PostReportLink { get; set; }

        public string PostQuoteLink { get; set; }

        public string PostLink { get; set; }

        public string PostFormatted { get; set; }

        public string PostHtml { get; set; }

        public string PostMarkdown { get; set; }

        public long PostId { get; set; }

        public long PostIndex { get; set; }

        public string PostDivString { get; set; }

        public bool HasSeen { get; set; }

        public bool IsQuoting { get; set; }

        public bool IsIgnored { get; set; }
    }
}
