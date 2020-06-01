﻿// <copyright file="SmileManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Smilies;

namespace Awful.Parser.Managers
{
    /// <summary>
    /// Manager for handling the Smiles used on Something Awful.
    /// </summary>
    public class SmileManager
    {
        private readonly WebClient webManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmileManager"/> class.
        /// </summary>
        /// <param name="webManager">The SA WebClient.</param>
        public SmileManager(WebClient webManager)
        {
            this.webManager = webManager;
        }

        /// <summary>
        /// Gets all of the smiles used on Something Awful.
        /// </summary>
        /// <param name="token">A CancellationToken.</param>
        /// <returns>A list of Smile Categories, which includes the smiles.</returns>
        public async Task<List<SmileCategory>> GetSmileListAsync(CancellationToken token = default)
        {
            var result = await this.webManager.GetDataAsync(EndPoints.SmileUrl, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return SmileHandler.ParseSmileList(document);
        }
    }
}
