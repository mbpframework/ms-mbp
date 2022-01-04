using System;
using System.Collections.Generic;
using System.Text;

namespace Mbp.Core.User
{
    /// <summary>
    /// 当前登录用户信息
    /// </summary>
    public interface ICurrentUser
    {
        /// <summary>
        /// 用户信息唯一编号
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// 用户登录名
        /// </summary>
        string LoginName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        string Phonenumber { get; set; }

        /// <summary>
        /// 登录令牌
        /// </summary>
        string AccessToken { get; set; }
    }
}
