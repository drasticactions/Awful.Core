// <copyright file="ThreadHandler.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Awful.Exceptions;
using Awful.Parser.Models.Posts;
using Awful.Parser.Models.Threads;
using Awful.Parser.Models.Users;

namespace Awful.Parser.Handlers
{
    /// <summary>
    /// Handles SAclopedia Elements.
    /// </summary>
    public static class ThreadHandler
    {
        /// <summary>
        /// Checks if the current IHtmlDocument contains a paywall message.
        /// Throws a PaywallException if the content is paywalled.
        /// </summary>
        /// <param name="doc">IHtmlDocument containing the SA Page.</param>
        public static void CheckPaywall(IHtmlDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            var test = doc.QuerySelector(".inner");
            if (test != null)
            {
                if (test.TextContent.Contains("Sorry, you must be a registered forums member to view this page."))
                {
                    throw new PaywallException(Awful.Core.Resources.ExceptionMessages.PaywallThreadHit);
                }
            }
        }

        /// <summary>
        /// Parses the IHtmlDocument for a forum thread list.
        /// </summary>
        /// <param name="doc">IHtmlDocument containing the Forum List.</param>
        /// <returns>List of Threads.</returns>
        public static List<Thread> ParseForumThreadList(IHtmlDocument doc)
        {
            CheckPaywall(doc);
            var forumThreadList = new List<Thread>();
            var threadTableList = doc.QuerySelector("#forum");
            if (threadTableList == null)
            {
                throw new FormatException(Awful.Core.Resources.ExceptionMessages.ThreadListMissing);
            }

            var rows = threadTableList.QuerySelectorAll("tr");

            foreach (var row in rows)
            {
                if (row.Id == null)
                {
                    continue;
                }

                var thread = new Thread
                {
                    ThreadId = Convert.ToInt32(row.Id.Replace("thread", string.Empty), CultureInfo.InvariantCulture),
                };
                ParseStar(row.QuerySelector(".star"), thread);
                ParseIcon(row.QuerySelector(".icon"), thread);
                ParseIcon2(row.QuerySelector(".icon2"), thread);
                ParseTitle(row.QuerySelector(".title"), thread);
                ParseAuthor(row.QuerySelector(".author"), thread);
                ParseReplies(row.QuerySelector(".replies"), thread);
                ParseViews(row.QuerySelector(".views"), thread);
                ParseRating(row.QuerySelector(".rating"), thread);
                ParseLastPost(row.QuerySelector(".lastpost"), thread);
                ParseLastSeen(row.QuerySelector(".lastseen"), thread);
                forumThreadList.Add(thread);
            }

            return forumThreadList;
        }

        /// <summary>
        /// Parses the content of a new thread page to get info about it.
        /// </summary>
        /// <param name="document">The IHtmlDocument of the New Thread page.</param>
        /// <returns>NewThread.</returns>
        public static NewThread ParseNewThread(IHtmlDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            return new NewThread
            {
                FormKey = document.QuerySelector(@"input[name=""formkey""]").GetAttribute("value"),
                FormCookie = document.QuerySelector(@"input[name=""form_cookie""]").GetAttribute("value"),
            };
        }

        /// <summary>
        /// Parses a thread for posts.
        /// </summary>
        /// <param name="doc">An IHtmlDocument with the thread posts.</param>
        /// <param name="responseEndpoint">The endpoint of the response.</param>
        /// <returns>A thread.</returns>
        public static Thread ParseThread(IHtmlDocument doc, string responseEndpoint = "")
        {
            return ParseThread(doc, new Thread(), responseEndpoint);
        }

