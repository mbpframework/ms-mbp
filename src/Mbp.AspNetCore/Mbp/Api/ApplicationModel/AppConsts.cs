using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.AspNetCore.Api
{
    internal static class ApplicationServiceConsts
    {
        public static string DefaultHttpVerb { get; set; }

        public static string DefaultAreaName { get; set; }

        public static string DefaultApiPreFix { get; set; }

        public static List<string> ControllerPostfixes { get; set; }
        public static List<string> ActionPostfixes { get; set; }

        public static List<Type> FormBodyBindingIgnoredTypes { get; set; }

        public static Dictionary<string, string> HttpVerbs { get; set; }

        static ApplicationServiceConsts()
        {
            HttpVerbs = new Dictionary<string, string>()
            {
                ["add"] = "POST",
                ["create"] = "POST",
                ["post"] = "POST",

                ["get"] = "GET",
                ["find"] = "GET",
                ["fetch"] = "GET",
                ["query"] = "GET",

                ["update"] = "POST",
                ["put"] = "POST",

                ["delete"] = "POST",
                ["remove"] = "POST",

                ["patch"] = "POST",
            };
        }
    }
}
