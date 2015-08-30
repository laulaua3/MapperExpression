using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapperCore.Extensions;
using MapperExpression.Tests.Units.ClassTests;
using MapperCore;
using System.Linq;
using System.Linq.Expressions;

namespace MapperExpression.Tests.Units.Extentions
{
    [TestClass]
    public class QueryableExtentionsTest
   
    {
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            Mapper.CreateMap<ClassSource, ClassDest>()
                .ForMember(s => s.PropString1, d => d.PropString2);
                
            Mapper.Initialize();
        }
        [TestMethod,TestCategory("Extentions")]
        public void OrderBy_Success()
        {
          
            IQueryable<ClassSource> actual = null;
      
            QueryableImplTest<ClassSource> expected = new QueryableImplTest<ClassSource>();

            actual = expected.OrderBy<ClassSource,ClassDest>("PropInt1");
            Assert.IsTrue(CheckExpressionMethod(actual.Expression, "OrderBy"));
            
        }
        [TestMethod, TestCategory("Extentions")]
        public void OrderByDescending_Success()
        {
          
            IQueryable<ClassSource> actual = null;

            QueryableImplTest<ClassSource> expected = new QueryableImplTest<ClassSource>();

            actual = expected.OrderByDescending<ClassSource, ClassDest>("PropInt1");
            Assert.IsTrue(CheckExpressionMethod(actual.Expression, "OrderByDescending"));

        }
        [TestMethod, TestCategory("Extentions")]
        public void ThenBy_Success()
        {
          
            IQueryable<ClassSource> actual = null;

            QueryableImplTest<ClassSource> expected = new QueryableImplTest<ClassSource>();

            actual = expected.OrderByDescending<ClassSource, ClassDest>("PropInt1").ThenBy<ClassSource, ClassDest>("PropInt1");
            Assert.IsTrue(CheckExpressionMethod(actual.Expression, "ThenBy"));

        }
        [TestMethod, TestCategory("Extentions")]
        public void ThenByDescending_Success()
        {
           
            IQueryable<ClassSource> actual = null;

            QueryableImplTest<ClassSource> expected = new QueryableImplTest<ClassSource>();

            actual = expected.OrderByDescending<ClassSource, ClassDest>("PropInt1").ThenByDescending<ClassSource, ClassDest>("PropInt1");
            Assert.IsTrue(CheckExpressionMethod(actual.Expression, "ThenByDescending"));

        }

        [TestMethod, TestCategory("Extentions")]
        public void Select_Success()
        {

            IQueryable<ClassDest> actual = null;

            QueryableImplTest<ClassSource> expected = new QueryableImplTest<ClassSource>();

            actual = expected.Select<ClassSource, ClassDest>();
            Assert.IsTrue(CheckExpressionMethod(actual.Expression, "Select"));

        }
        private bool CheckExpressionMethod(Expression expression,string methodeName)
        {
            if (expression.NodeType == ExpressionType.Call)
            {
                return (expression as MethodCallExpression).Method.Name == methodeName;
            }
            return false;
        }
    }
}
