namespace Mbp.EventBus
{
    /// <summary>
    /// 事件总线模块选项
    /// </summary>
    internal class EventBusModuleOptions
    {

        public MessageQueueOptions MessageQueue { get; set; }

        public EventLogStorageOptions EventLogStorage { get; set; }
    }
}
