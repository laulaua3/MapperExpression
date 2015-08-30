

using System.Collections.Generic;

namespace MapperExpression.Tests.Units.ClassTests
{
   public class ClassSource
    {
        public int PropInt1 { get; set; }
        public int PropSourceInt1 { get; set; }
        public string PropString1 { get; set; }

        public List<ClassSource2> ListProp { get; set; }

        public ClassSourceEntityBase ClassSourceEntityBase { get; set; }

        public ClassSource Same { get; set; }

        public ClassSource2 SubClass { get; set; }
    }
}
