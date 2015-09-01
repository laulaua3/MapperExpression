using System;

namespace MapperExpression.Exception
{
    [Serializable]
    public class NoFoundMapperException : System.Exception
    {

        public NoFoundMapperException(Type source, Type dest)
            : base("The mapping for the types '" + source.Name + "' et '" + dest.Name + "' are not configured")
        {

        }        
    }
}
