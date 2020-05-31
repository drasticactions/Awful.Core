﻿// <copyright file="PostHandler.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Net;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Awful.Parser.Models.Posts;

namespace Awful.Parser.Handlers
{
    /// <summary>
    /// Handles Something Awful Post Elements.
    /// </summary>
    public static class PostHandler
    {
        /// <summary>
        /// Parses an SA Posts IElement.
        /// </summary>
        /// <param name="parent">The posts parent document.</param>
        /// <param name="doc">The post's element.</param>
        /// <param name="wrapImgurGifs">Scans and wraps a posts Imgur embedded GIFs with lighter weight placeholders.</param>
        /// <returns>An SA Post.</returns>
        public static Post ParsePost(IHtmlDocument parent, IElement doc, bool wrapImgurGifs = true)
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            var post = new Post();
            post.User = UserHandler.ParseUserFromPost(doc);
            post.PostId = Convert.ToInt64(doc.Id.Replace("post", string.Empty), CultureInfo.InvariantCulture);
            var authorTd = doc.QuerySelector(@"[class*=""userid""]");
            authorTd.Remove();

            post.HasSeen = doc.QuerySelector(@"[class=""seen1""]") != null || doc.QuerySelector(@"[class=""seen2""]") != null;

            var postDate = doc.QuerySelector(@"td[class=""postdate""]");
            if (postDate != null)
            {
                post.PostDate = postDate.Text().Replace("\n", string.Empty).Replace("#", string.Empty).Replace("?", string.Empty);
            }

            var threadBody = doc.QuerySelector(".postbody");
            if (threadBody != null)
            {
                var jerkBody = threadBody.QuerySelector(@"a[title=""DON'T DO IT!!""]");
                if (jerkBody != null)
                {
                    post.IsIgnored = true;
                }
                else
                {
                    if (wrapImgurGifs)
                    {
                        var imgurGifs = threadBody.QuerySelectorAll(@"[src*=""imgur.com""][src*="".gif""]");
                        for (var i = 0; i < imgurGifs.Length; i++)
                        {
                            var imgurGif = imgurGifs[i];
                            var div = parent.CreateElement("div");
                            div.ClassList.Add("gifWrap");
                            var newImgur = parent.CreateElement("img");
                            newImgur.ClassList.Add("imgurGif");
                            newImgur.SetAttribute("data-originalurl", imgurGif.GetAttribute("src"));
                            newImgur.SetAttribute("data-posterurl", imgurGif.GetAttribute("src").Replace(".gif", "h.jpg"));
                            newImgur.SetAttribute("src", imgurGif.GetAttribute("src").Replace(".gif", "h.jpg"));
                            div.AppendChild(newImgur);
                            imgurGif.Replace(div);
                        }
                    }

                    var attachments = threadBody.QuerySelectorAll(@"[src*=""attachment.php""]");
                    foreach (var attachment in attachments)
                    {
                        attachment.SetAttribute("src", $"https://forums.somethingawful.com/{attachment.Attributes["src"].Value}");
                    }
                }

                post.PostHtml = HtmlEncode(threadBody.InnerHtml);
            }

            return post;
        }

        /// <summary>
        /// Parses a SA post preview element.
        /// </summary>
        /// <param name="doc">SA Post IHtmlDocument.</param>
        /// <returns>An SA Post.</returns>
        public static Post ParsePostPreview(IHtmlDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            var post = new Post();

            var threadBody = doc.QuerySelector(".postbody");
            if (threadBody != null)
            {
                post.PostHtml = threadBody.OuterHtml;
            }

            return post;
        }

        /// <summary>
        /// Encodes a SA Posts HTML.
        /// In order to get Unicode characters fully working, we need to first encode the entire post.
        /// THEN we decode the bits we can safely pass in, like single/double quotes.
        /// If we don't, the post format will be screwed up.
        /// </summary>
        /// <param name="text">The posts text.</param>
        /// <returns>An SA HTML encoded string.</returns>
        private static string HtmlEncode(string text)
        {
            char[] chars = WebUtility.HtmlEncode(text).ToCharArray();
            var result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            foreach (char c in chars)
            {
                int value = Convert.ToInt32(c);
                if (value > 127)
                {
                    result.AppendFormat(CultureInfo.InvariantCulture, "&#{0};", value);
                }
                else
                {
                    result.Append(c);
                }
            }

            result.Replace("&quot;", "\"");
            result.Replace("&#39;", @"'");
            result.Replace("&lt;", @"<");
            result.Replace("&gt;", @">");
            return result.ToString();
        }
    }
}
