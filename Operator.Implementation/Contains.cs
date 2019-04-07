using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Operator.Implementation
{
    public class Contains : IOperator
    {
        public IEnumerable<Expression> AddExpression(IEnumerable<Expression> expressions, MemberExpression property, object value)
        {
            var expressionList = expressions.ToList();
            var containType = value.GetType();
            var containMethod = containType.GetMethod("Contains");
            var containCall = Expression.Call(property, containMethod, Expression.Constant(value));
            expressionList.Add(containCall);
            return expressionList;
        }
    }
}
