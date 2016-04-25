using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapperExpression.Tests.Units.ClassTests;
using FizzWare.NBuilder;
using System;

namespace MapperExpression.Tests.Units
{
    [TestClass]
    public class MapperOfTDestTest
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Clean();
            //Create the default map for the test
            Mapper.CreateMap<ClassSource, ClassDest>()
                .ForMember(s => s.PropString1, d => d.PropString2);

        }
        [ClassCleanup]
        public static void Clean()
        {
            //Remove all map after test
            Mapper.Reset();
        }
        [TestMethod]
        public void Map_Success()
        {
            ClassDest actual = null;
            ClassSource expected = Builder<ClassSource>.CreateNew().Build();
            Init(null);
            Mapper.Initialize();
            actual = Mapper<ClassDest>.Map(expected);
            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.PropInt1, expected.PropInt1);
            Assert.AreEqual(actual.PropString2, expected.PropString1);
            Clean();
        }
        //[TestMethod]
        public void Map_UnTypedSource_Success()
        {
            ClassDest actual = null;
            object expected = Builder<ClassSource>.CreateNew().Build();
            Init(null);
            Mapper.Initialize();
            actual = Mapper<ClassDest>.Map(expected);
            Assert.IsNotNull(actual);
            Clean();
        }

    }
}
