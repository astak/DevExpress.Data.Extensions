using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using DevExpress.Data.Extensions.Properties;
using DevExpress.Data.Filtering;
using System.Reflection;
using System.Collections;

namespace DevExpress.Data.Extensions {
    public class CriteriaBuilder : ExpressionVisitor {
        private CriteriaOperator Result;
        private Stack<ParameterExpression> ParametersStack = new Stack<ParameterExpression>();

        private CriteriaBuilder() { }

        public static CriteriaOperator Build<TContext, TResult>(Expression<Func<TContext, TResult>> exp) {
            return BuildCore(exp);
        }

        public static CriteriaOperator Build<TContext>(Expression<Func<TContext, bool>> exp) {
            return Build<TContext, bool>(exp);
        }

        internal static CriteriaOperator BuildCore(Expression exp) {
            CriteriaBuilder instance = new CriteriaBuilder();
            instance.Visit(exp);
            return instance.Result;
        }

        protected override Expression VisitMember(MemberExpression node) {
            if (ProcessMemberParameter(node) || ProcessMember(node) || 
                ProcessClosure(node) || ProcessSingleAggregate(node))
                return node;
            string msg = string.Format(CultureInfo.CurrentCulture, Resources.ExpressionNotSupportedException, node);
            throw new NotSupportedException(msg);
        }

        protected override Expression VisitConstant(ConstantExpression node) {
            Result = new ConstantValue(node.Value);
            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node) {
            switch (node.NodeType) {
                case ExpressionType.AndAlso:
                    Result = ExtractOperand(node.Left) & ExtractOperand(node.Right);
                    break;
                case ExpressionType.OrElse:
                    Result = ExtractOperand(node.Left) | ExtractOperand(node.Right);
                    break;
                case ExpressionType.Equal:
                    Result = ExtractOperand(node.Left) == ExtractOperand(node.Right);
                    break;
                case ExpressionType.NotEqual:
                    Result = ExtractOperand(node.Left) != ExtractOperand(node.Right);
                    break;
                case ExpressionType.GreaterThan:
                    Result = ExtractOperand(node.Left) > ExtractOperand(node.Right);
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    Result = ExtractOperand(node.Left) >= ExtractOperand(node.Right);
                    break;
                case ExpressionType.LessThan:
                    Result = ExtractOperand(node.Left) < ExtractOperand(node.Right);
                    break;
                case ExpressionType.LessThanOrEqual:
                    Result = ExtractOperand(node.Left) <= ExtractOperand(node.Right);
                    break;
                case ExpressionType.And:
                    Result = new BinaryOperator(ExtractOperand(node.Left), ExtractOperand(node.Right), BinaryOperatorType.BitwiseAnd);
                    break;
                case ExpressionType.Or:
                    Result = new BinaryOperator(ExtractOperand(node.Left), ExtractOperand(node.Right), BinaryOperatorType.BitwiseOr);
                    break;
                case ExpressionType.ExclusiveOr:
                    Result = new BinaryOperator(ExtractOperand(node.Left), ExtractOperand(node.Right), BinaryOperatorType.BitwiseXor);
                    break;
                case ExpressionType.Subtract:
                    Result = ExtractOperand(node.Left) - ExtractOperand(node.Right);
                    break;
                case ExpressionType.Add:
                    Result = ExtractOperand(node.Left) + ExtractOperand(node.Right);
                    break;
                case ExpressionType.Divide:
                    Result = ExtractOperand(node.Left) / ExtractOperand(node.Right);
                    break;
                case ExpressionType.Multiply:
                    Result = ExtractOperand(node.Left) * ExtractOperand(node.Right);
                    break;
                case ExpressionType.Modulo:
                    Result = ExtractOperand(node.Left) % ExtractOperand(node.Right);
                    break;
                case ExpressionType.Coalesce:
                    CriteriaOperator left = ExtractOperand(node.Left);
                    Result = new FunctionOperator(FunctionOperatorType.Iif, new NullOperator(left), ExtractOperand(node.Right), left);
                    break;
                default:
                    string msg = string.Format(CultureInfo.CurrentCulture, Resources.ExpressionNotSupportedException, node.NodeType);
                    throw new NotSupportedException(msg);
            }
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node) {
            if (TryEvaluateMethodCall(node)) return node;
            if (node.Method.DeclaringType == typeof(Enumerable) || node.Method.DeclaringType == typeof(Queryable)) {
                Result = MethodToAggregation(node);
            } else {
                Result = MethodToFunction(node);
            }
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node) {
            switch (node.NodeType) {
                case ExpressionType.Not:
                    Result = node.Type == typeof(bool) ? 
                        !ExtractOperand(node.Operand) : 
                        new UnaryOperator(UnaryOperatorType.BitwiseNot, ExtractOperand(node.Operand));
                    break;
                case ExpressionType.Negate:
                    Result = -ExtractOperand(node.Operand);
                    break;
                case ExpressionType.Convert:
                    Result = ConvertToFunction(node);
                    break;
                default:
                    string msg = string.Format(CultureInfo.CurrentCulture, Resources.ExpressionNotSupportedException, node.NodeType);
                    throw new NotSupportedException(msg);
            }
            return node;
        }

