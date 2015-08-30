using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapperExpression.Tests.Units.ClassTests;
using MapperExpression.Core;
using MapperExpression;
using MapperExpression.Exception;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace MapperExpression.Tests.Units
{
    [TestClass]
    public class MapperConfigurationTests
    {

        [TestMethod, TestCategory("Ignore")]
        public void Ignore_Add_Succes()
        {
            MapperConfigurationTestContainer actual = null;

            actual = new MapperConfigurationTestContainer();
            actual.Ignore((d) => d.PropInt1);
            Assert.AreEqual(actual.GetIgnoreCount(), 1);
        }
        [TestMethod, TestCategory("AfterMap")]
        public void AfterMap_Add_Succes()
        {
            MapperConfigurationTestContainer actual = null;

            actual = new MapperConfigurationTestContainer();
            actual.AfterMap((s, d) =>
            {
                //Nothing To Do
            });
            Assert.AreEqual(actual.GetAfterMapActionCount(), 1);
        }
        [TestMethod, TestCategory("ExecuteAfterActions")]
        public void ExecuteAfterActions_Succes()
        {
            MapperConfigurationTestContainer actual = null;
            bool excecutedAction = false;
            actual = new MapperConfigurationTestContainer();
            actual.AfterMap((s, d) =>
            {
                excecutedAction = true;

            });
            actual.ExecuteAfterActions(new ClassSource(), new ClassDest());
            Assert.IsTrue(excecutedAction);
        }


        [TestMethod, TestCategory("ReverseMap")]
        public void ReverseMap_Success()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            MapperConfiguration<ClassDest, ClassSource> actual = null;

            actual = expected.ReverseMap();

            Assert.IsInstanceOfType(actual, typeof(MapperConfiguration<ClassDest, ClassSource>));

        }

        [TestMethod, TestCategory("ReverseMap"), ExpectedException(typeof(MapperExistException))]
        public void ReverseMap_MapperAlreadyExist_Exception()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            MapperConfiguration<ClassDest, ClassSource> actual = null;
            //For init the first reverse mapping
            expected.ReverseMap();

            actual = expected.ReverseMap();
            //For remove the reverse mapping (for the others tests)
            MapperConfigurationContainer.Instance.RemoveAt(1);
        }

        [TestMethod, TestCategory("Exception"), ExpectedException(typeof(PropertyNoExistException))]
        public void PropertyNoExistException_Exception()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            expected.GetPropertyInfoSource("test");
        }

        [TestMethod, TestCategory("Exception"), ExpectedException(typeof(NotSameTypePropertyException))]
        public void NotSameTypePropertyException_Exception()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            expected.ForMember(s => s.PropInt1, d => d.PropString2);
            expected.CreateMappingExpression(null);
        }

        [TestMethod, TestCategory("Exception"), ExpectedException(typeof(ReadOnlyPropertyException))]
        public void ReadOnlyPropertyExceptionException_Exception()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            expected.ForMember(s => s.PropInt1, d => d.RealOnlyPropInt1);
            expected.CreateMappingExpression(null);
        }

        [TestMethod, TestCategory("GetSortedExpression")]
        public void GetSortedExpression_PropertyFound_Succes()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            expected.CreateMappingExpression(null);
            LambdaExpression actual = null;
            actual = expected.GetSortedExpression("PropInt1");
            Assert.IsNotNull(actual);
        }

        [TestMethod, TestCategory("GetSortedExpression")ExpectedException(typeof(PropertyNoExistException))]
        public void GetSortedExpression_PropertyNotFound_Exception()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            expected.CreateMappingExpression(null);
            LambdaExpression actual = null;
            actual = expected.GetSortedExpression("PropNotExist");

        }

        [TestMethod, TestCategory("GetMapper"), ExpectedException(typeof(NoFoundMapperException))]
        public void GetMapper_NoFoundMapperException()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            expected.GetMapperTest(typeof(string), typeof(string), true);
        }

        [TestMethod, TestCategory("CreateMappingExpression")]
        public void CreateMappingExpression_NotInitialise()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            expected.CreateMappingExpression(null);
            int actual = expected.MemberToMap.Count;
            Assert.IsTrue(actual > 0);

        }
        [TestMethod, TestCategory("CheckAndConfigureTypeOfListTest")]
        public void CheckAndConfigureTypeOfListTest_IsList_NotSameType_ReturnTrue()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            PropertyInfo memberSource = typeof(ClassSource).GetProperty("ListProp");
            PropertyInfo memberDest = typeof(ClassDest).GetProperty("ListProp");
            Mapper.CreateMap<ClassSource2, ClassDest2>();
            Mapper.Initialize();

            bool actual = expected.CheckAndConfigureTypeOfListTest(memberSource, memberDest);
            Assert.IsTrue(actual);
            Mapper.Reset();
        }

        [TestMethod, TestCategory("CheckAndConfigureTypeOfListTest"), ExpectedException(typeof(NoFoundMapperException))]
        public void CheckAndConfigureTypeOfListTest_IsList_NotSameType_NoMapperFound_Exception()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            PropertyInfo memberSource = typeof(ClassSource).GetProperty("ListProp");
            PropertyInfo memberDest = typeof(ClassDest).GetProperty("ListProp");
            bool actual = expected.CheckAndConfigureTypeOfListTest(memberSource, memberDest);


        }

        [TestMethod, TestCategory("CheckAndConfigureMembersMapping")]
        public void CheckAndConfigureMembersMappingTest_SameTypeOfProperty_Success()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            PropertyInfo memberSource = typeof(ClassSource).GetProperty("PropInt1");
            PropertyInfo memberDest = typeof(ClassDest).GetProperty("PropInt1");
            expected.CheckAndConfigureMembersMappingTest(memberSource, memberDest);

            Assert.IsTrue(expected.MemberToMap.Count > 0);

        }

        [TestMethod, TestCategory("CheckAndConfigureMembersMapping")]
        public void CheckAndConfigureMembersMappingTest_SameDeclarativeTypeSource_OfProperty_Success()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            PropertyInfo memberSource = typeof(ClassSource).GetProperty("Same").PropertyType.GetProperty("PropInt1");
            PropertyInfo memberDest = typeof(ClassDest).GetProperty("PropInt1");
            
            expected.CheckAndConfigureMembersMappingTest(memberSource, memberDest);

            Assert.IsTrue(expected.MemberToMap.Count > 0);
        }
        [TestMethod, TestCategory("CheckAndConfigureMembersMapping")]
        public void CheckAndConfigureMembersMappingTest_SameProperty_NotSameType_WithOutMapper_Success()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            PropertyInfo memberSource = typeof(ClassSource).GetProperty("SubClass").PropertyType.GetProperty("PropInt1");
            PropertyInfo memberDest = typeof(ClassDest).GetProperty("PropInt1");
            expected.ForMember(s => s.SubClass.PropInt1, d => d.PropInt1);
            expected.CheckAndConfigureMembersMappingTest(memberSource, memberDest);

            Assert.IsTrue(expected.MemberToMap.Count > 0);
        }
        [TestMethod, TestCategory("CheckAndConfigureMembersMapping")]
        public void CheckAndConfigureMembersMappingTest_SameNameProperty_NotSameType_WithMapper_Success()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            PropertyInfo memberSource = typeof(ClassSource).GetProperty("SubClass");
            PropertyInfo memberDest = typeof(ClassDest).GetProperty("SubClass");
            expected.ForMember(s => s.SubClass, d => d.SubClass);
            Mapper.CreateMap<ClassSource2, ClassDest2>();
            Mapper.Initialize();
            expected.CheckAndConfigureMembersMappingTest(memberSource, memberDest);

            Assert.IsTrue(expected.MemberToMap.Count > 0);
        }
        [TestMethod, TestCategory("CheckAndConfigureMembersMapping"),ExpectedException(typeof(NotSameTypePropertyException))]
        public void CheckAndConfigureMembersMappingTest_NotSameNameProperty_NotSameType_WithOutMapper_Exception()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            PropertyInfo memberSource = typeof(ClassSource).GetProperty("SubClass").PropertyType.GetProperty("SameParent");
            PropertyInfo memberDest = typeof(ClassDest).GetProperty("SubClass2");
            
            expected.CheckAndConfigureMembersMappingTest(memberSource, memberDest);

            
        }
       
        [TestMethod, TestCategory("GetPropertyInfo")]
        public void GetPropertyInfo_PropertyFound_Success()
        {
            PropertyInfo actual = null;
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            Expression<Func<ClassSource, object>> exp = x => x.PropInt1;
            actual= expected.GetPropertyInfoTest(exp);

            Assert.AreEqual(actual.Name, "PropInt1");

        }
        [TestMethod, TestCategory("GetPropertyInfo"),ExpectedException(typeof(NotImplementedException))]
        public void GetPropertyInfo_PropertyNotImplementedException()
        {
            PropertyInfo actual = null;
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            Expression<Func<ClassDest, object>> exp = x => x.PropInt1 > 0;
            actual = expected.GetPropertyInfoTest(exp);

           

        }
        [TestMethod, TestCategory("GetPropertyInfo"), ExpectedException(typeof(NotImplementedException))]
        public void GetPropertyInfo_PropertyNotImplementedExceptionDefault()
        {
            PropertyInfo actual = null;
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            Expression<Func<ClassDest, object>> exp = x => null;
            
            actual = expected.GetPropertyInfoTest(exp);

        }
        [TestMethod, TestCategory("ChangeSource")]
        public void ChangeSource_Initialised()
        {
            List<MemberAssignment> actual = null;
            PropertyInfo test = typeof(ClassSource).GetProperty("Same");
            ParameterExpression para = Expression.Parameter(typeof(ClassSource), "s");
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            expected.CreateMappingExpression(null);
            actual = expected.ChangeSourceTest(test, para);
            Assert.IsTrue(actual.Count > 0);

        }
        [TestMethod, TestCategory("ChangeSource")]
        public void ChangeSource_NotInitialised()
        {
            List<MemberAssignment> actual = null;
            PropertyInfo test = typeof(ClassSource).GetProperty("Same");
            ParameterExpression para = Expression.Parameter(typeof(ClassSource), "s");
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            
            actual = expected.ChangeSourceTest(test, para);
            Assert.IsTrue(actual.Count > 0);

        }
        [TestMethod, TestCategory("CreateCheckIfNull")]
        public void CreateCheckIfNull_Success()
        {
            List<MemberAssignment> actual = null;
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            PropertyInfo source = typeof(ClassSource).GetProperty("SubClass");
            PropertyInfo dest = typeof(ClassDest).GetProperty("SubClass");
       
            
            var mapperExt = new MapperConfiguration<ClassSource2, ClassDest2>();
            mapperExt.CreateMappingExpression(null);
            expected.CreateCheckIfNullTest(source, dest, mapperExt);
            actual = expected.MemberToMap;
            Assert.IsTrue(actual.Count > 0);

        }

        [TestMethod, TestCategory("CreateCommonMember")]
        public void CreateCommonMember_IgnoreProperty()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            expected.Ignore(d => d.PropInt1);
            expected.CreateCommonMemberTest();
            
        }
        [TestMethod, TestCategory("CheckAndRemoveMemberSource")]
        public void CheckAndRemoveMemberSource_PropertyExist_Remove()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            expected.CreateMappingExpression(null);

            int countOri = expected.MemberToMap.Count;

            expected.CheckAndRemoveMemberSourceTest("PropInt1");

            Assert.AreNotEqual(countOri, expected.MemberToMap.Count);
        }
        [TestMethod, TestCategory("CheckAndRemoveMemberDest")]
        public void CheckAndRemoveMemberDest_PropertyExist_Remove()
        {
            MapperConfigurationTestContainer expected = new MapperConfigurationTestContainer();
            expected.CreateMappingExpression(null);

            int countOri = expected.MemberToMap.Count;

            expected.CheckAndRemoveMemberDestTest("PropInt1");

            Assert.AreNotEqual(countOri, expected.MemberToMap.Count);
        }
    }

}
