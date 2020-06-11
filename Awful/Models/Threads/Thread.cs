// <copyright file="Thread.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Awful.Parser.Models.Forums;
using Awful.Parser.Models.Posts;

namespace Awful.Parser.Models.Threads
{
    public class Thread
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public string ImageIconEndpoint { get; set; } = string.Empty;

        public string ImageIconLocation { get; set; }

        public string StoreImageIconEndpoint { get; set; }

        public string StoreImageIconLocation { get; set; }

        public string Author { get; set; }

        public long AuthorId { get; set; }

        public int ReplyCount { get; set; }

        public int ViewCount { get; set; }

        public decimal Rating { get; set; }

        public int TotalRatingVotes { get; set; }

        public string RatingImage { get; set; }

        public string RatingImageEndpoint { get; set; }

        public string KilledBy { get; set; }

        public long KilledById { get; set; }

        public DateTime KilledOn { get; set; }

        public bool IsArchived { get; set; }

        public bool IsSticky { get; set; }

        public bool IsNotified { get; set; }

        public bool IsLocked { get; set; }

        public bool IsLoggedIn { get; set; }

        public bool IsAnnouncement { get; set; }

        public bool HasBeenViewed { get; set; }

        public bool CanMarkAsUnread { get; set; }

        public int RepliesSinceLastOpened { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int ScrollToPost { get; set; }

        public string ScrollToPostString { get; set; }

        public string LoggedInUserName { get; set; }

        public int ThreadId { get; set; }

        public int ForumId { get; set; }

        public Forum ForumEntity { get; set; }

        public bool HasSeen { get; set; }

        public bool IsBookmark { get; set; }

        public string StarColor { get; set; }

        public string Html { get; set; }

        public bool IsPrivateMessage { get; set; }

        public int OrderNumber { get; set; }

        public List<Post> Posts { get; } = new List<Post>();

        public Thread Clone()
        {
            return this.MemberwiseClone() as Thread;
        }
    }
}
