using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HoloBlock.EntityFramework.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Will call .Include over the full param array.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <param name="source"></param>
        /// <param name="includedEntities">All related entities to be included in the query</param>
        /// <returns></returns>
        public static IQueryable<TSource> IncludeEntities<TSource>(this IQueryable<TSource> source, params Expression<Func<TSource, object>>[] includedEntities)
            where TSource : class
        {
            foreach (Expression<Func<TSource, object>> item in includedEntities)
            {
                // strip it down to the full navigation path and include the string, this way we don't need to worry about using .Include and .ThenInclude in the proper order
                source = source.Include(ParseLambda(item));
            }

            return source;
        }

        #region Private Methods

        /// <summary>
        /// Parses the lambda expression depending on its type to build its path.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static string ParseLambda(LambdaExpression expression)
        {
            if (expression.Body is MemberExpression memberBody)
            {
                return ParseMember(memberBody);
            }

            if (expression.Body is MethodCallExpression methodBody)
            {
                return ParseMethodCall(methodBody);
            }

            throw new NotSupportedException("Unknown body type: " + expression.Body.Type.FullName);
        }

        /// <summary>
        /// Parses member expressions to build the path.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static string ParseMember(MemberExpression expression)
        {
            StringBuilder sb = new StringBuilder();

            if (expression.Expression is MemberExpression me)
            {
                sb.Append(ParseMember(me));
            }

            if (sb.Length > 0)
            {
                sb.Append(".");
            }

            sb.Append(expression.Member.Name);

            return sb.ToString();
        }

        /// <summary>
        /// Parses the method call expression to build the path.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static string ParseMethodCall(MethodCallExpression expression)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Expression item in expression.Arguments)
            {
                if (sb.Length > 0)
                {
                    sb.Append(".");
                }

                if (item is MemberExpression me)
                {
                    sb.Append(ParseMember(me));
                }
                else if (item is LambdaExpression le)
                {
                    sb.Append(ParseLambda(le));
                }
                else if (item is MethodCallExpression mce)
                {
                    sb.Append(ParseMethodCall(mce));
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}
