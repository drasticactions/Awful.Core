// <copyright file="ProbationItem.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Awful.Parser.Models.Bans
{
    public class ProbationItem
    {
        public DateTime ProbationUntil { get; set; }

        public bool IsUnderProbation { get; set; }
    }
}
