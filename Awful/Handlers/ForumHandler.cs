using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Awful.Parser.Core;
using Awful.Parser.Models.Forums;
using Awful.Parser.Models.Threads;

namespace Awful.Parser.Handlers
{
    /// <summary>
    /// Handles Something Awful Forum Elements.
    /// </summary>
    public static class ForumHandler
    {
        /// <summary>
        /// Add Debug Forum (For Users who can access Apps In Developmental States).
        /// </summary>
        /// <returns>Debug Forum.</returns>
        public static Forum AddDebugForum()
        {
            var forum = new Forum()
            {
                Name = "Apps In Developmental States",
                Location = EndPoints.BaseUrl + "forumdisplay.php?forumid=261",
                IsSubForum = false,
                ForumId = 261,
            };
            return forum;
        }

        /// <summary>
        /// Parses the forum category list via the Category page select dropdown list.
        /// </summary>
        /// <param name="document">SA Page IHtmlDocument.</param>
        /// <returns>SA Forums Category List.</returns>
        public static List<Category> ParseCategoryListViaSelect(IHtmlDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var forumSelector = document.All.FirstOrDefault(m => m.LocalName == "select" && m.GetAttribute("name") == "forumid");
            if (forumSelector == null)
            {
                throw new FormatException(Awful.Core.Resources.ExceptionMessages.ForumSelectorMissing);
            }

            return new List<Category>();
        }

        /// <summary>
        /// Parses the forum category list via the main SA Forum page.
        /// </summary>
        /// <param name="document">SA Page IHtmlDocument.</param>
        /// <returns>SA Forums Category List.</returns>
        public static List<Category> ParseCategoryList(IHtmlDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var forumSelector = document.All.FirstOrDefault(m => m.LocalName == "table" && m.Id == "forums");
            if (forumSelector == null)
            {
                throw new FormatException(Awful.Core.Resources.ExceptionMessages.ForumTableMissing);
            }

            var categories = new List<Category>();
            var rows = forumSelector.QuerySelectorAll("tr");
            var order = 1;
            var innerOrder = 1;
            foreach (var row in rows)
            {
                if (row.ClassName == "section")
                {
                    var category = GetCategoryFromLink(row, order);
                    categories.Add(category);
                    order++;
                    innerOrder = 1;
                }
                else
                {
                    var selectedCatagory = categories.Last();
                    var forum = GetForumFromLink(row, innerOrder, selectedCatagory.Id);
                    innerOrder++;
                    selectedCatagory.ForumList.Add(forum);

                    var subForumList = row.QuerySelector(".subforums");
                    if (subForumList == null)
                    {
                        continue;
                    }

                    var subForumListLinks = subForumList.QuerySelectorAll("a");
                    foreach (var subForumLink in subForumListLinks)
                    {
                        var subForum = GetSubForumFromLink(subForumLink, innerOrder, selectedCatagory.Id);
                        subForum.ParentForumId = forum.ForumId;
                        forum.SubForums.Add(subForum);
                        innerOrder++;
                    }
                }
            }

            return categories;
        }

        /// <summary>
        /// Parses the HTML of the Something Awful Forums front page for the forum groups.
        /// </summary>
        /// <param name="document">SA HTML document.</param>
        /// <returns>SA Forums Group List.</returns>
        public static List<ForumGroup> ParseForumGroupList(IHtmlDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var forumSelector = document.All.FirstOrDefault(m => m.LocalName == "table" && m.Id == "forums");
            if (forumSelector == null)
            {
                throw new FormatException(Awful.Core.Resources.ExceptionMessages.ForumTableMissing);
            }

            var categories = new List<Category>();
            var forumGroup = new List<ForumGroup>();
            List<Forum> forums = new List<Forum>();
            var rows = forumSelector.QuerySelectorAll("tr");
            var order = 1;
            var innerOrder = 1;

            foreach (var row in rows)
            {
                if (row.ClassName == "section")
                {
                    if (categories.Any())
                    {
                        var lastCategory = categories.Last();
                        forumGroup.Add(new ForumGroup(lastCategory, forums));
                        forums = new List<Forum>();
                    }

                    var category = GetCategoryFromLink(row, order);
                    categories.Add(category);
                    order++;
                    innerOrder = 1;
                }
                else
                {
                    var selectedCatagory = categories.Last();
                    var forum = GetForumFromLink(row, innerOrder, selectedCatagory.Id);
                    innerOrder++;
                    selectedCatagory.ForumList.Add(forum);
                    forums.Add(forum);
                    var subForumList = row.QuerySelector(".subforums");
                    if (subForumList == null)
                    {
                        continue;
                    }

                    var subForumListLinks = subForumList.QuerySelectorAll("a");
                    foreach (var subForumLink in subForumListLinks)
                    {
                        var subForum = GetSubForumFromLink(subForumLink, innerOrder, selectedCatagory.Id);
                        subForum.ParentForumId = forum.ForumId;
                        forum.SubForums.Add(subForum);
                        forums.Add(forum);
                        innerOrder++;
                    }
                }
            }

            return forumGroup;
        }

