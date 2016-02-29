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



        internal MapperConfigurationBase Find(Type source, Type target, string name = null)
        {
            IList<MapperConfigurationBase> mapConfigs = Instance.FindAll(x => x.SourceType == source && x.TargetType == target);
            MapperConfigurationBase result = null;
            if (mapConfigs.Count > 0)
            {

                if (string.IsNullOrEmpty(name))
                {
                    result = mapConfigs.FirstOrDefault(x => x.Name == x.paramClassSource.Name);
                }
                else
                {
                    result = mapConfigs.FirstOrDefault(x => x.Name == name);
                }
            }
            return result;
        }
    }
}
