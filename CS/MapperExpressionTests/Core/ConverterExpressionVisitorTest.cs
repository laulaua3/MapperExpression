using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapperExpression.Core;
using System.Linq.Expressions;
using MapperExpression.Tests.Units.ClassTests;

namespace MapperExpression.Tests.Units.Core
{
    [TestClass]
    public class ConverterExpressionVisitorTest
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
        public void VisitMember_Expression_SimpleProperty_Success()
        {
            Init(null);
            ConverterExpressionVisitor<ClassDest, ClassSource> visitor = new ConverterExpressionVisitor<ClassDest, ClassSource>();

            Expression<Func<ClassDest, bool>> expected = x => x.PropString2 == "test";
            Expression actual = null;

            actual = visitor.Visit(expected);


            var test = actual as BinaryExpression;
            Assert.IsNotNull(test);
            Assert.IsInstanceOfType(test.Left, typeof(MemberExpression));
            Assert.AreEqual((test.Left as MemberExpression).Member.Name, "PropString1");
            Clean();
        }

        [TestMethod]
        public void VisitMember_Expression_SubClassProperty_Success()
        {
            Init(null);
            ConverterExpressionVisitor<ClassDest, ClassSource> visitor = new ConverterExpressionVisitor<ClassDest, ClassSource>();

            Expression<Func<ClassDest, bool>> expected = x => x.SubClass.PropString2 == "test";
            Expression actual = null;

            actual = visitor.Visit(expected);
            var test = actual as BinaryExpression;
            Assert.IsNotNull(test);
            Assert.IsInstanceOfType(test.Left, typeof(MemberExpression));
            Assert.AreEqual((test.Left as MemberExpression).Member.Name, "PropString1");

        }


        [TestMethod]
        public void VisitMember_Expression_SimpleProperty_MultiCondition_Success()
        {
            Init(null);
            ConverterExpressionVisitor<ClassDest, ClassSource> visitor = new ConverterExpressionVisitor<ClassDest, ClassSource>();

            Expression<Func<ClassDest, bool>> expected = x => x.PropString2 == "test" && x.PropString2 == "test3";
            Expression actual = null;

            actual = visitor.Visit(expected);


            var test = actual as BinaryExpression;
            Assert.IsNotNull(test);
            Assert.IsInstanceOfType(test.Left, typeof(BinaryExpression));
            Assert.IsInstanceOfType(test.Right, typeof(BinaryExpression));
            Assert.AreEqual(((test.Left as BinaryExpression).Left as MemberExpression).Member.Name, "PropString1");
            Assert.AreEqual(((test.Right as BinaryExpression).Left as MemberExpression).Member.Name, "PropString1");
            Clean();
        }
        [TestMethod]
        public void VisitMember_Expression_SimpleProperty_MultiCondition_SubClass_Success()
        {
            Init(null);
            ConverterExpressionVisitor<ClassDest, ClassSource> visitor = new ConverterExpressionVisitor<ClassDest, ClassSource>();

            Expression<Func<ClassDest, bool>> expected = x => x.PropString2 == "test" && x.SubClass.PropString2 == "test";
            Expression actual = null;

            actual = visitor.Visit(expected);


            var test = actual as BinaryExpression;
            Assert.IsNotNull(test);
            Assert.IsInstanceOfType(test.Left, typeof(BinaryExpression));
            Assert.IsInstanceOfType(test.Right, typeof(BinaryExpression));
            Assert.AreEqual(((test.Left as BinaryExpression).Left as MemberExpression).Member.Name, "PropString1");
            Assert.AreEqual(((test.Right as BinaryExpression).Left as MemberExpression).Member.Name, "PropString1");
            Clean();
        }
    }
}
