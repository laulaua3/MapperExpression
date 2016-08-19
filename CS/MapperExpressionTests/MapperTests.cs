
using MapperExpression.Core;
using MapperExpression.Exceptions;
using MapperExpression.Tests.Units.ClassTests;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace MapperExpression.Tests.Units
{
    [TestClass]
    public class MapperTests
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

        [TestMethod, TestCategory("CreateMap")]
        public void Mapper_CreateMap_NotExist_ContainerCount1()
        {
            Clean();
            //Create the default map for the test
            Mapper.CreateMap<ClassSource, ClassDest>()
                .ForMember(s => s.PropString1, d => d.PropString2);

            Assert.AreEqual(MapperConfigurationCollectionContainer.Instance.Count, 1);
        }

        [TestMethod, TestCategory("CreateMap")]
        public void Mapper_CreateMap_Already_Exist_ContainerCount1()
        {
            //Create a other map configuration with the same parameter.
            Mapper.CreateMap<ClassSource, ClassDest>();

            Assert.IsTrue(MapperConfigurationCollectionContainer.Instance.Exists(m => m.SourceType == typeof(ClassSource) && m.TargetType == typeof(ClassDest)));
        }
        [TestMethod, TestCategory("CreateMap")]
        public void Mapper_CreateMap_With_Name()
        {
            //Create a other map configuration with the same parameter.
            Mapper.CreateMap<ClassSource, ClassDest>("test");

            Assert.IsTrue(MapperConfigurationCollectionContainer.Instance.Exists(m => m.Name == "test"));
        }
        [TestMethod, TestCategory("Map")]
        public void Map_ReturnDestinationObject_Success()
        {

            Mapper.Initialize();

            ClassDest actual = null;
            ClassSource expected = new ClassSource() { PropInt1 = 1, PropSourceInt1 = 1, PropString1 = "test" };

            actual = Mapper.Map<ClassSource, ClassDest>(expected);

            Assert.AreEqual(actual.PropInt1, expected.PropInt1);
            Assert.AreEqual(actual.PropString2, expected.PropString1);
            Assert.AreEqual(actual.PropInt2, 0);
        }

        [TestMethod, TestCategory("Map"), ExpectedException(typeof(MapperNotInitializedException))]
        public void Map_MapperNotInitialise_Exception()
        {

            Mapper.CreateMap<ClassSource, ClassDest>()
               .ForMember(s => s.PropString1, d => d.PropString2);
            ClassDest actual = null;
            ClassSource expected = new ClassSource() { PropInt1 = 1, PropSourceInt1 = 1, PropString1 = "test" };
            using (ShimsContext.Create())
            {
                MapperExpression.Core.Fakes.ShimMapperConfiguration<ClassSource, ClassDest>.AllInstances.GetFuncDelegate = (s) =>
                {
                    throw new MapperNotInitializedException(typeof(ClassSource), typeof(ClassDest));
                };

                actual = Mapper.Map<ClassSource, ClassDest>(expected);
            }
        }

        [TestMethod, TestCategory("Map")]
        public void Map_Return_null()
        {
            ClassDest actual = null;
            ClassSource expected = new ClassSource() { PropInt1 = 1, PropSourceInt1 = 1, PropString1 = "test" };
            using (ShimsContext.Create())
            {
                MapperExpression.Core.Fakes.ShimMapperConfiguration<ClassSource, ClassDest>.AllInstances.GetFuncDelegate = (s) => { return null; };

                actual = Mapper.Map<ClassSource, ClassDest>(expected);
            }


            Assert.IsNull(actual);
        }

        [TestMethod, TestCategory("GetQueryExpression")]
        public void GetQueryExpression_ReturnExpression()
        {
            Clean();
            
            Mapper.CreateMap<ClassSource, ClassDest>()
                .ForMember(s => s.PropString1, d => d.PropString2);

            Mapper.Initialize();

            Expression<Func<ClassSource, ClassDest>> actual = null;

            actual = Mapper.GetQueryExpression<ClassSource, ClassDest>();

            Assert.IsNotNull(actual);
        }

        [TestMethod, TestCategory("GetQuery")]
        public void GetQuery_ReturnFunc()
        {
            Init(null);
            Mapper.Initialize();

            Func<ClassSource, ClassDest> actual = null;

            actual = Mapper.GetQuery<ClassSource, ClassDest>();

            Assert.IsNotNull(actual);
        }

        [TestMethod, TestCategory("Exception"), ExpectedException(typeof(NoFoundMapperException))]
        public void Map_NoFoundMapperException_Exception()
        {
            ClassDest2 actual = null;
            actual = Mapper.Map<ClassSource, ClassDest2>(new ClassSource());
        }

        [TestMethod, TestCategory("Exception"), ExpectedException(typeof(NoActionAfterMappingException))]
        public void Map_NoActionException_Exception()
        {
            ClassDest actual = null;
            Mapper.GetMapper<ClassSource, ClassDest>().AfterMap(null);
            Mapper.Initialize();
            actual = Mapper.Map<ClassSource, ClassDest>(new ClassSource());
            Clean();
        }

        [TestMethod]
        public void GetPropertiesNotMapped_ReturnProperties_Success()
        {

            PropertiesNotMapped actual = null;
            Mapper.Initialize();
            actual = Mapper.GetPropertiesNotMapped<ClassSource, ClassDest>();
            Assert.IsTrue(actual.SourceProperties.Count > 0);
            Assert.IsTrue(actual.TargetProperties.Count > 0);
        }
      
        //[TestMethod]
        //public void Map_ExistingObject_Success()
        //{
        //    ClassDest actual = new ClassDest();
        //    ClassSource expected = Builder<ClassSource>.CreateNew()
        //        .With(x => x.SubClass = Builder<ClassSource2>.CreateNew().Build())
        //        .Build();
        //    Mapper.CreateMap<ClassSource, ClassDest>()
        //        .ForMember(s => s.SubClass, d => d.SubClass);
        //    Mapper.CreateMap<ClassSource2, ClassDest2>();
        //    Mapper.Initialize();
        //    Mapper.Map(expected, actual);
        //    Assert.IsNotNull(actual);
        //    Assert.AreEqual(actual.PropInt1, expected.PropInt1);
        //    Assert.AreEqual(actual.PropString2, expected.PropString1);
        //    Clean();
        //}
    }
}
