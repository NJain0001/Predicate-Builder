using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Operator
{
    public interface IOperator
    {
        IEnumerable<Expression> AddExpression(IEnumerable<Expression> expressions, MemberExpression property, object value);
    }
}
