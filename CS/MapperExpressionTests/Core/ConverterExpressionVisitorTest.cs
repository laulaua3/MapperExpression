using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapperExpression.Core;
using System.Linq.Expressions;
using MapperExpression.Tests.Units.ClassTests;
using System.Linq;
using System.Collections.Generic;

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
            Expression<Func<ClassDest, bool>> expected = x => x.PropString2 == "test";
            ConverterExpressionVisitor visitor = new ConverterExpressionVisitor(GetParametersDictionnary(expected, typeof(ClassSource)), typeof(ClassSource));

            
            Expression actual = null;

            actual = visitor.Visit(expected);


            var test = actual as BinaryExpression;
            Assert.IsNotNull(test);
            Assert.IsInstanceOfType(test.Left, typeof(MemberExpression));
            Assert.AreEqual((test.Left as MemberExpression).Member.ReflectedType, typeof(ClassSource));
            Assert.AreEqual((test.Left as MemberExpression).Member.Name, "PropString1");
            Clean();
        }

        [TestMethod]
        public void VisitMember_Expression_SubClassProperty_Success()
        {
            Init(null);
         
            Expression<Func<ClassSource, bool>> expected = x => x.SubClass.PropString1 == "test";
            ConverterExpressionVisitor visitor = new ConverterExpressionVisitor(GetParametersDictionnary(expected, typeof(ClassDest)), typeof(ClassDest));
            Expression actual = null;

            actual = visitor.Visit(expected);
            var test = actual as BinaryExpression;
            Assert.IsNotNull(test);
            Assert.IsInstanceOfType(test.Left, typeof(MemberExpression));
            Assert.AreEqual((test.Left as MemberExpression).Member.ReflectedType, typeof(ClassDest2));
            Assert.AreEqual((test.Left as MemberExpression).Member.Name, "PropString2");

        }


        [TestMethod]
        public void VisitMember_Expression_SimpleProperty_MultiCondition_Success()
        {
            Init(null);
            Expression<Func<ClassDest, bool>> expected = x => x.PropString2 == "test" && x.PropString2 == "test3";
            ConverterExpressionVisitor visitor = new ConverterExpressionVisitor(GetParametersDictionnary(expected, typeof(ClassSource)), typeof(ClassSource));

          
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
            Expression<Func<ClassDest, bool>> expected = x => x.PropString2 == "test" && x.SubClass.PropString2 == "test";
            ConverterExpressionVisitor visitor = new ConverterExpressionVisitor(GetParametersDictionnary(expected, typeof(ClassSource)), typeof(ClassSource));

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
        public void VisitMember_Expression_ExtentionMethod_Success()
        {
            Clean();
            Mapper.CreateMap<ClassDest, ClassSource>()
                .ForMember(s => s.PropString2, d => d.PropString1)
                .ForMember(s => s.SubClass, d => d.SubClass)
                .ForMember(s => s.CountListProp, d => d.ListProp.Count());
            Expression<Func<ClassDest, bool>> expected = x => x.CountListProp > 0;
            ConverterExpressionVisitor visitor = new ConverterExpressionVisitor(GetParametersDictionnary(expected, typeof(ClassSource)), typeof(ClassSource));

          
            Expression actual = null;

            actual = visitor.Visit(expected);


            var test = actual as BinaryExpression;
            Assert.IsNotNull(test);

            Clean();
        }
        [TestMethod]
        public void Visit_Null_Expression_Success()
        {
            Init(null);
            Expression<Func<ClassDest, bool>> expected = null;
            ConverterExpressionVisitor visitor = new ConverterExpressionVisitor(GetParametersDictionnary(expected, typeof(ClassSource)), typeof(ClassSource));



            Expression actual = null;

            actual = visitor.Visit(expected);
    
            Assert.IsNull(actual);
        }

        private Dictionary<Expression, Expression> GetParametersDictionnary<TFrom>(Expression<TFrom> from, Type toType)
        {
            Dictionary<Expression, Expression> parameterMap
             = new Dictionary<Expression, Expression>();
            if (from != null)
            {
                ParameterExpression[] newParams =
                    new ParameterExpression[from.Parameters.Count];
                for (int i = 0; i < newParams.Length; i++)
                {
                    newParams[i] = Expression.Parameter(toType, from.Parameters[i].Name);
                    parameterMap[from.Parameters[i]] = newParams[i];
                }
            }
            return parameterMap;
        }
    }
}
