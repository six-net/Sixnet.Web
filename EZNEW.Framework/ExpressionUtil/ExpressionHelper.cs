using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Framework.ExpressionUtil
{
    public static class ExpressionHelper
    {
        static ConcurrentDictionary<string, dynamic> propertyOrFieldFunctions = new ConcurrentDictionary<string, dynamic>();//Property or Field access Methods
        static Type baseExpressType = typeof(Expression);// Expression base type
        static MethodInfo lambdaMethod = null;//generate lambda method

        static ExpressionHelper()
        {
            var baseExpressMethods = baseExpressType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            lambdaMethod = baseExpressMethods.FirstOrDefault(c => c.Name == "Lambda" && c.IsGenericMethod && c.GetParameters()[1].ParameterType.FullName == typeof(ParameterExpression[]).FullName);
        }

        public static string GetExpressionText(string expression)
        {
            return
                String.Equals(expression, "model", StringComparison.OrdinalIgnoreCase)
                    ? String.Empty // If it's exactly "model", then give them an empty string, to replicate the lambda behavior
                    : expression;
        }

        public static string GetExpressionText(LambdaExpression expression)
        {
            // Split apart the expression string for property/field accessors to create its name
            Stack<string> nameParts = new Stack<string>();
            Expression part = expression.Body;
            while (part != null)
            {
                if (part.NodeType == ExpressionType.Call)
                {
                    MethodCallExpression methodExpression = (MethodCallExpression)part;

                    if (!IsSingleArgumentIndexer(methodExpression))
                    {
                        break;
                    }
                    nameParts.Push(
                        GetIndexerInvocation(
                            methodExpression.Arguments.Single(),
                            expression.Parameters.ToArray()));

                    part = methodExpression.Object;
                }
                else if (part.NodeType == ExpressionType.ArrayIndex)
                {
                    BinaryExpression binaryExpression = (BinaryExpression)part;

                    nameParts.Push(
                        GetIndexerInvocation(
                            binaryExpression.Right,
                            expression.Parameters.ToArray()));

                    part = binaryExpression.Left;
                }
                else if (part.NodeType == ExpressionType.MemberAccess)
                {
                    MemberExpression memberExpressionPart = (MemberExpression)part;
                    nameParts.Push("." + memberExpressionPart.Member.Name);
                    part = memberExpressionPart.Expression;
                }
                else if (part.NodeType == ExpressionType.Parameter)
                {
                    // Dev10 Bug #907611
                    // When the expression is parameter based (m => m.Something...), we'll push an empty
                    // string onto the stack and stop evaluating. The extra empty string makes sure that
                    // we don't accidentally cut off too much of m => m.Model.
                    nameParts.Push(String.Empty);
                    part = null;
                }
                else if (part.NodeType == ExpressionType.Convert)
                {
                    part = ((UnaryExpression)part).Operand;
                }
                else
                {
                    break;
                }
            }

            // If it starts with "model", then strip that away
            if (nameParts.Count > 0 && String.Equals(nameParts.Peek(), ".model", StringComparison.OrdinalIgnoreCase))
            {
                nameParts.Pop();
            }

            if (nameParts.Count > 0)
            {
                return nameParts.Aggregate((left, right) => left + right).TrimStart('.');
            }

            return String.Empty;
        }

        private static string GetIndexerInvocation(Expression expression, ParameterExpression[] parameters)
        {
            Expression converted = Expression.Convert(expression, typeof(object));
            ParameterExpression fakeParameter = Expression.Parameter(typeof(object), null);
            Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(converted, fakeParameter);
            Func<object, object> func;

            try
            {
                func = CachedExpressionCompiler.Process(lambda);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            return "[" + Convert.ToString(func(null), CultureInfo.InvariantCulture) + "]";
        }

        internal static bool IsSingleArgumentIndexer(Expression expression)
        {
            MethodCallExpression methodExpression = expression as MethodCallExpression;
            if (methodExpression == null || methodExpression.Arguments.Count != 1)
            {
                return false;
            }
            return methodExpression.Method
                .DeclaringType
                .GetDefaultMembers()
                .OfType<PropertyInfo>()
                .Any(p => p.GetGetMethod() == methodExpression.Method);
        }

        #region get expression value

        /// <summary>
        /// get expression value
        /// </summary>
        /// <param name="valueExpression">expression</param>
        /// <returns>value</returns>
        public static object GetExpressionValue(Expression valueExpression)
        {
            object value = null;
            switch (valueExpression.NodeType)
            {
                case ExpressionType.Constant:
                    value = ((ConstantExpression)valueExpression).Value;
                    break;
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.ArrayIndex:
                case ExpressionType.ArrayLength:
                case ExpressionType.Call:
                case ExpressionType.Coalesce:
                case ExpressionType.Conditional:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Decrement:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Increment:
                case ExpressionType.Invoke:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.New:
                case ExpressionType.Not:
                case ExpressionType.NotEqual:
                case ExpressionType.OnesComplement:
                case ExpressionType.Or:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.MemberAccess:
                    value = Expression.Lambda(valueExpression).Compile().DynamicInvoke();
                    break;
                default:
                    break;
            }
            return value;
        }

        #endregion

        #region verify whether is compare node type

        /// <summary>
        /// verify whether is compare node type
        /// </summary>
        /// <param name="nodeType">node type</param>
        /// <returns>is compare node type</returns>
        public static bool IsCompareNodeType(ExpressionType nodeType)
        {
            bool compareNode = false;
            switch (nodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    compareNode = true;
                    break;
            }
            return compareNode;
        }

        #endregion

        #region verify whether is boolean node type

        /// <summary>
        /// verify whether is boolean node type
        /// </summary>
        /// <param name="nodeType">node type</param>
        /// <returns>is boolean nodetype</returns>
        public static bool IsBoolNodeType(ExpressionType nodeType)
        {
            bool boolNode = false;
            switch (nodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    boolNode = true;
                    break;
            }
            return boolNode;
        }

        #endregion

        #region get property name by expression

        /// <summary>
        /// get property name by expression
        /// </summary>
        public static string GetExpressionPropertyName(Expression propertyExpression)
        {
            string name = string.Empty;
            switch (propertyExpression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    name = (propertyExpression as MemberExpression).Member.Name;
                    break;
                case ExpressionType.Constant:
                    var constantExpression = propertyExpression as ConstantExpression;
                    if (constantExpression.Type == typeof(MethodInfo))
                    {
                        name = (constantExpression.Value as MethodInfo).Name;
                    }
                    break;
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    UnaryExpression unaryExp = propertyExpression as UnaryExpression;
                    if (unaryExp.Operand.NodeType == ExpressionType.MemberAccess)
                    {
                        name = (unaryExp.Operand as MemberExpression).Member.Name;
                    }
                    break;
            }
            return name;
        }

        /// <summary>
        /// get property or field name by expression
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="propertyOrField">property or field expression</param>
        /// <returns>property or field name</returns>
        public static string GetExpressionPropertyName<T>(Expression<Func<T, dynamic>> propertyOrField)
        {
            return GetExpressionPropertyName(propertyOrField.Body);
        }

        /// <summary>
        /// get property or field name list by expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyOrFields">property or field expression</param>
        /// <returns>name list</returns>
        public static List<string> GetExpressionPropertyNames<T>(params Expression<Func<T, dynamic>>[] propertyOrFields)
        {
            if (propertyOrFields == null || propertyOrFields.Length <= 0)
            {
                return new List<string>(0);
            }
            return propertyOrFields.Select(c => GetExpressionPropertyName(c)).ToList();
        }

        #endregion

        #region get the children expression

        /// <summary>
        /// get the children expression
        /// </summary>
        /// <param name="expression">parent expression</param>
        /// <returns>children expression</returns>
        public static Expression GetLastChildExpression(Expression expression)
        {
            if (expression == null)
            {
                return expression;
            }
            Expression childExpression = expression;
            if (expression.CanReduce)
            {
                return GetLastChildExpression(childExpression.Reduce());
            }
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    childExpression = GetLastChildExpression((expression as LambdaExpression)?.Body);
                    break;
                case ExpressionType.Constant:
                case ExpressionType.Parameter:
                case ExpressionType.Conditional:
                case ExpressionType.DebugInfo:
                case ExpressionType.Default:
                case ExpressionType.Dynamic:
                case ExpressionType.Goto:
                case ExpressionType.Index:
                case ExpressionType.Label:
                case ExpressionType.MemberInit:
                case ExpressionType.New:
                case ExpressionType.NewArrayBounds:
                case ExpressionType.NewArrayInit:
                default:
                    break;
                case ExpressionType.Invoke:
                    childExpression = GetLastChildExpression((expression as InvocationExpression)?.Expression);
                    break;
                case ExpressionType.MemberAccess:
                    var memberExpression = expression as MemberExpression;
                    if (memberExpression?.Expression != null)
                    {
                        childExpression = GetLastChildExpression(memberExpression.Expression);
                    }
                    break;
                case ExpressionType.ArrayLength:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    childExpression = GetLastChildExpression((expression as UnaryExpression)?.Operand);
                    break;
                case ExpressionType.Call:
                    childExpression = GetLastChildExpression((expression as MethodCallExpression)?.Object);
                    break;
            }
            return childExpression ?? expression;
        }

        #endregion

        #region get the access method with the specified type

        /// <summary>
        /// get the access method with the specified type
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="name">field name</param>
        /// <returns></returns>
        public static Func<T, dynamic> GetPropertyOrFieldFunction<T>(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }
            Type type = typeof(T);
            string funcKey = string.Format("{0}_{1}", type.GUID, name);
            if (propertyOrFieldFunctions.TryGetValue(funcKey, out dynamic funcValue))
            {
                return funcValue;
            }
            ParameterExpression parExp = Expression.Parameter(type);//parameter expression
            Array parameterArray = Array.CreateInstance(typeof(ParameterExpression), 1);
            parameterArray.SetValue(parExp, 0);
            string[] propertyNameArray = name.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            Expression propertyExpress = null;
            foreach (string pname in propertyNameArray)
            {
                if (propertyExpress == null)
                {
                    propertyExpress = Expression.PropertyOrField(parExp, pname);
                }
                else
                {
                    propertyExpress = Expression.PropertyOrField(propertyExpress, pname);
                }
            }
            Type funcType = typeof(Func<,>).MakeGenericType(type, typeof(object));// make method
            var genericLambdaMethod = lambdaMethod.MakeGenericMethod(funcType);
            var lambdaExpression = genericLambdaMethod.Invoke(null, new object[]
            {
                Expression.Convert(propertyExpress,typeof(object)),parameterArray
            }) as Expression<Func<T, object>>;
            if (lambdaExpression == null)
            {
                return null;
            }
            var function = lambdaExpression.Compile();
            propertyOrFieldFunctions[funcKey] = function;
            return function;
        }

        #endregion
    }
}
