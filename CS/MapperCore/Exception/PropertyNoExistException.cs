using System;

namespace MapperExpression.Exception
{
    [Serializable]
    public class PropertyNoExistException : System.Exception
    {
        
        public PropertyNoExistException(string propertyName,Type typeObject)
            : base("The property '" + propertyName + "' does not exist for the type'" + typeObject.ToString() + "'")
        {
        
        }

    }
}
