
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MapperExpression.Tests.Units.ClassTests;
using MapperExpression.Core;
using MapperExpression.Exception;
using Microsoft.QualityTools.Testing.Fakes;
using System.Linq.Expressions;
using System;
using System.Linq;
using FizzWare.NBuilder;

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

            Assert.AreEqual(MapperConfigurationContainer.Instance.Count, 1);
        }

        [TestMethod, TestCategory("CreateMap")]
        public void Mapper_CreateMap_Already_Exist_ContainerCount1()
        {
            //Create a other map configuration with the same parameter.
            Mapper.CreateMap<ClassSource, ClassDest>();

            Assert.IsTrue(MapperConfigurationContainer.Instance.Exists(m => m.TypeSource == typeof(ClassSource) && m.TypeDest == typeof(ClassDest)));
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
                MapperExpression.Core.Fakes.ShimMapperConfiguration<ClassSource, ClassDest>.AllInstances.GetFuncDelegate = (s) => {
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

        //[TestMethod, TestCategory("Map"),ExpectedException(typeof(NotImplementedException))]
        //public void Mapper_CreateMap_WithCountMethodInSource_Sucess()
        //{
        //    Clean();
        //    int nbCount = 3;
        //    ClassDest actual = null;
        //    ClassSource expected = Builder<ClassSource>.CreateNew()
        //        .With(x => x.ListProp = Builder<ClassSource2>.CreateListOfSize(nbCount).Build().ToList())
        //        .Build();
        //    //Create the default map for the test
        //    Mapper.CreateMap<ClassSource, ClassDest>()
        //        .ForMember(s => s.PropString1, d => d.PropString2)
        //        .ForMember(s => s.ListProp.Count(), d => d.CountListProp);
        //    Mapper.Initialize();

        //    actual = Mapper.Map<ClassSource, ClassDest>(expected);

        //    Assert.AreEqual(actual.CountListProp, nbCount);
        //    Clean();
        //}
    }
}
