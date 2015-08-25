using System;
using System.Linq;
using System.Reflection;

namespace MapperCore.Exception
{
    public class ReadOnlyPropertyException :System.Exception
    {

        public ReadOnlyPropertyException(PropertyInfo property)
            : base("La propriété '" + property.Name + "' de destination est en lecture seule")
        {

        }
    }
}
