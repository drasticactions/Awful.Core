// <copyright file="BanItem.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Awful.Parser.Models.Bans
{
    public class BanPage
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<BanItem> Bans { get; set; } = new List<BanItem>();
    }
}
