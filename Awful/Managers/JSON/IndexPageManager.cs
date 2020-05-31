// <copyright file="IndexPageManager.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Awful.Core.Models.JSON;
using Awful.Parser.Core;

namespace Awful.Core.Managers.JSON
{
    public class IndexPageManager
    {
        private readonly WebClient webManager;

        public IndexPageManager(WebClient webManager)
        {
            this.webManager = webManager;
        }

        public async Task<IndexPage> GetIndexPageAsync (bool addAdditionalMetadata = false, CancellationToken token = default)
        {
            var result = await webManager.GetDataAsync(EndPoints.IndexPageUrl, token);
            if (!result.IsSuccess)
                throw new Exception("Could not get Index page JSON");

            if (!addAdditionalMetadata)
                return JsonSerializer.Deserialize<IndexPage>(result.ResultHtml);

            var data = JsonSerializer.Deserialize<IndexPage>(result.ResultHtml);

            foreach (var forum in data.Forums)
            {
                UpdateForumMetadata(forum);
            }

            return data;
        }

        public async Task<List<Forum>> GetForumListAsync(CancellationToken token = default)
        {
            var result = await webManager.GetDataAsync(EndPoints.IndexPageUrl, token);
            if (!result.IsSuccess)
                throw new Exception("Could not get Index page JSON");

            var data = JsonSerializer.Deserialize<IndexPage>(result.ResultHtml);

            foreach (var forum in data.Forums)
                UpdateForumMetadata(forum);

            // The forums API returns null values for forums you can't access.
            // So if we see a zero for the ID, don't add it to the list.
            var forums = data.Forums.SelectMany(n => Flatten(n)).Where(n => n.Id != 0).ToList();
            for (int i = 0; i < forums.Count; i++)
                forums[i].SortOrder = i + 1;

            return forums;
        }

        private IEnumerable<Forum> Flatten(Forum forum)
        {
            yield return forum;
            if (forum.SubForums != null)
            {
                foreach (var child in forum.SubForums)
                    foreach (var descendant in Flatten(child))
                        yield return descendant;
            }
        }

        private void UpdateForumMetadata (Forum forum, Forum parentForum = null)
        {
            if (parentForum != null)
                forum.ParentId = parentForum.Id;

            if (forum.SubForums == null)
                return;

            foreach (var subForum in forum.SubForums)
                UpdateForumMetadata(subForum, forum);
        }
    }
}