        protected override Expression VisitNew(NewExpression node) {
            ArrayList args = new ArrayList();
            if (PopulateArgumentValues(node.Arguments, args)) {
                Result = new ConstantValue(node.Constructor.Invoke(args.ToArray()));
                return node;
            }
            string msg = string.Format(CultureInfo.CurrentCulture, Resources.NewNotSupported, node);
            throw new NotSupportedException(msg);
        }

        protected override Expression VisitLambda<T>(Expression<T> node) {
            ParametersStack.Push(node.Parameters[0]);
            Expression result = base.VisitLambda<T>(node);
            ParametersStack.Pop();
            return result;
        }

        protected override Expression VisitConditional(ConditionalExpression node) {
            FunctionOperator result = new FunctionOperator(FunctionOperatorType.Iif);
            AppendIifOperands(result, node);
            Result = result;
            return node;
        }

        protected override Expression VisitNewArray(NewArrayExpression node) {
            ArrayList array = new ArrayList();
            foreach (Expression itemExpression in node.Expressions) {
                ConstantValue itemValue = ExtractOperand(itemExpression) as ConstantValue;
                if (IsNull(itemValue)) throw new ArrayInitRequiresConstantException(itemExpression);
                array.Add(itemValue.Value);
            }
            Result = new ConstantValue(array);
            return node;
        }

        private AggregateOperand BuildAggregation(Expression collectionProperty, Expression aggregationExpression, 
            Expression condition, Aggregate aggregationType) {
            CriteriaOperator @operator = ExtractOperand(collectionProperty);
            OperandProperty collection = @operator as OperandProperty;
            if (!IsNull(collection))
                return new AggregateOperand(collection, ExtractOperand(aggregationExpression), aggregationType, ExtractOperand(condition));
            AggregateOperand aggregation = @operator as AggregateOperand;
            if (!IsNull(aggregation)) {
                aggregation.AggregatedExpression = ExtractOperand(aggregationExpression);
                aggregation.AggregateType = aggregationType;
                return aggregation;
            }
            string msg = string.Format(CultureInfo.CurrentCulture, Resources.CollectionPropertyAbsentException, collectionProperty);
            throw new NotSupportedException(msg);
        }

        private FunctionOperator MethodToFunction(MethodCallExpression node) {
            IList<CriteriaOperator> operands = new List<CriteriaOperator>();
            if (node.Object != null)
                operands.Add(ExtractOperand(node.Object));
            foreach (Expression arg in node.Arguments)
                operands.Add(ExtractOperand(arg));
            FunctionOperatorType operatorType = FunctionOperatorType.Custom;
            switch (node.Method.Name) {
                case "ToLower":
                    operatorType = FunctionOperatorType.Lower;
                    break;
                case "ToString":
                    operatorType = FunctionOperatorType.ToStr;
                    break;
                case "Trim":
                    ThrowIfTrimArgumentsNotSupported(node);
                    operatorType = FunctionOperatorType.Trim;
                    break;
                case "ToUpper":
                    operatorType = FunctionOperatorType.Upper;
                    break;
                case "AddMilliseconds":
                    operatorType = FunctionOperatorType.AddMilliSeconds;
                    break;
                case "Add":
                    operatorType = FunctionOperatorType.AddTimeSpan;
                    break;
                case "Atan":
                    operatorType = FunctionOperatorType.Atn;
                    break;
                case "Atan2":
                    operatorType = FunctionOperatorType.Atn2;
                    break;
                case "ToChar":
                    operatorType = FunctionOperatorType.Char;
                    break;
                case "IndexOf":
                    return ExtractCharIndex(node);
                case "Pow":
                    operatorType = FunctionOperatorType.Power;
                    break;
                case "Sqrt":
                    operatorType = FunctionOperatorType.Sqr;
                    break;
                case "ToSingle":
                    operatorType = FunctionOperatorType.ToFloat;
                    break;
                case "ToInt32":
                    operatorType = FunctionOperatorType.ToInt;
                    break;
                case "ToInt64":
                    operatorType = FunctionOperatorType.ToLong;
                    break;
                default:
                    operatorType = GetOperatorTypeByMethodName(node.Method.Name);
                    if (operatorType == FunctionOperatorType.Custom)
                        return new FunctionOperator(node.Method.Name, operands);
                    break;
            }
            return new FunctionOperator(operatorType, operands);
        }

