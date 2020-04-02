using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Awful.Core.Models.JSON
{

    public partial class IndexPage
    {
        [JsonProperty("stats")]
        public Stats Stats { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("forums")]
        public List<Forum> Forums { get; set; }
    }

    public partial class Forum
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        public long ParentId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("title_short")]
        public string TitleShort { get; set; }

        [JsonProperty("has_threads")]
        public bool HasThreads { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("sub_forums")]
        public List<Forum> SubForums { get; set; }

        [JsonProperty("moderators")]
        public List<Moderator> Moderators { get; set; }
    }

    public partial class Moderator
    {
        [JsonProperty("userid")]
        public long Userid { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }

    public partial class Stats
    {
        [JsonProperty("archived_posts")]
        public long ArchivedPosts { get; set; }

        [JsonProperty("archived_threads")]
        public long ArchivedThreads { get; set; }

        [JsonProperty("banned_users")]
        public long BannedUsers { get; set; }

        [JsonProperty("banned_users_total")]
        public long BannedUsersTotal { get; set; }

        [JsonProperty("online_registered")]
        public long OnlineRegistered { get; set; }

        [JsonProperty("online_total")]
        public long OnlineTotal { get; set; }

        [JsonProperty("unique_posts")]
        public long UniquePosts { get; set; }

        [JsonProperty("unique_threads")]
        public long UniqueThreads { get; set; }

        [JsonProperty("usercount")]
        public long Usercount { get; set; }
    }

    public partial class User
    {
        [JsonProperty("userid")]
        public long Userid { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("homepage")]
        public string Homepage { get; set; }

        [JsonProperty("icq")]
        public string Icq { get; set; }

        [JsonProperty("aim")]
        public string Aim { get; set; }

        [JsonProperty("yahoo")]
        public string Yahoo { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("usertitle")]
        public string Usertitle { get; set; }

        [JsonProperty("joindate")]
        public long Joindate { get; set; }

        [JsonProperty("lastpost")]
        public long Lastpost { get; set; }

        [JsonProperty("posts")]
        public long Posts { get; set; }

        [JsonProperty("receivepm")]
        public long Receivepm { get; set; }

        [JsonProperty("postsperday")]
        public double Postsperday { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("biography")]
        public string Biography { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("interests")]
        public string Interests { get; set; }

        [JsonProperty("occupation")]
        public string Occupation { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }
    }
}
