// <copyright file="ErrorHandler.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.Text.Json;
using Awful.Exceptions;
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
        /// <param name="exception">The exception being thrown.</param>
        /// <returns>A new result object with the given error.</returns>
        public static Result CreateErrorObject(Result result, Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            result.IsSuccess = false;
            result.Type = typeof(Error).ToString();

            var isPaywall = exception is PaywallException;

            var error = new Error()
            {
                Type = exception.GetType().ToString(),
                Reason = exception.Message,
                StackTrace = exception.StackTrace,
                IsPaywall = isPaywall,
            };
            result.ResultJson = JsonSerializer.Serialize(error);
            return result;
        }
    }
}
