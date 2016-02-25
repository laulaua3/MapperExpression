using System;
using System.Collections.Generic;
using System.Linq;

namespace MapperExpression.Core
{
    /// <summary>
    /// Singleton storage mappers
    /// </summary>
    /// <remarks>Don't need a lazy singleton because this list is for all thread</remarks>
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
            return Find(m => m.SourceType == source && m.TargetType == target);
        }
    }
}
