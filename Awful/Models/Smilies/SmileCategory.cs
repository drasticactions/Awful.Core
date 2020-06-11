// <copyright file="SmileCategory.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Awful.Parser.Models.Smilies
{
    public class SmileCategory
    {
        public List<Smile> SmileList { get; } = new List<Smile>();

        public string Name { get; set; }
    }
}
