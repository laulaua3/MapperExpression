using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapperCore.Core;
using System.Linq.Expressions;
using MapperExpression.Tests.Units.ClassTests;

namespace MapperExpression.Tests.Units.Core
{
    [TestClass]
    public class MapperExpressionVisitorTest
    {
        [TestMethod,TestCategory("Visit")]
        public void Visit_ExpressionIsNull_ReturnNull()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(false,Expression.Parameter(typeof(ClassSource),"s"));
            Expression exp = null;
            Expression actual = null;

            actual = expected.Visit(exp);

            Assert.IsNull(actual);
        }
        [TestMethod, TestCategory("Visit")]
        public void Visit_ExpressionLambdaConstantSimple_ReturnExpression()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(true, Expression.Parameter(typeof(ClassSource), "s"));
            Expression<Func<ClassSource,object >> exp = x=>x.PropInt1;
            Expression actual = null;

            actual = expected.Visit(exp);

            Assert.IsNotNull(actual);
        }
        [TestMethod, TestCategory("Visit")]
        public void Visit_ExpressionConstantSimple_ReturnExpression()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(true, Expression.Parameter(typeof(ClassSource), "s"));
            Expression<Func<ClassSource, object>> exp = x => x.PropInt1;
            Expression actual = null;

            actual = expected.Visit(exp.Body);

            Assert.IsNotNull(actual);
        }
        [TestMethod, TestCategory("Visit")]
        public void Visit_ExpressionMemberSimple_ReturnExpression()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(true, Expression.Parameter(typeof(ClassSource), "s"));
            Expression<Func<ClassSource, object>> exp = x => x.PropString1;
            Expression actual = null;

            actual = expected.Visit(exp.Body);

            Assert.IsNotNull(actual);
        }
        [TestMethod, TestCategory("Visit")]
        public void Visit_ExpressionDefault_ReturnExpressionMember()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(true, Expression.Parameter(typeof(ClassSource), "s"));
            Expression<Func<ClassSource, bool>> exp = x => x.PropString1 !="";
            Expression actual = null;

            actual = expected.Visit(exp.Body);

            Assert.IsTrue(actual.NodeType == ExpressionType.MemberAccess);
        }
        [TestMethod, TestCategory("Visit")]
        public void Visit_ExpressionSubClassCheckIfNull_ReturnExpression()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(true, Expression.Parameter(typeof(ClassSource), "s"));
            Expression<Func<ClassSource, object>> exp = x => x.SubClass.PropString1;
            Expression actual = null;

            actual = expected.Visit(exp);

            Assert.IsTrue(actual.NodeType == ExpressionType.Conditional);
        }
        [TestMethod, TestCategory("Visit")]
        public void Visit_ExpressionSubClassCheckIfNull_DefaultValueConstant_ReturnExpression()
        {
            MapperExpressionVisitor expected = new MapperExpressionVisitor(true, Expression.Parameter(typeof(ClassSource), "s"));
            Expression<Func<ClassSource, object>> exp = x => x.SubClass.PropInt1;
            Expression actual = null;

            actual = expected.Visit(exp);

            Assert.IsTrue(actual.NodeType == ExpressionType.Conditional);
        }

        
    }
}
