// <copyright file="Moderator.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Awful.Core.Models.JSON
{
    public class Moderator
    {
        [JsonPropertyName("userid")]
        public long Userid { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}
