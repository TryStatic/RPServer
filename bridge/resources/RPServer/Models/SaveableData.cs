using System;
using System.Linq.Expressions;

namespace RPServer.Models
{
    internal class SaveableData
    {
        protected string GetColumnName<T>(Expression<Func<T>> expression, out object value)
        {
            var expressionBody = expression.Body as MemberExpression;
            var attrs = expressionBody?.Member.GetCustomAttributes(typeof(SqlColumnNameAttribute), false);
            value = GetValue(expressionBody);
            if (attrs == null || attrs.Length == 0) return null;
            var desc = attrs[0] as SqlColumnNameAttribute;
            return desc?.ColumnName;
        }

        private object GetValue(MemberExpression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }


        protected class SqlColumnNameAttribute : Attribute
        {
            public string ColumnName { get; }
            internal SqlColumnNameAttribute(string columnName) => ColumnName = columnName;
        }
    }
}