        private FunctionOperator ConvertToFunction(UnaryExpression node) {
            if (node.Type == typeof(char))
                return new FunctionOperator(FunctionOperatorType.Char, ExtractOperand(node.Operand));
            if (node.Type == typeof(decimal))
                return new FunctionOperator(FunctionOperatorType.ToDecimal, ExtractOperand(node.Operand));
            if (node.Type == typeof(double))
                return new FunctionOperator(FunctionOperatorType.ToDouble, ExtractOperand(node.Operand));
            if (node.Type == typeof(float))
                return new FunctionOperator(FunctionOperatorType.ToFloat, ExtractOperand(node.Operand));
            if (node.Type == typeof(int))
                return new FunctionOperator(FunctionOperatorType.ToInt, ExtractOperand(node.Operand));
            if (node.Type == typeof(long))
                return new FunctionOperator(FunctionOperatorType.ToLong, ExtractOperand(node.Operand));
            string msg = string.Format(CultureInfo.CurrentCulture, Resources.ConversionNotSupportedException, node.Operand.Type, node.Type);
            throw new NotSupportedException(msg);
        }

        private CriteriaOperator MethodToAggregation(MethodCallExpression node) {
            switch (node.Method.Name) {
                case "Average":
                    return BuildAggregation(SafeGetArgument(node.Arguments, 0), SafeGetArgument(node.Arguments, 1), null, Aggregate.Avg);
                case "Count":
                    return BuildAggregation(SafeGetArgument(node.Arguments, 0), null, SafeGetArgument(node.Arguments, 1), Aggregate.Count);
                case "Any":
                    return BuildAggregation(SafeGetArgument(node.Arguments, 0), null, SafeGetArgument(node.Arguments, 1), Aggregate.Exists);
                case "Max":
                    return BuildAggregation(SafeGetArgument(node.Arguments, 0), SafeGetArgument(node.Arguments, 1), null, Aggregate.Max);
                case "Min":
                    return BuildAggregation(SafeGetArgument(node.Arguments, 0), SafeGetArgument(node.Arguments, 1), null, Aggregate.Min);
                case "Sum":
                    return BuildAggregation(SafeGetArgument(node.Arguments, 0), SafeGetArgument(node.Arguments, 1), null, Aggregate.Sum);
                case "Where":
                    return BuildAggregation(SafeGetArgument(node.Arguments, 0), null, SafeGetArgument(node.Arguments, 1), Aggregate.Exists);
                case "Contains":
                    return ContainsToOperand(node);
                case "SingleOrDefault":
                    return BuildAggregation(SafeGetArgument(node.Arguments, 0), null, SafeGetArgument(node.Arguments, 1), Aggregate.Single);
                default:
                    return MethodToFunction(node);
            }
        }

        private CriteriaOperator ExtractOperand(Expression node) {
            if (node == null) return null;
            Visit(node);
            return Result;
        }

        private IEnumerable<CriteriaOperator> ExtractOperands(Expression node) {
            ConstantValue collectionValue = ExtractOperand(node) as ConstantValue;
            if (IsNull(collectionValue)) throw new ContainsRequiresArrayException(node);
            IEnumerable collection = collectionValue.Value as IEnumerable;
            if (collection == null) throw new ContainsRequiresArrayException(collectionValue);
            foreach (object item in collection)
                yield return new ConstantValue(item);
        }

