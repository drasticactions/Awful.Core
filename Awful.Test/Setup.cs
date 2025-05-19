using Awful.Parser.Core;
using Awful.Parser.Handlers;
using Awful.Parser.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Awful.Test
{
    public class Setup
    {
        public static async Task<WebClient> SetupWebClient(bool useAuth = true)
        {
            var username = Environment.GetEnvironmentVariable("AWFUL_USER");
            var password = Environment.GetEnvironmentVariable("AWFUL_PASSWORD");
            if (!File.Exists("user.cookies"))
            {
                var WebClient = new WebClient();
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    return WebClient;
                var authManager = new AuthenticationManager(WebClient);
                var result = await authManager.AuthenticateAsync(username, password);
                if (!result.IsSuccess)
                    throw new Exception("Could not log in!");
                CookieManager.SaveCookie(result.AuthenticationCookieContainer, "user.cookies");

                return WebClient;
            }
            else
            {
                return new WebClient(CookieManager.LoadCookie("user.cookies"));
            }
        }
    }
}
