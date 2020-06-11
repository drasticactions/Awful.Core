// <copyright file="Stats.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Awful.Core.Models.JSON
{
    public partial class Stats
    {
        [JsonPropertyName("archived_posts")]
        public long ArchivedPosts { get; set; }

        [JsonPropertyName("archived_threads")]
        public long ArchivedThreads { get; set; }

        [JsonPropertyName("banned_users")]
        public long BannedUsers { get; set; }

        [JsonPropertyName("banned_users_total")]
        public long BannedUsersTotal { get; set; }

        [JsonPropertyName("online_registered")]
        public long OnlineRegistered { get; set; }

        [JsonPropertyName("online_total")]
        public long OnlineTotal { get; set; }

        [JsonPropertyName("unique_posts")]
        public long UniquePosts { get; set; }

        [JsonPropertyName("unique_threads")]
        public long UniqueThreads { get; set; }

        [JsonPropertyName("usercount")]
        public long Usercount { get; set; }
    }
}
