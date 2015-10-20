using System;
using System.Collections.Generic;
using System.Linq;

namespace MapperExpression.Core
{
    /// <summary>
    /// Singleton storage mappers
    /// </summary>
    internal class MapperConfigurationContainer : List<MapperConfigurationBase>
    {

        private static MapperConfigurationContainer currentInstance;

        internal static MapperConfigurationContainer Instance
        {
            get
            {
                if (currentInstance == null)
                {
                    currentInstance = new MapperConfigurationContainer();
                }
                return currentInstance;
            }
        }

        private MapperConfigurationContainer()
        {

        }

        internal MapperConfigurationBase Find(Type source, Type target)
        {
            return Find(m => m.TypeSource == source && m.TypeDest == target);
        }
    }
}
