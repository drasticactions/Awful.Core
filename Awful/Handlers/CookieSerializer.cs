// <copyright file="CookieSerializer.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;

namespace Awful.Parser.Handlers
{
    /// <summary>
    /// Serializes or Deserializes Cookie Containers.
    /// </summary>
    public static class CookieSerializer
    {
        /// <summary>
        /// Serialized a cookie container.
        /// </summary>
        /// <param name="cookies">The cookie container.</param>
        /// <param name="stream">The stream to serialize the cookies to.</param>
        public static void Serialize(CookieCollection cookies, Stream stream)
        {
            var serializer = new DataContractSerializer(typeof(IEnumerable<Cookie>));
            var cookieList = cookies.OfType<Cookie>();

            serializer.WriteObject(stream, cookieList);
        }

        /// <summary>
        /// Deserializes a cookie container.
        /// </summary>
        /// <param name="baseUri">The base uri of the cookie package.</param>
        /// <param name="stream">The stream to write the cookies to.</param>
        /// <param name="cookieHostName">The cookie hostname, defaults to `.somethingawful.com`.</param>
        /// <returns>A CookieContainer.</returns>
        public static CookieContainer Deserialize(Uri baseUri, Stream stream, string cookieHostName = ".somethingawful.com")
        {
            var container = new CookieContainer();
            var serializer = new DataContractSerializer(typeof(IEnumerable<Cookie>));
            var cookies = (IEnumerable<Cookie>)serializer.ReadObject(stream);
            var cookieCollection = new CookieCollection();

            foreach (var fixedCookie in cookies.Select(cookie => new Cookie(cookie.Name, cookie.Value, "/", cookieHostName)))
            {
                cookieCollection.Add(fixedCookie);
            }

            container.Add(baseUri, cookieCollection);
            return container;
        }
    }
}
