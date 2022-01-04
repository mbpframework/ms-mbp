namespace Mbp.EventBus
{
    internal class EventLogStorageOptions
    {
        /// <summary>
        /// 存储提供程序
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// 数据库连接字符串，Provider为Mysql生效
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 表名前缀，Provider为Mysql生效
        /// </summary>
        public string TableNamePrefix { get; set; }

        /// <summary>
        /// 数据库连接字符串，Provider为MongoDB生效
        /// </summary>
        public string DatabaseConnection { get; set; }

        /// <summary>
        /// 数据库名字，Provider为MongoDB生效
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// 发布集合名称，Provider为MongoDB生效
        /// </summary>
        public string PublishedCollection { get; set; }

        /// <summary>
        /// 接受集合名称，Provider为MongoDB生效
        /// </summary>
        public string ReceivedCollection { get; set; }
    }
}
