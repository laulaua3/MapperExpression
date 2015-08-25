using System;
using System.Collections.Generic;
using System.Linq;

namespace MapperCore.Core
{
    /// <summary>
    /// Singleton du stockage des mappeurs
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

        internal MapperConfigurationBase Find(Type source, Type destination)
        {
            return this.FirstOrDefault(m => m.TypeSource == source && m.TypeDest == destination);
        }
    }
}
