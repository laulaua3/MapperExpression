using System;

using System.Reflection;

namespace MapperCore.Exception
{
    public class PropertyNoExistException : System.Exception
    {
        
        public PropertyNoExistException(string propertyName,Type typeObject)
            : base("La propriété '" + propertyName + "' n'existe pas pour le type '" + typeObject.ToString() + "'")
        {
        
        }

    }
}
