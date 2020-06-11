// <copyright file="Forum.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Awful.Core.Models.JSON
{
    public partial class Forum
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        public bool IsFavorite { get; set; }

        public int SortOrder { get; set; }

        public long ParentId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("title_short")]
        public string TitleShort { get; set; }

        [JsonPropertyName("has_threads")]
        public bool HasThreads { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("sub_forums")]
        public List<Forum> SubForums { get; }

        [JsonPropertyName("moderators")]
        public List<Moderator> Moderators { get; }
    }
}
