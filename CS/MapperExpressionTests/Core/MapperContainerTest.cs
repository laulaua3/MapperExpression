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
            MapperConfigurationCollectionContainer.Instance.Clear();
            var countMapper = 0;
            var mapperToInsert = new MapperConfiguration<ClassSource, ClassDest>("sourceTest");
            MapperConfigurationCollectionContainer.Instance.Add(mapperToInsert);
            MapperConfigurationCollectionContainer.Instance.RemoveAt(0);
            Assert.AreEqual(countMapper, MapperConfigurationCollectionContainer.Instance.Count);
            MapperConfigurationCollectionContainer.Instance.Clear();

        }
        [TestMethod, TestCategory("MapperContainer")]
        public void GetEnumerator_Success()
        {
            MapperConfigurationCollectionContainer.Instance.Clear();

            var mapperToInsert = new MapperConfiguration<ClassSource, ClassDest>("sourceTest");
            MapperConfigurationCollectionContainer.Instance.Add(mapperToInsert);
            IEnumerator actual = (MapperConfigurationCollectionContainer.Instance as IEnumerable).GetEnumerator();
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.MoveNext());
            MapperConfigurationCollectionContainer.Instance.Clear();

        }
    }
}
