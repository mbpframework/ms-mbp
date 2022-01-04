using Nitrogen.Ddd.Domain;
using System;

namespace Nitrogen.Orm.Dapper.Extensions
{
    public static class ExpressionExtension
    {
        public static bool Like(this string col, string value)
        => throw new InvalidOperationException("LikeMethodInvoked");

        public static bool LikeLeft(this string col, string value)
        => throw new InvalidOperationException("LikeLeftMethodInvoked");

        public static bool LikeRight(this string col, string value)
        => throw new InvalidOperationException("LikeRightMethodInvoked");

        public static bool In<T>(this object col, params T[] ary)
        => throw new InvalidOperationException("InMethodInvoked");
    }
}
