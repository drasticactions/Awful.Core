// <copyright file="SerializableCookie.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Net;
using System.Text.Json.Serialization;

namespace Awful.Models;

/// <summary>
/// A serializable version of a cookie.
/// </summary>
public class SerializableCookie
{
    /// <summary>
    /// Gets or sets the name of the cookie.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the value of the cookie.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the path of the cookie.
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; }

    /// <summary>
    /// Gets or sets the domain of the cookie.
    /// </summary>
    [JsonPropertyName("domain")]
    public string Domain { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the cookie is secure.
    /// </summary>
    [JsonPropertyName("secure")]
    public bool Secure { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the cookie is HttpOnly.
    /// </summary>
    [JsonPropertyName("httpOnly")]
    public bool HttpOnly { get; set; }

    /// <summary>
    /// Gets or sets the expiration date of the cookie.
    /// </summary>
    [JsonPropertyName("expires")]
    public DateTime? Expires { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the cookie should be discarded.
    /// </summary>
    [JsonPropertyName("discard")]
    public bool Discard { get; set; }

    /// <summary>
    /// Gets or sets the version of the cookie.
    /// </summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the comment associated with the cookie.
    /// </summary>
    [JsonPropertyName("comment")]
    public string Comment { get; set; }

    /// <summary>
    /// Convert from Cookie to SerializableCookie.
    /// </summary>
    /// <param name="cookie"><see cref="Cookie"/>.</param>
    /// <returns><see cref="SerializableCookie"/>.</returns>
    public static SerializableCookie FromCookie(Cookie cookie)
    {
        return new SerializableCookie
        {
            Name = cookie.Name,
            Value = cookie.Value,
            Path = cookie.Path,
            Domain = cookie.Domain,
            Secure = cookie.Secure,
            HttpOnly = cookie.HttpOnly,
            Expires = cookie.Expires == DateTime.MinValue ? null : cookie.Expires,
            Discard = cookie.Discard,
            Version = cookie.Version,
            Comment = cookie.Comment,
        };
    }

    /// <summary>
    /// Convert from SerializableCookie to Cookie.
    /// </summary>
    /// <returns><see cref="Cookie"/>.</returns>
    public Cookie ToCookie()
    {
        var cookie = new Cookie(this.Name, this.Value, this.Path, this.Domain)
        {
            Secure = this.Secure,
            HttpOnly = this.HttpOnly,
            Discard = this.Discard,
            Version = this.Version,
        };

        if (!string.IsNullOrEmpty(this.Comment))
        {
            cookie.Comment = this.Comment;
        }

        if (this.Expires.HasValue)
        {
            cookie.Expires = this.Expires.Value;
        }

        return cookie;
    }
}