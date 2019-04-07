using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Operator.Implementation
{
    public class NotEquals : IOperator
    {
        public IEnumerable<Expression> AddExpression(IEnumerable<Expression> expressions, MemberExpression property, object value)
        {
            var expressionList = expressions.ToList();
            expressionList.Add(Expression.NotEqual(property, Expression.Convert(Expression.Constant(value), property.Type)));
            return expressionList;
        }
    }
}
