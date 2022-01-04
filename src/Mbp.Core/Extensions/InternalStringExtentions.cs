using System;
using System.Text.RegularExpressions;

namespace Mbp.Internal.Extensions
{
    /// <summary>
    /// String 扩展方法
    /// </summary>
    public static class InternalStringExtensions
    {
        /// <summary>
        /// 是否为空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 是否包含
        /// </summary>
        /// <param name="str"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsIn(this string str, params string[] data)
        {
            foreach (var item in data)
            {
                if (str == item)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 移除后缀
        /// </summary>
        /// <param name="str"></param>
        /// <param name="postFixes"></param>
        /// <returns></returns>
        public static string RemovePostFix(this string str, params string[] postFixes)
        {
            if (str == null)
            {
                return null;
            }

            if (str == string.Empty)
            {
                return string.Empty;
            }

            if (postFixes.IsNullOrEmpty())
            {
                return str;
            }

            foreach (var postFix in postFixes)
            {
                if (str.EndsWith(postFix))
                {
                    return str.Left(str.Length - postFix.Length);
                }
            }

            return str;
        }

        /// <summary>
        /// 移除前缀
        /// </summary>
        /// <param name="str"></param>
        /// <param name="preFixes"></param>
        /// <returns></returns>
        public static string RemovePreFix(this string str, params string[] preFixes)
        {
            if (str == null)
            {
                return null;
            }

            if (str == string.Empty)
            {
                return string.Empty;
            }

            if (preFixes.IsNullOrEmpty())
            {
                return str;
            }

            foreach (var preFix in preFixes)
            {
                if (str.StartsWith(preFix))
                {
                    return str.Right(str.Length - preFix.Length);
                }
            }

            return str;
        }

        /// <summary>
        /// 返回最右边指定长度字符串
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="length">Max number of charaters to return</param>
        /// <returns>Returns string from right</returns>
        public static string Left(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(0, len);
        }

        /// <summary>
        /// 返回最左边指定长度字符串
        /// </summary>
        /// <param name="value">String value</param>
        /// <param name="length">Max number of charaters to return</param>
        /// <returns>Returns string from left</returns>
        public static string Right(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(str.Length - len, len);
        }

        /// <summary>
        /// 获取Camel第一单词
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetCamelCaseFirstWord(this string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.Length == 1)
            {
                return str;
            }

            var res = Regex.Split(str, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})");

            if (res.Length < 1)
            {
                return str;
            }
            else
            {
                return res[0];
            }
        }

        /// <summary>
        /// 获取Pascal第一单词
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetPascalCaseFirstWord(this string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.Length == 1)
            {
                return str;
            }

            var res = Regex.Split(str, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})");

            if (res.Length < 2)
            {
                return str;
            }
            else
            {
                return res[1];
            }
        }

        /// <summary>
        /// 获取PascalOrCamel第一单词
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetPascalOrCamelCaseFirstWord(this string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.Length == 1)
            {
                return str;
            }

            if (str[0] >= 65 && str[0] <= 90)
            {
                return GetPascalCaseFirstWord(str);
            }
            else
            {
                return GetCamelCaseFirstWord(str);
            }
        }
    }
}
