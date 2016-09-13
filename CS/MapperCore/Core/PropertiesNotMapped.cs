using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace MapperExpression.Core
{
    /// <summary>
    /// Result of the treatment for the propeties not mapped.
    /// </summary>
    public class PropertiesNotMapped
    {

        internal List<PropertyInfo> sourceProperties;
        internal List<PropertyInfo> targetProperties;

        /// <summary>
        /// Gets the source properties not mapped.
        /// </summary>
        public ReadOnlyCollection<PropertyInfo> SourceProperties
        {
            get
            {
                return new ReadOnlyCollection<PropertyInfo>(sourceProperties);
            }
        }
        /// <summary>
        /// Gets the target properties not mapped.
        /// </summary>
        public ReadOnlyCollection<PropertyInfo> TargetProperties
        {
            get
            {
                return new ReadOnlyCollection<PropertyInfo>(targetProperties);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertiesNotMapped"/> class.
        /// </summary>
        public PropertiesNotMapped()
        {
            sourceProperties = new List<PropertyInfo>();
            targetProperties = new List<PropertyInfo>();
        }

    }
}
