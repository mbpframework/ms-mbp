using DotNetCore.CAP;

namespace Mbp.EventBus
{
    /// <summary>
    /// 绑定订阅的事件主题
    /// </summary>
    public class MbpSubscribeAttribute : CapSubscribeAttribute
    {
        public MbpSubscribeAttribute(string topicName, bool isPartial = false) : base(topicName, isPartial)
        {

        }
    }
}
