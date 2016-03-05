using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapperExpression.Core;
using MapperExpression.Tests.Units.ClassTests;
using System.Collections;

namespace MapperExpression.Tests.Units.Core
{
    [TestClass]
    public class MapperContainerTest
    {
        [TestMethod, TestCategory("MapperContainer")]
        public void RemotAt_Success()
        {
            MapperConfigurationContainer.Instance.Clear();
            var countMapper = 0;
            var mapperToInsert = new MapperConfiguration<ClassSource, ClassDest>("sourceTest");
            MapperConfigurationContainer.Instance.Add(mapperToInsert);
            MapperConfigurationContainer.Instance.RemoveAt(0);
            Assert.AreEqual(countMapper, MapperConfigurationContainer.Instance.Count);
            MapperConfigurationContainer.Instance.Clear();

        }
        [TestMethod, TestCategory("MapperContainer")]
        public void GetEnumerator_Success()
        {
            MapperConfigurationContainer.Instance.Clear();

            var mapperToInsert = new MapperConfiguration<ClassSource, ClassDest>("sourceTest");
            MapperConfigurationContainer.Instance.Add(mapperToInsert);
            IEnumerator actual = (MapperConfigurationContainer.Instance as IEnumerable).GetEnumerator();
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.MoveNext());
            MapperConfigurationContainer.Instance.Clear();

        }
    }
}