        /// <summary>
        /// Parses a thread for posts.
        /// </summary>
        /// <param name="doc">An IHtmlDocument with the thread posts.</param>
        /// <param name="thread">The Thread.</param>
        /// <param name="responseEndpoint">The endpoint of the response.</param>
        /// <returns>A thread.</returns>
        public static Thread ParseThread(IHtmlDocument doc, Thread thread, string responseEndpoint = "")
        {
            if (thread == null)
            {
                throw new ArgumentNullException(nameof(thread));
            }

            CheckPaywall(doc);
            ParseArchive(doc, thread);
            ParseThreadInfo(doc, thread, responseEndpoint);
            ParseThreadPageNumbers(doc, thread);
            ParseThreadPage(doc, thread);
            ParseThreadPosts(doc, thread);
            return thread;
        }

        /// <summary>
        /// Parses the previous posts shown when making a new post.
        /// </summary>
        /// <param name="doc">The IHtmlDocument of a new post page.</param>
        /// <returns>List of Posts.</returns>
        public static List<Post> ParsePreviousPosts(IHtmlDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            var posts = new List<Post>();
            var threadDivTableHolder = doc.QuerySelector("#thread");
            foreach (var threadTable in threadDivTableHolder.QuerySelectorAll("table"))
            {
                if (string.IsNullOrEmpty(threadTable.GetAttribute("data-idx")))
                {
                    continue;
                }

                posts.Add(PostHandler.ParsePost(doc, threadTable));
            }

            return posts;
        }

        private static void ParseThreadPosts(IHtmlDocument doc, Thread thread)
        {
            var threadDivTableHolder = doc.QuerySelector("#thread");
            foreach (var threadTable in threadDivTableHolder.QuerySelectorAll("table.post"))
            {
                if (string.IsNullOrEmpty(threadTable.Id.Replace("post", string.Empty)))
                {
                    continue;
                }

                thread.Posts.Add(PostHandler.ParsePost(doc, threadTable));
            }
        }

        private static void ParseThreadPage(IHtmlDocument doc, Thread thread)
        {
            thread.Name = doc.Title.Replace(" - The Something Awful Forums", string.Empty);
            var threadBody = doc.QuerySelector("body");
            thread.ThreadId = Convert.ToInt32(threadBody.GetAttribute("data-thread"), CultureInfo.InvariantCulture);
            thread.ForumId = Convert.ToInt32(threadBody.GetAttribute("data-forum"), CultureInfo.InvariantCulture);
        }

        private static void ParseThreadInfo(IHtmlDocument doc, Thread thread, string responseUri = "")
        {
            thread.LoggedInUserName = doc.QuerySelector("#loggedinusername").TextContent;
            thread.IsLoggedIn = thread.LoggedInUserName != "Unregistered Faggot";
            if (string.IsNullOrEmpty(responseUri))
            {
                return;
            }

            string[] test = responseUri.Split('#');

            if (test.Length > 1 && test[1].Contains("pti"))
            {
                thread.ScrollToPost = int.Parse(Regex.Match(responseUri.Split('#')[1], @"\d+").Value, CultureInfo.InvariantCulture) - 1;
                thread.ScrollToPostString = string.Concat("#", responseUri.Split('#')[1]);
            }
        }

        private static void ParseArchive(IHtmlDocument doc, Thread thread)
        {
            var archiveButton = doc.QuerySelector(@"img[src*=""button-archive""]");
            if (archiveButton != null)
            {
                thread.IsArchived = true;
            }
        }

        private static void ParseStar(IElement element, Thread thread)
        {
            if (element == null)
            {
                return;
            }

            var starColor = element.ClassList.FirstOrDefault(n => n.Contains("bm") && !n.Contains("bm-1"));
            if (string.IsNullOrEmpty(starColor))
            {
                return;
            }

            thread.StarColor = starColor;
            thread.IsBookmark = true;
        }

        private static void ParseLastSeen(IElement element, Thread thread)
        {
            if (element == null)
            {
                return;
            }

            thread.HasSeen = true;
            var count = element.QuerySelector(".count");
            if (count == null)
            {
                return;
            }

            thread.RepliesSinceLastOpened = Convert.ToInt32(count.TextContent, CultureInfo.InvariantCulture);
        }

