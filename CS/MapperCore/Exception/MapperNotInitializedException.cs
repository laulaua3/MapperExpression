using System;

namespace MapperExpression.Exception
{
    [Serializable]
    public  class MapperNotInitializedException :System.Exception
    {
      public MapperNotInitializedException(Type typeSource, Type typeDest)
          : base("Mapper for the source type '" + typeSource.FullName + "' and the type of destination '" + typeDest.FullName + "' is not initialized (called Mapper.Initialise())")
      {

      }
    }
}
