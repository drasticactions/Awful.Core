// <copyright file="PrivateMessage.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Awful.Parser.Models.PostIcons;
using Awful.Parser.Models.Posts;

namespace Awful.Parser.Models.Messages
{
    public class PrivateMessage
    {
        public int Id { get; set; }

        public PostIcon Icon { get; set; }

        public string ImageIconLocation { get; set; }

        public string Title { get; set; }

        public string Sender { get; set; }

        public DateTime Date { get; set; }

        public Post Post { get; set; }

        public string MessageEndpoint { get; set; }

        public string ImageIconEndpoint { get; internal set; }

        public string StatusImageIconEndpoint { get; internal set; }

        public string StatusImageIconLocation { get; internal set; }
    }
}
