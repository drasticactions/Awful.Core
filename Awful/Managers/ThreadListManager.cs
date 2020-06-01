// <copyright file="ThreadListManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Forums;
using Awful.Parser.Models.Threads;
using Thread = Awful.Parser.Models.Threads.Thread;

namespace Awful.Parser.Managers
{
    /// <summary>
    /// Manager for handling Thread Lists on Something Awful.
    /// </summary>
    public class ThreadListManager
    {
        private readonly WebClient webManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadListManager"/> class.
        /// </summary>
        /// <param name="webManager">The SA WebClient.</param>
        public ThreadListManager(WebClient webManager)
        {
            this.webManager = webManager;
        }

        /// <summary>
        /// Gets the list of threads in a given Forum.
        /// </summary>
        /// <param name="forumId">The Forum Id.</param>
        /// <param name="page">The page of the forum to get.</param>
        /// <param name="token">A CancellationToken.</param>
        /// <returns>A ThreadList.</returns>
        public async Task<ThreadList> GetForumThreadListAsync(int forumId, int page, CancellationToken token = default)
        {
            var pageUrl = string.Format(CultureInfo.InvariantCulture, EndPoints.ForumPage, forumId) + string.Format(CultureInfo.InvariantCulture, EndPoints.PageNumber, page);
            var result = await this.webManager.GetDataAsync(pageUrl, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            var threadList = new ThreadList();
            ForumHandler.GetForumPageInfo(document, threadList);
            threadList.Threads = ThreadHandler.ParseForumThreadList(document);
            return threadList;
        }
    }
}
