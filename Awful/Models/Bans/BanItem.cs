// <copyright file="BanItem.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Awful.Parser.Models.Bans
{
    public class BanItem
    {
        public string Type { get; set; }

        public int PostId { get; set; }

        public DateTime Date { get; set; }

        public string HorribleJerk { get; set; }

        public int HorribleJerkId { get; set; }

        public string PunishmentReason { get; set; }

        public string RequestedBy { get; set; }

        public int RequestedById { get; set; }

        public string ApprovedBy { get; set; }

        public int ApprovedById { get; set; }
    }
}
