using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapperExpression.Extensions;
using MapperExpression.Tests.Units.ClassTests;
using MapperExpression;
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
            Clean();
            Mapper.CreateMap<ClassSource, ClassDest>()
                .ForMember(s => s.PropString1, d => d.PropString2)
                .ReverseMap();

            Mapper.Initialize();
        }
        [ClassCleanup]
        public static void Clean()
        {
            //Remove all map after test
            Mapper.Reset();
        }
        [TestMethod, TestCategory("Extentions")]
        public void OrderBy_Success()
        {
            Init(null);
            IQueryable<ClassSource> actual = null;

            QueryableImplTest<ClassSource> expected = new QueryableImplTest<ClassSource>();

            actual = expected.OrderBy<ClassSource, ClassDest>("PropInt1");
            Assert.IsTrue(CheckExpressionMethod(actual.Expression, nameof(QueryableExtentions.OrderBy)));

        }
        [TestMethod, TestCategory("Extentions")]
        public void OrderByDescending_Success()
        {

            IQueryable<ClassSource> actual = null;

            QueryableImplTest<ClassSource> expected = new QueryableImplTest<ClassSource>();

            actual = expected.OrderByDescending<ClassSource, ClassDest>("PropInt1");
            Assert.IsTrue(CheckExpressionMethod(actual.Expression, nameof(QueryableExtentions.OrderByDescending)));

        }
        [TestMethod, TestCategory("Extentions")]
        public void ThenBy_Success()
        {
            Mapper.CreateMap<ClassSource, ClassDest>()
                .ForMember(s => s.PropString1, d => d.PropString2);
            Mapper.Initialize();
            IQueryable<ClassSource> actual = null;

            QueryableImplTest<ClassSource> expected = new QueryableImplTest<ClassSource>();

            actual = expected.OrderByDescending<ClassSource, ClassDest>("PropInt1")
                             .ThenBy<ClassSource, ClassDest>("PropInt1");
            Assert.IsTrue(CheckExpressionMethod(actual.Expression, nameof(QueryableExtentions.ThenBy)));

        }
        [TestMethod, TestCategory("Extentions")]
        public void ThenByDescending_Success()
        {
            Init(null);
            IQueryable<ClassSource> actual = null;

            QueryableImplTest<ClassSource> expected = new QueryableImplTest<ClassSource>();

            actual = expected.OrderByDescending<ClassSource, ClassDest>("PropInt1")
                             .ThenByDescending<ClassSource, ClassDest>("PropInt1");
            Assert.IsTrue(CheckExpressionMethod(actual.Expression, nameof(QueryableExtentions.ThenByDescending)));

        }

        [TestMethod, TestCategory("Extentions")]
        public void Select_Success()
        {
            Init(null);
            IQueryable<ClassDest> actual = null;

            QueryableImplTest<ClassSource> expected = new QueryableImplTest<ClassSource>();

            actual = expected.Select<ClassSource, ClassDest>();
            Assert.IsTrue(CheckExpressionMethod(actual.Expression, nameof(QueryableExtentions.Select)));

        }

        [TestMethod, TestCategory("Extentions")]
        public void Select_SameType_Success()
        {
            Init(null);
            IQueryable<ClassSource> actual = null;

            QueryableImplTest<ClassSource> expected = new QueryableImplTest<ClassSource>();

            actual = expected.Select<ClassSource, ClassSource>();
            Assert.AreEqual(actual.Expression.NodeType, ExpressionType.Constant);

        }
        [TestMethod, TestCategory("Extentions")]
        public void Select_NameOfMapper_Success()
        {
            Init(null);
            IQueryable<ClassDest> actual = null;

            QueryableImplTest<ClassSource> expected = new QueryableImplTest<ClassSource>();
            Mapper.CreateMap<ClassSource, ClassDest>("test");
            actual = expected.Select<ClassSource, ClassDest>("test");
            Assert.IsTrue(CheckExpressionMethod(actual.Expression, nameof(QueryableExtentions.Select)));

        }
        [TestMethod, TestCategory("Extentions")]
        public void Where_Success()
        {
            IQueryable<ClassDest> actual = null;
            Mapper.CreateMap<ClassSource, ClassDest>()
                 .ForMember(s => s.PropInt1, d => d.PropInt2, true); 
            QueryableImplTest<ClassDest> expected = new QueryableImplTest<ClassDest>();
            Expression<Func<ClassSource, bool>> criterias = x => x.PropInt1 == 1;
            actual = expected.Where(criterias);
            Assert.IsTrue(CheckExpressionMethod(actual.Expression, nameof(QueryableExtentions.Where)));


        }
        private bool CheckExpressionMethod(Expression expression, string methodeName)
        {
            if (expression.NodeType == ExpressionType.Call)
            {
                return (expression as MethodCallExpression).Method.Name == methodeName;
            }
            return false;
        }
    }
}
