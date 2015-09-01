using System;

namespace MapperExpression.Exception
{
    [Serializable]
    public class MapperExistException:System.Exception
   
    {

        public MapperExistException(Type source,Type dest)
            : base("A mapper already exists for the type of source '" + source.FullName + "' and the type of destination '" + dest.FullName + "'")
        {

        }
    }
}
