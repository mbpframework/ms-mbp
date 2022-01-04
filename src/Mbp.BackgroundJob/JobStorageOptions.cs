using System;
using System.Collections.Generic;
using System.Text;

namespace WuhanIns.Nitrogen.BackgroundJob
{
    internal class JobStorageOptions
    {
        public string Provider { get; set; }

        public string ConnectionString { get; set; }

        public string TablePrefix { get; set; }
    }
}
