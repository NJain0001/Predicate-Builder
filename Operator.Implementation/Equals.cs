using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Operator;

namespace Operator.Implementation
{
    public class Equals : IOperator
    {
        public IEnumerable<Expression> AddExpression(IEnumerable<Expression> expressions, MemberExpression property, object value)
        {
            var expressionList = expressions.ToList();
            expressionList.Add(Expression.Equal(property, Expression.Convert(Expression.Constant(value), property.Type)));
            return expressionList;
        }
    }
}