        private AggregateOperand ExtractSingleAggregate(MemberExpression node) {
            AggregateOperand singleAggregate = ExtractOperand(node.Expression) as AggregateOperand;
            if (IsNull(singleAggregate) || singleAggregate.AggregateType != Aggregate.Single) {
                string msg = string.Format(CultureInfo.CurrentCulture, Resources.ExpressionNotSupportedException, node);
                throw new NotSupportedException(msg);
            }

            return singleAggregate;
        }

        private FunctionOperator ExtractCharIndex(MethodCallExpression node) {
            FunctionOperator result = new FunctionOperator(FunctionOperatorType.CharIndex,
                ExtractOperand(SafeGetArgument(node.Arguments, 0)), ExtractOperand(node.Object));
            for (int i = 1; i < node.Arguments.Count; i++)
                result.Operands.Add(ExtractOperand(node.Arguments[i]));
            return result;
        }

        private static bool IsNull(CriteriaOperator operand) {
            return object.ReferenceEquals(operand, null);
        }

        public static Expression SafeGetArgument(IList<Expression> arguments, int index) {
            return arguments.Count > index ? arguments[index] : null;
        }

        private bool ProcessMemberParameter(MemberExpression node) {
            ParameterExpression parameter = node.Expression as ParameterExpression;
            if (parameter == null) return false;
            StringBuilder nameBuilder = BuildParentTraversalChain(parameter);
            nameBuilder.Append(node.Member.Name);
            Result = new OperandProperty(nameBuilder.ToString());
            return true;
        }

        private bool ProcessMember(MemberExpression node) {
            if (node.Expression == null) {
                Result = EvaluateMember(node);
                return true;
            }
            if (node.Member.Name == "Length" && node.Expression.Type == typeof(string)) {
                Result = new FunctionOperator(FunctionOperatorType.Len, ExtractOperand(node.Expression));
                return true;
            }
            if (node.Member.Name == "Date" && node.Expression.Type == typeof(DateTime)) {
                Result = new FunctionOperator(FunctionOperatorType.GetDate, ExtractOperand(node.Expression));
                return true;
            } if (node.Member.Name == "Day" && node.Expression.Type == typeof(DateTime)) {
                Result = new FunctionOperator(FunctionOperatorType.GetDay, ExtractOperand(node.Expression));
                return true;
            } if (node.Member.Name == "DayOfWeek" && node.Expression.Type == typeof(DateTime)) {
                Result = new FunctionOperator(FunctionOperatorType.GetDayOfWeek, ExtractOperand(node.Expression));
                return true;
            } if (node.Member.Name == "DayOfYear" && node.Expression.Type == typeof(DateTime)) {
                Result = new FunctionOperator(FunctionOperatorType.GetDayOfYear, ExtractOperand(node.Expression));
                return true;
            } if (node.Member.Name == "Hour" && node.Expression.Type == typeof(DateTime)) {
                Result = new FunctionOperator(FunctionOperatorType.GetHour, ExtractOperand(node.Expression));
                return true;
            } if (node.Member.Name == "Millisecond" && node.Expression.Type == typeof(DateTime)) {
                Result = new FunctionOperator(FunctionOperatorType.GetMilliSecond, ExtractOperand(node.Expression));
                return true;
            } if (node.Member.Name == "Minute" && node.Expression.Type == typeof(DateTime)) {
                Result = new FunctionOperator(FunctionOperatorType.GetMinute, ExtractOperand(node.Expression));
                return true;
            } if (node.Member.Name == "Month" && node.Expression.Type == typeof(DateTime)) {
                Result = new FunctionOperator(FunctionOperatorType.GetMonth, ExtractOperand(node.Expression));
                return true;
            } if (node.Member.Name == "Second" && node.Expression.Type == typeof(DateTime)) {
                Result = new FunctionOperator(FunctionOperatorType.GetSecond, ExtractOperand(node.Expression));
                return true;
            } if (node.Member.Name == "TimeOfDay" && node.Expression.Type == typeof(DateTime)) {
                Result = new FunctionOperator(FunctionOperatorType.GetTimeOfDay, ExtractOperand(node.Expression));
                return true;
            } if (node.Member.Name == "Year" && node.Expression.Type == typeof(DateTime)) {
                Result = new FunctionOperator(FunctionOperatorType.GetYear, ExtractOperand(node.Expression));
                return true;
            }
            return false;
        }

