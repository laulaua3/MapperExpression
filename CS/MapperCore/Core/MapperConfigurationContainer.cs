using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MapperExpression.Core
{
    /// <summary>
    /// Singleton storage mappers.
    /// </summary>
    /// <remarks>Don't need a lazy singleton because this is for all thread.</remarks>
    internal class MapperConfigurationCollectionContainer :
        IEnumerable<MapperConfigurationBase>
    {

        private readonly HashSet<MapperConfigurationBase> items;
        private static MapperConfigurationCollectionContainer currentInstance;

        /// <summary>
        /// Prevents a default instance of the <see cref="MapperConfigurationCollectionContainer"/> class from being created.
        /// </summary>
        private MapperConfigurationCollectionContainer()
        {
            items = new HashSet<MapperConfigurationBase>();

        }


        internal static MapperConfigurationCollectionContainer Instance
        {
            get
            {
                if (currentInstance == null)
                {
                    currentInstance = new MapperConfigurationCollectionContainer();
                }
                return currentInstance;
            }
        }

        /// <summary>
        /// Gets the number of item.
        /// </summary>
        internal int Count
        {
            get
            {
                return items.Count;
            }
        }


        /// <summary>
        /// Gets the <see cref="MapperConfigurationBase"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="MapperConfigurationBase"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal MapperConfigurationBase this[int index]
        {
            get
            {
                if (index > items.Count)
                    throw new IndexOutOfRangeException();
                // We use this for the performance (yes it's better).
                var enumerator = GetEnumerator();

                int i = 0;
                while (enumerator.MoveNext())
                {
                    if (i == index)
                    {
                        return enumerator.Current;

                    }
                    i++;
                }
                return null;
            }
        }

        /// <summary>
        /// Finds the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="name">The name.</param>
        internal MapperConfigurationBase Find(Type source, Type target, string name = null)
        {
            // We use this for the performance (yes it's better).
            var enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                string nameMapper = string.IsNullOrEmpty(name) ? current.paramClassSource.Name : name;
                if (current.SourceType == source && current.TargetType == target && current.Name == nameMapper)
                    return current;
            }
            return null;

        }

        /// <summary>
        /// Whether mapping exists from the predicate.
        /// </summary>
        /// <param name="match">The predigate.</param>
        internal bool Exists(Func<MapperConfigurationBase, bool> match)
        {
            var enumerator = GetEnumerator();
            // We use this for the performance (yes it's better).
            while (enumerator.MoveNext())
            {
                if (match(enumerator.Current))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes at the index.
        /// </summary>
        /// <param name="index">The index.</param>
        internal void RemoveAt(int index)
        {
           
            MapperConfigurationBase itemToDelete = this[index];
            if (itemToDelete != null)
            {
                items.Remove(itemToDelete);
            }
        }

        /// <summary>
        /// Clears all mappers.
        /// </summary>
        internal void Clear()
        {
            items.Clear();
        }

        public void Add(MapperConfigurationBase value)
        {
            items.Add(value);
        }

        public IEnumerator<MapperConfigurationBase> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}