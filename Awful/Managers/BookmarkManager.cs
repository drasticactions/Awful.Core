﻿// <copyright file="BookmarkManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Awful.Exceptions;
using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Web;

namespace Awful.Parser.Managers
{
    /// <summary>
    /// Manager for Something Awful Bookmarks.
    /// </summary>
    public class BookmarkManager
    {
        private readonly WebClient webManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkManager"/> class.
        /// </summary>
        /// <param name="webManager">The SA WebClient.</param>
        public BookmarkManager(WebClient webManager)
        {
            this.webManager = webManager;
        }

        /// <summary>
        /// Gets all the bookmarks for a given authenticated user.
        /// </summary>
        /// <param name="perPage">Amount of bookmarked threads to gather, default is 40.</param>
        /// <param name="token">A CancellationToken.</param>
        /// <returns>List of Threads.</returns>
        public async Task<List<Models.Threads.Thread>> GetAllBookmarksAsync(int perPage = 40, CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var threadList = new List<Models.Threads.Thread>();
            var page = 1;
            while (true)
            {
                var threads = await this.GetBookmarkListAsync(page, perPage, token).ConfigureAwait(false);
                if (!threads.Any())
                {
                    break;
                }

                threadList.AddRange(threads);
                page++;
            }

            return threadList;
        }

        /// <summary>
        /// Gets a single page of Bookmarked Threads for the authenticated user.
        /// </summary>
        /// <param name="page">The bookmark page number.</param>
        /// <param name="perPage">Amount of bookmarked threads to gather, default is 40.</param>
        /// <param name="token">A CancellationToken.</param>
        /// <returns>List of Threads.</returns>
        public async Task<List<Models.Threads.Thread>> GetBookmarkListAsync(int page, int perPage = 40, CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            string url = EndPoints.BookmarksUrl;
            if (page >= 0)
            {
                url = string.Format(CultureInfo.InvariantCulture, EndPoints.BookmarksUrl, perPage) + string.Format(CultureInfo.InvariantCulture, EndPoints.PageNumber, page);
            }

            var result = await this.webManager.GetDataAsync(url, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return ThreadHandler.ParseForumThreadList(document);
        }

        /// <summary>
        /// Add a new bookmark.
        /// </summary>
        /// <param name="threadId">The Thread Id.</param>
        /// <param name="token">A CancellationToken.</param>
        /// <returns>A SA Request Result.</returns>
        public async Task<Result> AddBookmarkAsync(long threadId, CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var dic = new Dictionary<string, string>
            {
                ["json"] = "1",
                ["action"] = "add",
                ["threadid"] = threadId.ToString(CultureInfo.InvariantCulture),
            };
            using var header = new FormUrlEncodedContent(dic);
            return await this.webManager.PostDataAsync(EndPoints.Bookmark, header, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Removes a bookmark.
        /// </summary>
        /// <param name="threadId">The Thread Id.</param>
        /// <param name="token">A CancellationToken.</param>
        /// <returns>A SA Request Result.</returns>
        public async Task<Result> RemoveBookmarkAsync(long threadId, CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var dic = new Dictionary<string, string>
            {
                ["json"] = "1",
                ["action"] = "remove",
                ["threadid"] = threadId.ToString(CultureInfo.InvariantCulture),
            };
            using var header = new FormUrlEncodedContent(dic);
            return await this.webManager.PostDataAsync(EndPoints.Bookmark, header, token).ConfigureAwait(false);
        }
    }
}
