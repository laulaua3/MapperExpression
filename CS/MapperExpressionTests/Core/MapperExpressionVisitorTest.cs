using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapperExpression.Core;
using System.Linq.Expressions;
using MapperExpression.Tests.Units.ClassTests;

namespace MapperExpression.Tests.Units.Core
{
    [TestClass]
    public class MapperExpressionVisitorTest
    {
        [TestMethod, TestCategory("Visit")]
        public void Visit_ExpressionIsNull_ReturnNull()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(Expression.Parameter(typeof(ClassSource), "s"));
            Expression exp = null;
            Expression actual = null;

            actual = expected.Visit(exp);

            Assert.IsNull(actual);
        }
        [TestMethod, TestCategory("Visit")]
        public void Visit_ExpressionLambdaConstantSimple_ReturnExpression()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(Expression.Parameter(typeof(ClassSource), "s"));
            Expression<Func<ClassSource, object>> exp = x => x.PropInt1;
            Expression actual = null;

            actual = expected.Visit(exp, true);

            Assert.IsNotNull(actual);
        }
        [TestMethod, TestCategory("Visit")]
        public void Visit_ExpressionConstantSimple_ReturnExpression()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(Expression.Parameter(typeof(ClassSource), "s"));
            Expression<Func<ClassSource, object>> exp = x => x.PropInt1;
            Expression actual = null;

            actual = expected.Visit(exp.Body, true);

            Assert.IsNotNull(actual);
        }
        [TestMethod, TestCategory("Visit")]
        public void Visit_ExpressionMemberSimple_ReturnExpression()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(Expression.Parameter(typeof(ClassSource), "s"));
            Expression<Func<ClassSource, object>> exp = x => x.PropString1;
            Expression actual = null;

            actual = expected.Visit(exp.Body, true);

            Assert.IsNotNull(actual);
        }
        
        [TestMethod, TestCategory("Visit")]
        public void Visit_ExpressionSubClassCheckIfNull_ReturnExpression()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(Expression.Parameter(typeof(ClassSource), "s"));
            Expression<Func<ClassSource, object>> exp = x => x.SubClass.PropString1;
            Expression actual = null;

            actual = expected.Visit(exp, true);

            Assert.IsTrue(actual.NodeType == ExpressionType.Conditional);
        }
        [TestMethod, TestCategory("Visit")]
        public void Visit_ExpressionSubClassCheckIfNull_DefaultValueConstant_ReturnExpression()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(Expression.Parameter(typeof(ClassSource), "s"));
            Expression<Func<ClassSource, object>> exp = x => x.SubClass.PropInt1;
            Expression actual = null;

            actual = expected.Visit(exp, true);

            Assert.IsTrue(actual.NodeType == ExpressionType.Conditional);
        }
        [TestMethod, TestCategory("Visit")]
        public void Visit_ParameterExpression_CheckIfNull_IsTrue_Expression()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(Expression.Parameter(typeof(ClassSource), "s"));
            ParameterExpression exp = Expression.Parameter(typeof(ClassSource2), "x");
            Expression actual = null;

            actual = expected.Visit(exp,true) as ParameterExpression;

            Assert.IsTrue(actual.NodeType == ExpressionType.Parameter);
            Assert.AreNotEqual(actual.Type, exp.Type);
        }

    }
}
