using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapperExpression.Extensions;
using System.Linq.Expressions;
using MapperExpression.Tests.Units.ClassTests;

namespace MapperExpression.Tests.Units.Extentions
{
    [TestClass]
    public class MapperExtentionsTest
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Clean();
            //Create the default map for the test
            Mapper.CreateMap<ClassSource, ClassDest>()
                .ForMember(s => s.PropString1, d => d.PropString2)
                .ForMember(s => s.SubClass, d => d.SubClass)
                .ReverseMap();
            Mapper.CreateMap<ClassDest2, ClassSource2>()
                .ForMember(s => s.PropString2, d => d.PropString1)
                .ReverseMap();
            Mapper.Initialize();
        }
        [ClassCleanup]
        public static void Clean()
        {
            //Remove all map after test
            Mapper.Reset();
        }
        [TestMethod]
        public void ConvertTo_SimpleExpression_Success()
        {
            Expression<Func<ClassSource, bool>> actual = null;
            Expression<Func<ClassDest, bool>> expected = x => x.PropString2 == "test";

            actual = expected.ConvertTo<ClassDest, ClassSource>();
            var test = actual.Body as BinaryExpression;
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(test.Left, typeof(MemberExpression));
            Assert.AreEqual((test.Left as MemberExpression).Member.ReflectedType, typeof(ClassSource));
            Assert.AreEqual((test.Left as MemberExpression).Member.Name, "PropString1");
        }
    }
}
