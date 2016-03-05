using System;
using System.Linq.Expressions;

namespace MapperExpression.Helper
{
    /// <summary>
    /// Helper
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
            // Dans le cas de type valeur (ex :Integer), il faut instancier l'objet pour avoir sa valeur par défaut
            if (typeObject.BaseType == typeof(ValueType))
            {
                NewExpression exp = Expression.New(typeObject);
                LambdaExpression lambda = Expression.Lambda(exp);
                Delegate constructor = lambda.Compile();
                defaultValue = constructor.DynamicInvoke();
            }
            return defaultValue;
        }
    }
}
