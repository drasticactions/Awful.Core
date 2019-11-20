using Awful.Parser.Core;
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
        public static async Task<WebClient> SetupWebClient()
        {
            var username = Environment.GetEnvironmentVariable("AWFUL_USER");
            var password = Environment.GetEnvironmentVariable("AWFUL_PASSWORD");
            if (!File.Exists("user.cookies"))
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                    throw new Exception("You must set the username and password to log in!");
                var WebClient = new WebClient();
                var authManager = new AuthenticationManager(WebClient);
                var result = await authManager.AuthenticateAsync(username, password);
                if (!result.IsSuccess)
                    throw new Exception("Could not log in!");
                using (FileStream stream = File.Create("user.cookies"))
                {
                    var formatter = new BinaryFormatter();
                    System.Console.WriteLine("Serializing cookie container");
                    formatter.Serialize(stream, WebClient.CookieContainer);
                }
                return WebClient;
            }
            else
            {
                System.Net.CookieContainer cookieContainer;
                using (FileStream stream = File.OpenRead("user.cookies"))
                {
                    var formatter = new BinaryFormatter();
                    System.Console.WriteLine("Deserializing cookie container");
                    cookieContainer = (System.Net.CookieContainer)formatter.Deserialize(stream);
                    return new WebClient(cookieContainer);
                }
            }
        }
    }
}