        /// <summary>
        /// Get Forum Page Info.
        /// </summary>
        /// <param name="document">The SA Forum Page.</param>
        /// <param name="threadList">The Forum Thread List.</param>
        /// <returns>A Forum Thread List with Updated Info.</returns>
        public static ThreadList GetForumPageInfo(IHtmlDocument document, ThreadList threadList)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (threadList == null)
            {
                throw new ArgumentNullException(nameof(threadList));
            }

            var pages = document.QuerySelector(".pages");
            if (pages == null)
            {
                return threadList;
            }

            var select = pages.QuerySelector("select");
            var selectedPageItem = select.QuerySelector("option:checked");
            threadList.CurrentPage = Convert.ToInt32(selectedPageItem.TextContent, CultureInfo.InvariantCulture);
            threadList.TotalPages = select.ChildElementCount;
            return threadList;
        }

        /// <summary>
        /// Get detailed forum descriptions from the category page.
        /// </summary>
        /// <param name="document">The SA Forum Page.</param>
        /// <param name="category">The Forum Category.</param>
        /// <returns>The Forum Category with updated forum descriptions.</returns>
        public static Category ParseForumDescriptions(IHtmlDocument document, Category category)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var subForumListLinks = document.QuerySelectorAll(".subforum");
            foreach (var row in subForumListLinks)
            {
                var forumHtml = row.QuerySelector("a");
                var topicsHtml = row.QuerySelector(".topics");
                var postsHtml = row.QuerySelector(".posts");
                var descriptionHtml = row.QuerySelector("dd");
                var forum = category.ForumList.FirstOrDefault(n => n.Location == forumHtml.GetAttribute("href"));
                if (forum == null)
                {
                    continue;
                }

                forum.Description = descriptionHtml.TextContent.Replace("-", string.Empty).Trim();
                forum.TotalPosts = Convert.ToInt32(postsHtml.TextContent, CultureInfo.InvariantCulture);
                forum.TotalTopics = Convert.ToInt32(topicsHtml.TextContent, CultureInfo.InvariantCulture);
            }

            return category;
        }

        /// <summary>
        /// Get detailed forum descriptions from the category page.
        /// </summary>
        /// <param name="document">The SA Forum Page.</param>
        /// <param name="forum">The Forum.</param>
        /// <returns>The Forum with updated sub forum descriptions.</returns>
        public static Forum ParseSubForumDescriptions(IHtmlDocument document, Forum forum)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (forum == null)
            {
                throw new ArgumentNullException(nameof(forum));
            }

            var subForumListLinks = document.QuerySelectorAll(".subforum");
            foreach (var row in subForumListLinks)
            {
                var topicsHtml = row.QuerySelector(".topics");
                var postsHtml = row.QuerySelector(".posts");
                var forumHtml = row.QuerySelector("a");
                var descriptionHtml = row.QuerySelector("dd");
                var subForum = forum.SubForums.FirstOrDefault(n => n.Location == forumHtml.GetAttribute("href"));
                if (subForum == null)
                {
                    continue;
                }

                subForum.Description = descriptionHtml.TextContent.Replace("-", string.Empty).Trim();
                subForum.TotalPosts = Convert.ToInt32(postsHtml.TextContent, CultureInfo.InvariantCulture);
                subForum.TotalTopics = Convert.ToInt32(topicsHtml.TextContent, CultureInfo.InvariantCulture);
            }

            return forum;
        }

        private static Category GetCategoryFromLink(IElement row, int order = 0)
        {
            var catHtml = row.QuerySelector("a");
            var category = new Category();
            category.Name = catHtml.TextContent;
            category.Location = catHtml.GetAttribute("href");
            var queryString = HtmlHelpers.ParseQueryString(category.Location);
            category.Id = Convert.ToInt32(queryString["forumid"], CultureInfo.InvariantCulture);
            category.Order = order;
            return category;
        }

        private static Forum GetForumFromLink(IElement row, int order = 0, int categoryId = 0)
        {
            var forum = new Forum();
            var forumHtml = row.QuerySelector(".forum");
            forum.Location = forumHtml.GetAttribute("href");
            var queryString = HtmlHelpers.ParseQueryString(forum.Location);
            forum.ForumId = Convert.ToInt32(queryString["forumid"], CultureInfo.InvariantCulture);
            forum.CategoryId = categoryId;
            forum.Description = forumHtml.GetAttribute("title");
            forum.Name = forumHtml.TextContent;
            forum.Order = order;
            return forum;
        }

        private static Forum GetSubForumFromLink(IElement subForumLink, int order = 0, int categoryId = 0)
        {
            var subForum = new Forum();
            subForum.Location = subForumLink.GetAttribute("href");
            var subQueryString = HtmlHelpers.ParseQueryString(subForum.Location);
            subForum.ForumId = Convert.ToInt32(subQueryString["forumid"], CultureInfo.InvariantCulture);
            subForum.CategoryId = categoryId;
            subForum.Name = subForumLink.TextContent;
            subForum.IsSubForum = true;
            subForum.Order = order;
            return subForum;
        }
    }
}