        private bool ProcessSingleAggregate(MemberExpression node) {
            AggregateOperand singleAggregate = ExtractSingleAggregate(node);
            singleAggregate.AggregatedExpression = new OperandProperty(node.Member.Name);
            Result = singleAggregate;
            return true;
        }

        private bool ProcessClosure(MemberExpression node) {
            if (node == null) return false;
            ConstantExpression constantNode = node.Expression as ConstantExpression;
            if (constantNode == null) return false;
            FieldInfo fi = (FieldInfo)node.Member;
            Result = new ConstantValue(fi.GetValue(constantNode.Value));
            return true;
        }

        private StringBuilder BuildParentTraversalChain(ParameterExpression parameter) {
            StringBuilder nameBuilder = new StringBuilder();
            if (parameter == ParametersStack.Peek()) return nameBuilder;
            foreach (ParameterExpression param in ParametersStack) {
                if (param == parameter) break;
                nameBuilder.Append("^.");
            }
            return nameBuilder;
        }

        private static FunctionOperatorType GetOperatorTypeByMethodName(string methodName) {
            FunctionOperatorType result = FunctionOperatorType.Custom;
            if (CriteriaOperator.GetCustomFunction(methodName) != null) return result;
            if (Enum.TryParse<FunctionOperatorType>(methodName, out result)) return result;
            string msg = string.Format(CultureInfo.CurrentCulture, Resources.MethodNotSupportedException, methodName);
            throw new NotSupportedException(msg);
        }

        private static void ThrowIfTrimArgumentsNotSupported(MethodCallExpression node) {
            if (node.Arguments.Count > 0)
                throw new NotSupportedException(Resources.TrimMethodNotSupportedException);
        }

        private CriteriaOperator ContainsToOperand(MethodCallExpression node) {
            if (node.Object != null)
                return new FunctionOperator(FunctionOperatorType.Contains, ExtractOperand(node.Object),
                    ExtractOperand(SafeGetArgument(node.Arguments, 0)));
            else return new InOperator(ExtractOperand(SafeGetArgument(node.Arguments, 1)),
                ExtractOperands(SafeGetArgument(node.Arguments, 0)).ToArray());
        }

        private void AppendIifOperands(FunctionOperator @operator, ConditionalExpression node) {
            @operator.Operands.Add(ExtractOperand(node.Test));
            @operator.Operands.Add(ExtractOperand(node.IfTrue));
            ConditionalExpression ifFalseNode = node.IfFalse as ConditionalExpression;
            if (ifFalseNode == null)
                @operator.Operands.Add(ExtractOperand(node.IfFalse));
            else AppendIifOperands(@operator, ifFalseNode);
        }

        private bool TryEvaluateMethodCall(MethodCallExpression node) {
            object obj = null;
            ConstantExpression objexpr = node.Object as ConstantExpression;
            if (node.Object != null && objexpr == null) return false;
            if (objexpr != null)
                obj = objexpr.Value;
            ArrayList args = new ArrayList();
            if (!PopulateArgumentValues(node.Arguments, args)) return false;
            Result = new ConstantValue(node.Method.Invoke(obj, args.ToArray()));
            return true;
        }

        private bool PopulateArgumentValues(IEnumerable<Expression> arguments, IList values) {
            foreach (Expression arg in arguments) {
                ConstantExpression constant = arg as ConstantExpression;
                if (constant == null) return false;
                values.Add(constant.Value);
            }
            return true;
        }

        private ConstantValue EvaluateMember(MemberExpression node) {
            PropertyInfo pi = node.Member as PropertyInfo;
            if (pi == null) {
                FieldInfo fi = (FieldInfo)node.Member;
                return new ConstantValue(fi.GetValue(null));
            } else return new ConstantValue(pi.GetValue(null));
        }
    }

    public class ContainsRequiresArrayException : NotSupportedException {
        private static string GetMessage(object source) {
            return string.Format(CultureInfo.CurrentCulture, Resources.ContainsRequiresArrayException, source);
        }

        public ContainsRequiresArrayException(object source) : base(GetMessage(source)) { }
    }

    public class ArrayInitRequiresConstantException :NotSupportedException {
        private static string GetMessage(Expression node) {
            return string.Format(CultureInfo.CurrentCulture, Resources.ArrayInitRequiresConstantException, node);
        }

        public ArrayInitRequiresConstantException(Expression node) : base(GetMessage(node)) { }
    }
}