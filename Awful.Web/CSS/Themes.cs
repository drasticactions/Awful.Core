using System;
using System.IO;
using System.Reflection;

namespace Awful.Web.CSS
{
    public class Themes
    {
        public static string BaseCSS = "Awful.Web.Templates.base-thread.css";

        public static string GetCSS(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }
    }
}
