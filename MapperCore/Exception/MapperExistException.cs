using System;

namespace MapperCore.Exception
{
    public class MapperExistException:System.Exception
   
    {

        public MapperExistException(Type source,Type dest)
            : base("Un mappeur existe déjà pour le type source '" + source.FullName + "' et le type de destination '" + dest.FullName + "'")
        {

        }
    }
}
