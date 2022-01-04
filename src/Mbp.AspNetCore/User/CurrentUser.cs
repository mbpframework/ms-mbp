namespace Mbp.Core.User
{
    public class CurrentUser : ICurrentUser
    {
        /// <summary>
        /// 用户信息唯一编号
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string Phonenumber { get; set; }

        /// <summary>
        /// 登录令牌
        /// </summary>
        public string AccessToken { get; set; }
    }
}
