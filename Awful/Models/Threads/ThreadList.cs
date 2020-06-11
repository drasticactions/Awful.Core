// <copyright file="ThreadList.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Awful.Parser.Models.Forums;
using Awful.Parser.Models.Posts;

namespace Awful.Parser.Models.Threads
{
    public class ThreadList
    {
        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public List<Thread> Threads { get; } = new List<Thread>();
    }
}
