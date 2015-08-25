
using System;
namespace MapperCore.Exception
{
    public class NotSameTypePropertyException : System.Exception
    {

        public NotSameTypePropertyException(Type typeSource, Type typeDest)
            : base("Les propriétés source et de destination ne sont pas du même type(source est de type " + typeSource.Name + " et destination est de type " + typeDest.Name + ")")
        {

        }
    }
}
