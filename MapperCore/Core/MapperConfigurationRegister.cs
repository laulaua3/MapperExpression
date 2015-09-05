using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapperCore.Core
{
    /// <summary>
    /// Singleton des mappeurs
    /// </summary>
    internal class MapperConfigurationRegister : List<MapperConfigurationBase>
    {


        private static MapperConfigurationRegister _instance;

        internal static MapperConfigurationRegister Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MapperConfigurationRegister();
                }
                return _instance;
            }
        }

        private MapperConfigurationRegister()
        {

        }

        internal MapperConfigurationBase Find(Type source, Type destination)
        {
            return this.FirstOrDefault(m => m.TypeSource == source && m.TypeDest == destination);
        }
    }
}
