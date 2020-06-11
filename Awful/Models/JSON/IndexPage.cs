// <copyright file="IndexPage.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Awful.Core.Models.JSON
{
    public partial class IndexPage
    {
        [JsonPropertyName("stats")]
        public Stats Stats { get; set; }

        [JsonPropertyName("user")]
        public User User { get; set; }

        [JsonPropertyName("forums")]
        public List<Forum> Forums { get; }
    }
}
