using Res.Business.Bo;
using System;
using System.Collections.Generic;
using System.Text;
using Mbp.EventBus;

namespace Res.Business
{
    public interface Sub
    {
        void Subscribe(Class2 datetime);
    }


    public class Class1 : Sub, IMbpSubscribe
    {
        [MbpSubscribe("ABCD")]
        public void Subscribe(Class2 datetime)
        {
            Console.WriteLine(datetime.Name);
        }
    }
}
