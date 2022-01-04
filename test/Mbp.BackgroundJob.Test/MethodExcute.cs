using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nitrogen.BackgroundJob.Test
{
    internal static class MethodExcute
    {
        public static Task GetName()
        {
            return Task.Run(() => Console.WriteLine("GetName"));
        }

        public static Task GetName(string a)
        {
            return Task.Run(() => Console.WriteLine(a));
        }
    }
}