        private static void ParseIcon(IElement element, Thread thread)
        {
            if (element == null)
            {
                return;
            }

            var img = element.QuerySelector("img");
            thread.ImageIconUrl = img.GetAttribute("src");
            thread.ImageIconLocation = Path.GetFileNameWithoutExtension(thread.ImageIconUrl);
        }

        private static void ParseIcon2(IElement element, Thread thread)
        {
            if (element == null)
            {
                return;
            }

            var img = element.QuerySelector("img");
            thread.StoreImageIconUrl = img.GetAttribute("src");
            thread.StoreImageIconLocation = Path.GetFileNameWithoutExtension(thread.StoreImageIconUrl);
        }

        private static void ParseTitle(IElement element, Thread thread)
        {
            if (element == null)
            {
                return;
            }

            if (element.ClassList.Contains("title_sticky"))
            {
                thread.IsSticky = true;
            }

            var threadList = element.QuerySelector(".thread_title");
            thread.Name = threadList.TextContent;
        }

        private static void ParseAuthor(IElement element, Thread thread)
        {
            if (element == null)
            {
                return;
            }

            var authorLink = element.QuerySelector("a");
            var user = new User
            {
                Id = Convert.ToInt64(authorLink.GetAttribute("href").Split('=').Last(), CultureInfo.InvariantCulture),
                Username = authorLink.TextContent,
            };
            thread.Author = user.Username;
            thread.AuthorId = user.Id;
        }

        private static void ParseReplies(IElement element, Thread thread)
        {
            if (element == null)
            {
                return;
            }

            thread.ReplyCount = Convert.ToInt32(element.TextContent, CultureInfo.InvariantCulture);
            thread.TotalPages = (thread.ReplyCount / 40) + 1;
        }

        private static void ParseViews(IElement element, Thread thread)
        {
            if (element == null)
            {
                return;
            }

            thread.ViewCount = Convert.ToInt32(element.TextContent, CultureInfo.InvariantCulture);
        }

        private static void ParseRating(IElement element, Thread thread)
        {
            if (element == null || element.ChildElementCount <= 0)
            {
                return;
            }

            var img = element.QuerySelector("img");
            thread.RatingImageUrl = img.GetAttribute("src");
            thread.RatingImage = Path.GetFileNameWithoutExtension(thread.RatingImageUrl);
            var firstSplit = img.GetAttribute("title").Split('-');
            thread.TotalRatingVotes = Convert.ToInt32(Regex.Match(firstSplit[0], @"\d+").Value, CultureInfo.InvariantCulture);
            thread.Rating = Convert.ToDecimal(Regex.Match(firstSplit[1], @"[\d]{1,4}([.,][\d]{1,2})?").Value, CultureInfo.InvariantCulture);
        }

        private static void ParseLastPost(IElement element, Thread thread)
        {
            if (element == null)
            {
                return;
            }

            var date = element.QuerySelector(".date");
            var author = element.QuerySelector(".author");
            var user = new User();
            thread.KilledOn = DateTime.Parse(date.TextContent, CultureInfo.InvariantCulture);
            user.Id = Convert.ToInt64(author.GetAttribute("href").Split('=').Last(), CultureInfo.InvariantCulture);
            user.Username = author.TextContent;
            thread.KilledById = user.Id;
            thread.KilledBy = user.Username;
        }

        private static void ParseThreadPageNumbers(IHtmlDocument doc, Thread thread)
        {
            var pages = doc.QuerySelector(".pages");
            if (pages == null)
            {
                thread.CurrentPage = 1;
                thread.TotalPages = 1;
                return;
            }

            var select = pages.QuerySelector("select");
            if (select == null)
            {
                thread.CurrentPage = 1;
                thread.TotalPages = 1;
                return;
            }

            var selectedPageItem = select.QuerySelector("option:checked");
            thread.CurrentPage = Convert.ToInt32(selectedPageItem.TextContent, CultureInfo.InvariantCulture);
            thread.TotalPages = select.ChildElementCount;
        }
    }
}
