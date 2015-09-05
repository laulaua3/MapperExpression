using System;
using System.Reflection;

namespace MapperExpression.Exception
{
    [Serializable]
    public class ReadOnlyPropertyException :System.Exception
    {

        public ReadOnlyPropertyException(PropertyInfo property)
            : base("The destination property  '" + property.Name + "' is read-only")
        {

        }
    }
}
