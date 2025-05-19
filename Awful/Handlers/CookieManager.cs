// <copyright file="CookieManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using Awful.Models;

namespace Awful.Parser.Handlers
{
    /// <summary>
    /// Helpers for managing cookies, used by Something Awful for authentication.
    /// </summary>
    public static class CookieManager
    {
        /// <summary>
        /// Loads a cookie.
        /// </summary>
        /// <param name="path">Path to the cookie file.</param>
        /// <returns>A CookieContainer.</returns>
        public static CookieContainer LoadCookie(string path)
        {
            var json = File.ReadAllText(path);
            var serializableCookies = JsonSerializer.Deserialize<List<SerializableCookie>>(json, SourceGenerationContext.Default.ListSerializableCookie);
            var container = new CookieContainer();

            foreach (var serializableCookie in serializableCookies)
            {
                var cookie = serializableCookie.ToCookie();
                container.Add(cookie);
            }

            return container;
        }

        /// <summary>
        /// Saves a cookie to storage.
        /// </summary>
        /// <param name="cookieContainer">The cookie container to be saved.</param>
        /// <param name="path">The path where the cookie should be saved.</param>
        public static void SaveCookie(CookieContainer cookieContainer, string path)
        {
            var serializableCookies = new List<SerializableCookie>();

            var knownDomains = GetKnownDomains();

            foreach (var domain in knownDomains)
            {
                var cookies = cookieContainer.GetCookies(new Uri(domain));
                foreach (Cookie cookie in cookies)
                {
                    serializableCookies.Add(SerializableCookie.FromCookie(cookie));
                }
            }

            var json = JsonSerializer.Serialize(serializableCookies, SourceGenerationContext.Default.ListSerializableCookie);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Deleted a cookie.
        /// </summary>
        /// <param name="path">Path of the cookie.</param>
        public static void RemoveCookie(string path)
        {
            System.IO.File.Delete(path);
        }

        private static List<string> GetKnownDomains()
        {
            return new List<string>
            {
                "https://forums.somethingawful.com",
            };
        }
    }
}
