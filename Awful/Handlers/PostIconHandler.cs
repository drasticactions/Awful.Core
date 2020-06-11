﻿// <copyright file="PostIconHandler.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AngleSharp.Html.Dom;
using Awful.Parser.Models.PostIcons;

namespace Awful.Parser.Handlers
{
    /// <summary>
    /// Handles Something Awful Post Icon Elements.
    /// </summary>
    public static class PostIconHandler
    {
        /// <summary>
        /// Parses the SA Post Icon Page.
        /// </summary>
        /// <param name="document">Document containing the Post Icon List.</param>
        /// <returns>A list of post icons.</returns>
        public static List<PostIcon> ParsePostIconList(IHtmlDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var postIconList = new List<PostIcon>();
            var postIconsHtml = document.QuerySelectorAll(".posticon");
            foreach (var postIconHtml in postIconsHtml)
            {
                var inputId = Convert.ToInt32(postIconHtml.QuerySelector("input").GetAttribute("value"), CultureInfo.InvariantCulture);
                var image = postIconHtml.QuerySelector("img");
                var srcAlt = image.GetAttribute("alt");
                var imageLocation = image.GetAttribute("src");
                postIconList.Add(new PostIcon()
                {
                    Id = inputId,
                    ImageEndpoint = imageLocation,
                    ImageLocation = Path.GetFileNameWithoutExtension(imageLocation),
                    Title = srcAlt,
                });
            }

            return postIconList;
        }
    }
}
