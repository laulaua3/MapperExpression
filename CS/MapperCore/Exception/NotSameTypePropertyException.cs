
using System;
namespace MapperExpression.Exception
{
    [Serializable]
    public class NotSameTypePropertyException : System.Exception
    {
       
        public NotSameTypePropertyException(Type typeSource, Type typeDest)
            : base("The source and destination properties are not the same type (source type " + typeSource.Name + " destination and type " + typeDest.Name + ")")
        {

        }
    }
}
