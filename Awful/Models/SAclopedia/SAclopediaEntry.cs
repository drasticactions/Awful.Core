// <copyright file="SAclopediaEntry.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Awful.Parser.Models.SAclopedia
{
    public class SAclopediaEntry
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public List<SAclopediaPost> Posts { get; } = new List<SAclopediaPost>();
    }
}
