using System.Threading;

namespace Mbp.Core
{
    /// <summary>
    /// MbpContext寄存类
    /// </summary>
    public class MbpContextAccessor : IMbpContextAccessor
    {
        // Mbp进程上下文信息
        private static AsyncLocal<HttpContextHolder> _MbpContextCurrent = new AsyncLocal<HttpContextHolder>();

        public MbpContext MbpContext
        {
            get
            {
                return _MbpContextCurrent.Value?.Context;
            }
            set
            {
                var holder = _MbpContextCurrent.Value;
                if (holder != null)
                {
                    // Clear current MbpContext trapped in the AsyncLocals, as its done.
                    holder.Context = null;
                }

                if (value != null)
                {
                    // Use an object indirection to hold the MbpContext in the AsyncLocal,
                    // so it can be cleared in all ExecutionContexts when its cleared.
                    _MbpContextCurrent.Value = new HttpContextHolder { Context = value };
                }
            }
        }

        private class HttpContextHolder
        {
            public MbpContext Context;
        }
    }
}
