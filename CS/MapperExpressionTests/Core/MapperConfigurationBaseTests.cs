

using MapperCore;
using MapperCore.Core;
using MapperCore.Exception;
using MapperExpression.Tests.Units.ClassTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MapperExpression.Tests.Units
{
    [TestClass]
    public class MapperConfigurationBaseTests
    {

        [TestMethod,TestCategory("Constructor")]
        public void NewMapperConfigurationBase_SetProperties()
        {
            MapperConfigurationBase actual = null;
            actual = new MapperConfiguration<ClassSource, ClassDest>();
            Assert.IsNotNull(actual.MemberToMap);
            Assert.AreEqual(actual.TypeDest, typeof(ClassDest));
            Assert.AreEqual(actual.TypeSource, typeof(ClassSource));
        }

        [TestMethod, TestCategory("GetDestinationType")]
        public void GetDestinationType_WithoutServiceConstructor()
        {
            Type actual = null;
            var mapper = new MapperConfiguration<ClassSource, ClassDest>();
            actual = mapper.GetDestinationType();

            Assert.AreEqual(actual, typeof(ClassDest));
        }

        [TestMethod, TestCategory("GetDestinationType")]
        public void GetDestinationType_WithServiceConstructor()
        {
            Type actual = null;
            Mapper.ConstructServicesUsing((x) => { return new ClassDest2(); });
           
            var mapper = Mapper.CreateMap<ClassSource2, ClassDest2>().ConstructUsingServiceLocator();
            Mapper.Initialize();
            actual = mapper.GetDestinationType();

            Assert.AreEqual(actual, typeof(ClassDest2));
            Mapper.Reset();
        }


        [TestMethod, TestCategory("GetDelegate"), ExpectedException(typeof(MapperNotInitializedException))]
        public void GetDelegate_MapperNotInitialise_Exception()
        {
          
            MapperConfigurationBase mapper = new MapperConfiguration<ClassSource, ClassDest>();
            
            mapper.GetDelegate();
         
        }
    }
}
