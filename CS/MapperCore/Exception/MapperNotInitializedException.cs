using System;

namespace MapperExpression.Exception
{
    [Serializable]
    public  class MapperNotInitializedException :System.Exception
    {
      public MapperNotInitializedException(Type typeSource, Type typeDest)
          : base("Le mappeur pour le type source '" + typeSource.FullName + "' et le type de destination '" + typeDest.FullName + "' n'est pas initialisé (appelé Mapper.Initialise()")
      {

      }
    }
}
