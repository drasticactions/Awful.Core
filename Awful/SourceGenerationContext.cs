// <copyright file="SourceGenerationContext.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Text.Json.Serialization;
using Awful.Core.Models.JSON;
using Awful.Models;

namespace Awful;

/// <summary>
/// Source generation context for JSON serialization.
/// </summary>
[JsonSourceGenerationOptions(
        WriteIndented = false,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull | JsonIgnoreCondition.WhenWritingDefault)]
[JsonSerializable(typeof(SerializableCookie))]
[JsonSerializable(typeof(IndexPage))]

[JsonSerializable(typeof(List<SerializableCookie>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}