using System;
using System.Linq.Expressions;

namespace OnlineExam.WebApi.Utility._Expression
{
    public class WhereHelper<T> where T : class
    {
        private ParameterExpression param;
        private BinaryExpression filter;
        public WhereHelper()
        {
            param = Expression.Parameter(typeof(T), "c");
            //1==1
            Expression left = Expression.Constant(1);
            filter = Expression.Equal(left, left);
        }

        public Expression<Func<T, bool>> GetExpression()
        {
            return Expression.Lambda<Func<T,bool>>(filter,param);
        }

        public void Equal(string propertyName,object value)
        {
            Expression left = Expression.Property(param, typeof(T).GetProperty(propertyName));
            Expression right = Expression.Constant(value, value.GetType());
            Expression result = Expression.Equal(left, right);
            filter = Expression.And(filter, result);
        }

        public void Contains(string propertyName,string value)
        {
            Expression left = Expression.Property(param, typeof(T).GetProperty(propertyName));
            Expression right = Expression.Constant(value, value.GetType());
            MethodCallExpression result = Expression.Call(left, typeof(string).GetMethod("Contains"), right);
            filter = Expression.And(filter, result);
        }

    }
}
