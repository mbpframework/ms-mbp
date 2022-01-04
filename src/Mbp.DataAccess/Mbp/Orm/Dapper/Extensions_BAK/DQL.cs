namespace Nitrogen.Orm.Dapper.Extensions
{
    /// <summary>
    /// 查询结构
    /// </summary>
    public enum DQL
    {
        /// <summary>
        /// From
        /// </summary>
        From = 1,
        /// <summary>
        /// Where
        /// </summary>
        Where = 2,
        /// <summary>
        /// GroupBy
        /// </summary>
        GroupBy = 4,
        /// <summary>
        /// Having
        /// </summary>
        Having = 8,
        /// <summary>
        /// Select
        /// </summary>
        Select = 16,
        /// <summary>
        /// OrderBy
        /// </summary>
        OrderBy = 32
    }
}
