// <copyright file="User.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Awful.Core.Models.JSON
{
    public class User
    {
        [JsonPropertyName("userid")]
        public long Userid { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("homepage")]
        public string Homepage { get; set; }

        [JsonPropertyName("icq")]
        public string Icq { get; set; }

        [JsonPropertyName("aim")]
        public string Aim { get; set; }

        [JsonPropertyName("yahoo")]
        public string Yahoo { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("usertitle")]
        public string Usertitle { get; set; }

        [JsonPropertyName("joindate")]
        public long Joindate { get; set; }

        [JsonPropertyName("lastpost")]
        public long Lastpost { get; set; }

        [JsonPropertyName("posts")]
        public long Posts { get; set; }

        [JsonPropertyName("receivepm")]
        public long Receivepm { get; set; }

        [JsonPropertyName("postsperday")]
        public double Postsperday { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("biography")]
        public string Biography { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("interests")]
        public string Interests { get; set; }

        [JsonPropertyName("occupation")]
        public string Occupation { get; set; }

        [JsonPropertyName("picture")]
        public string Picture { get; set; }
    }
}
