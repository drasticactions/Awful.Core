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
        public List<Forum> Forums { get; set; }
    }

    public partial class Forum
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

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
        public List<Forum> SubForums { get; set; }

        [JsonPropertyName("moderators")]
        public List<Moderator> Moderators { get; set; }
    }

    public partial class Moderator
    {
        [JsonPropertyName("userid")]
        public long Userid { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }
    }

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

    public partial class User
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
