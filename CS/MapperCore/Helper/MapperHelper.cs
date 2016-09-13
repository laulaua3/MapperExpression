using System;
using System.Linq.Expressions;

namespace MapperExpression.Helper
{
    /// <summary>
    /// Internal helper.
    /// </summary>
    internal static class MapperHelper
    {
        /// <summary>
        /// Gets the default value of the type.
        /// </summary>
        /// <param name="typeObject">The type.</param>
        internal static object GetDefaultValue(Type typeObject)
        {
            object defaultValue = null;
            // In the case of value types (eg Integer), you must instantiate the object to have its default value.
            if (typeObject.BaseType == typeof(ValueType))
            {
                NewExpression exp = Expression.New(typeObject);
                LambdaExpression lambda = Expression.Lambda(exp);
                // Yes, we know that this is a consumer.
                Delegate constructor = lambda.Compile();
                defaultValue = constructor.DynamicInvoke();
            }
            // Case : Enum
            if (typeObject.IsEnum)
            {
                // We return the first of enum
                defaultValue =Enum.Parse(typeObject,Enum.GetNames(typeObject)[0]);
            }

            return defaultValue;
        }
    }
}
