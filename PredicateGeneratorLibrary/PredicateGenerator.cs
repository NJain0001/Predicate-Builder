using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Operator;
using ValueType;
using Operator.Implementation;
using ValueType.Implementation;
using System.Linq.Expressions;
using LinqKit;

namespace PredicateGeneratorLibrary
{
    public class PredicateGenerator
    {
        enum OperatorEnum { Equals, NotEquals, GreaterThan, LessThan, GreaterThanOrEqualTo, LessThanOrEqualTo, Contains, NotContains }

        private Dictionary<string, IOperator> operators = new Dictionary<string, IOperator>()
        {
            {"Equals", new Equals()},
            {"Not Equals", new NotEquals()},
            {"Greater Than", new GreaterThan() },
            {"Less Than", new LessThan() },
            {"Greater Than Or Equal To", new GreaterThanOrEqualTo() },
            {"Less Than Or Equal To", new LessThanOrEqualTo() },
            {"Contains", new Contains() },
            {"Not Contains", new NotContains() }
        };

        //private Dictionary<OperatorEnum, IOperator> operators = new Dictionary<OperatorEnum, IOperator>()
        //{
        //    {OperatorEnum.Equals, new Equals()},
        //    {OperatorEnum.NotEquals, new NotEquals()},
        //    {OperatorEnum.GreaterThan, new GreaterThan() },
        //    {OperatorEnum.LessThan, new LessThan() },
        //    {OperatorEnum.GreaterThanOrEqualTo, new GreaterThanOrEqualTo() },
        //    {OperatorEnum.LessThanOrEqualTo, new LessThanOrEqualTo() },
        //    {OperatorEnum.Contains, new Contains() },
        //    {OperatorEnum.NotContains, new NotContains() }
        //};

        private Dictionary<string, IValueType> valueTypes = new Dictionary<string, IValueType>()
        {
            {"System.Nullable`1[System.Int64]", new NullableInteger64() },
            {"System.Nullable`1[System.Int32]", new NullableInteger32() },
            {"System.Nullable`1[System.Int16]", new NullableInteger16() },
            {"System.Nullable`1[System.DateTime]", new NullableDateTime() },
            {"System.Nullable`1[System.Boolean]", new NullableBoolean() },
            {"System.Nullable`1[System.Decimal]", new NullableDecimal() },
            {"System.String", new StringType() },
        };

        public Expression<Func<T, bool>> GeneratePredicate<T>(IEnumerable<RoutingOptionRequest> request)
        {
            var predicate = PredicateBuilder.New<T>();
            var requestList = request.ToList();

            for (int i = 0; i < requestList.Count(); i++)
            {
                var expressions = GenerateExpression<T>(requestList[i]);
                var op = requestList[i].Operator;
                var condition = string.Empty;
                if (i == 0)
                {
                    condition = requestList[i].Condition;
                }
                else
                {
                    condition = requestList[i - 1].Condition;
                }

                predicate = InsertIntoPredicate<T>(expressions, predicate, op, condition, i);
                predicate = HandleNestedOptions(requestList[i], predicate);
            }

            return predicate;

        }

        private static ExpressionStarter<T> InsertIntoPredicate<T>(IEnumerable<Expression<Func<T,bool>>> expressions, ExpressionStarter<T> predicate, string op, string condition, int index)
        {
            var inner = PredicateBuilder.New<T>(false);
            foreach (var expression in expressions)
            {
                if (index == 0)
                {
                    predicate = predicate.Or(expression);
                }
                else if (op == "Contains")
                {
                    inner = inner.Or(expression);
                }
                else
                {
                    predicate = StoreIntoPredicate<T>(predicate, expression, condition);
                }
            }

            if (inner.IsStarted != false)
            {
                predicate.And(inner);
            }

            return predicate;
        }

        private ExpressionStarter<T> HandleNestedOptions<T>(RoutingOptionRequest request, ExpressionStarter<T> predicate)
        {
            if (request.NestedRequest != null)
            {
                var conditionFromRequest = request.Condition;
                var nestedRequests = request.NestedRequest.ToList();
                var innerPredicate = PredicateBuilder.New<T>(false);

                for (int i = 0; i < nestedRequests.Count(); i++)
                {
                    var expressions = GenerateExpression<T>(nestedRequests[i]);
                    var op = nestedRequests[i].Operator;
                    var conditionFromNested = string.Empty;
                    if (i == 0)
                    {
                        conditionFromNested = nestedRequests[i].Condition;
                    }
                    else
                    {
                        conditionFromNested = nestedRequests[i - 1].Condition;
                    }

                    innerPredicate = InsertIntoPredicate<T>(expressions, innerPredicate, op, conditionFromNested, i);
                }

                predicate = StoreIntoPredicate<T>(predicate, innerPredicate, conditionFromRequest);
            }
            return predicate;
        }
        private IEnumerable<Expression<Func<T, bool>>> GenerateExpression<T>(RoutingOptionRequest request)
        {
            try
            {
                List<Expression<Func<T, bool>>> expressionList = new List<Expression<Func<T, bool>>>();
                var type = typeof(T);
                var parameter = Expression.Parameter(type, "m");
                var property = Expression.PropertyOrField(parameter, request.TargetName);
                var dataType = property.Type.ToString();
                var convertedValue = ConvertValues(dataType, request.Values);
                var expressions = OperatorDecision(property, convertedValue, request.Operator, parameter);
                foreach (var expression in expressions)
                {
                    expressionList.Add(Expression.Lambda<Func<T, bool>>(expression, new ParameterExpression[] { parameter }));
                }
                return expressionList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private static ExpressionStarter<T> StoreIntoPredicate<T>(ExpressionStarter<T> predicate, Expression<Func<T,bool>> expression, string condition)
        {
            try
            {
                if (condition == "And")
                {
                    predicate = predicate.And(expression);
                }
                else
                {
                    predicate = predicate.Or(expression);
                }
                return predicate;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private IEnumerable<object> ConvertValues(string dataType, IEnumerable<string> values)
        {
            try
            {
                List<object> convertedValues = new List<object>();

                foreach (var value in values)
                {
                    DetermineValueConversion(dataType, convertedValues, value);
                }

                return convertedValues.AsEnumerable();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private IEnumerable<Expression> OperatorDecision(MemberExpression property, IEnumerable<object> values, string op, ParameterExpression parameter)
        {
            try
            {
                IEnumerable<Expression> expressions = new List<Expression>();
                foreach (var value in values)
                {
                    expressions = operators[op].AddExpression(expressions, property, value);
                }
                return expressions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DetermineValueConversion(string dataType, List<object> convertedValues, string value)
        {
            valueTypes[dataType].AddValueType(value, ref convertedValues);
        }
    }
}
