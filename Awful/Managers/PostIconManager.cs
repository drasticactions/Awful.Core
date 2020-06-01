﻿// <copyright file="PostIconManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Awful.Exceptions;
using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.PostIcons;

namespace Awful.Parser.Managers
{
    /// <summary>
    /// Manager for Post Icons on Something Awful.
    /// </summary>
    public class PostIconManager
    {
        private readonly WebClient webManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostIconManager"/> class.
        /// </summary>
        /// <param name="webManager">The SA WebClient.</param>
        public PostIconManager(WebClient webManager)
        {
            this.webManager = webManager;
        }

        /// <summary>
        /// Gets post icons for Threads.
        /// </summary>
        /// <param name="forumId">The Forum Id.</param>
        /// <param name="token">A CancellationToken.</param>
        /// <returns>A list of PostIcon's for Threads.</returns>
        public async Task<List<PostIcon>> GetForumPostIconsAsync(int forumId = 0, CancellationToken token = default)
        {
            return await this.GetPostIcons_InternalAsync(false, forumId, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets post icons for Private Messages.
        /// </summary>
        /// <param name="token">A CancellationToken.</param>
        /// <returns>A list of PostIcon's that can be used for Private Messages.</returns>
        public async Task<List<PostIcon>> GetPrivateMessagePostIconsAsync(CancellationToken token = default)
        {
            return await this.GetPostIcons_InternalAsync(true, 0, token).ConfigureAwait(false);
        }

        private async Task<List<PostIcon>> GetPostIcons_InternalAsync(bool isPrivateMessage = false, int forumId = 0, CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            string url = isPrivateMessage ? EndPoints.NewPrivateMessageBase : string.Format(CultureInfo.InvariantCulture, EndPoints.NewThread, forumId);
            var result = await this.webManager.GetDataAsync(url, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return PostIconHandler.ParsePostIconList(document);
        }
    }
}
