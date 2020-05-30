// <copyright file="ErrorHandler.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Text.Json;
using Awful.Parser.Models.Web;

namespace Awful.Parser.Core
{
    /// <summary>
    /// Creates user handled errors from a given Something Awful Request.
    /// </summary>
    public static class ErrorHandler
    {
        /// <summary>
        /// Creates a new error object to be given for a user request.
        /// </summary>
        /// <param name="result">The Something Awful request.</param>
        /// <param name="reason">The reason for the failure, usually given as part of the request.</param>
        /// <param name="stacktrace">If the error occured in this library, the stack trace.</param>
        /// <param name="type">The type of error generated.</param>
        /// <param name="isPaywall">If the error may have come from the paywall.</param>
        /// <returns>A new result object with the given error.</returns>
        public static Result CreateErrorObject(Result result, string reason, string stacktrace, string type = "", bool isPaywall = false)
        {
            if (reason == null)
            {
                throw new ArgumentNullException(nameof(reason));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            result.IsSuccess = false;
            result.Type = typeof(Error).ToString();
            if (!isPaywall)
            {
                isPaywall = reason.Equals("paywall", System.StringComparison.OrdinalIgnoreCase);
            }

            var error = new Error()
            {
                Type = type,
                Reason = reason,
                StackTrace = stacktrace,
                IsPaywall = isPaywall,
            };
            result.ResultJson = JsonSerializer.Serialize(error);
            return result;
        }
    }
}
