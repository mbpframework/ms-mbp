using Mbp.Framework.Web;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Mbp.Framework.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            // Æô¶¯¿ò¼Ü
            MbpFrameworkWebHost.WebHost<Startup>(args);
        }

        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Log.Information(e.Exception.StackTrace);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Information(e.ExceptionObject.As<InvalidOperationException>().StackTrace);
        }
    }
}
