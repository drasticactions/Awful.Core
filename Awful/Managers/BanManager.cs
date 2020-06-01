// <copyright file="BanManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Awful.Exceptions;
using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Bans;

namespace Awful.Parser.Managers
{
    /// <summary>
    /// Manager for handling Bans on Something Awful.
    /// </summary>
    public class BanManager
    {
        private readonly WebClient webManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="BanManager"/> class.
        /// </summary>
        /// <param name="webManager">The SA WebClient.</param>
        public BanManager(WebClient webManager)
        {
            this.webManager = webManager;
        }

        /// <summary>
        /// Get the Banned Members page.
        /// </summary>
        /// <param name="page">The page number to start parsing from. Defaults to 1.</param>
        /// <param name="token">A CancellationToken.</param>
        /// <returns>A BanPage.</returns>
        public async Task<BanPage> GetBanPageAsync(int page = 1, CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var result = await this.webManager.GetDataAsync(string.Format(CultureInfo.InvariantCulture, EndPoints.RapSheet, page), token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return BanHandler.ParseBanPage(document);
        }

        /// <summary>
        /// Pings the main forums page and checks if the user under probation.
        /// </summary>
        /// <param name="token">A CancellationToken.</param>
        /// <returns>A ProbationItem.</returns>
        public async Task<ProbationItem> CheckForProbation(CancellationToken token = default)
        {
            if (!this.webManager.IsAuthenticated)
            {
                throw new UserAuthenticationException(Awful.Core.Resources.ExceptionMessages.UserAuthenticationError);
            }

            var result = await this.webManager.GetDataAsync(EndPoints.BaseUrl, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            var prob = BanHandler.ParseForProbation(document);
            this.webManager.Probation = prob;
            return prob;
        }
    }
}
