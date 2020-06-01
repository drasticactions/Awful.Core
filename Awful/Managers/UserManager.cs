// <copyright file="UserManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Models.Users;

namespace Awful.Parser.Managers
{
    /// <summary>
    /// Manager for handling Users on Something Awful.
    /// </summary>
    public class UserManager
    {
        private readonly WebClient webManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserManager"/> class.
        /// </summary>
        /// <param name="webManager">The SA WebClient.</param>
        public UserManager(WebClient webManager)
        {
            this.webManager = webManager;
        }

        /// <summary>
        /// Gets a user from their profile page.
        /// </summary>
        /// <param name="userId">The User Id. 0 gets the current logged in user.</param>
        /// <param name="token">A CancellationToken.</param>
        /// <returns>A User.</returns>
        public async Task<User> GetUserFromProfilePageAsync(long userId, CancellationToken token = default)
        {
            string url = string.Format(CultureInfo.InvariantCulture, EndPoints.UserProfile, userId);
            var result = await this.webManager.GetDataAsync(url, token).ConfigureAwait(false);
            var document = await this.webManager.Parser.ParseDocumentAsync(result.ResultHtml, token).ConfigureAwait(false);
            return UserHandler.ParseUserFromProfilePage(userId, document);
        }
    }
}
