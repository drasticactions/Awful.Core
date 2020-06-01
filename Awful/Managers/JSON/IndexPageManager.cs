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
            var result = await this.webManager.GetDataAsync(EndPoints.IndexPageUrl, token).ConfigureAwait(false);

            if (!addAdditionalMetadata)
            {
                return JsonSerializer.Deserialize<IndexPage>(result.ResultHtml);
            }

            var data = JsonSerializer.Deserialize<IndexPage>(result.ResultHtml);

            foreach (var forum in data.Forums)
            {
                this.UpdateForumMetadata(forum);
            }

            return data;
        }

        public async Task<List<Forum>> GetForumListAsync(CancellationToken token = default)
        {
            var result = await this.webManager.GetDataAsync(EndPoints.IndexPageUrl, token).ConfigureAwait(false);

            var data = JsonSerializer.Deserialize<IndexPage>(result.ResultHtml);

            foreach (var forum in data.Forums)
            {
                this.UpdateForumMetadata(forum);
            }

            // The forums API returns null values for forums you can't access.
            // So if we see a zero for the ID, don't add it to the list.
            var forums = data.Forums.SelectMany(n => this.Flatten(n)).Where(n => n.Id != 0).ToList();
            for (int i = 0; i < forums.Count; i++)
            {
                forums[i].SortOrder = i + 1;
            }

            return forums;
        }

        private IEnumerable<Forum> Flatten(Forum forum)
        {
            yield return forum;
            if (forum.SubForums != null)
            {
                foreach (var child in forum.SubForums)
                {
                    foreach (var descendant in this.Flatten(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        private void UpdateForumMetadata(Forum forum, Forum parentForum = null)
        {
            if (parentForum != null)
            {
                forum.ParentId = parentForum.Id;
            }

            if (forum.SubForums == null)
            {
                return;
            }

            foreach (var subForum in forum.SubForums)
            {
                this.UpdateForumMetadata(subForum, forum);
            }
        }
    }
}
