using MapperExpression.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace MapperExpression.Tests.Units.ClassTests
{
    /// <summary>
    ///Need to create a derived class for test the protected properties/Methods
    /// </summary>
    public class MapperConfigurationTestContainer : MapperConfiguration<ClassSource, ClassDest>
    {

       
        public int GetIgnoreCount()
        {
            return propertiesToIgnore.Count;
        }

        public int GetAfterMapActionCount()
        {
            return actionsAfterMap.Count;
        }

        public MapperConfigurationBase GetMapperTest(Type tSource, Type tDest, bool throwExceptionOnNoFound)
        {
            return GetMapper(tSource, tDest, throwExceptionOnNoFound);
        }

        public  bool CheckAndConfigureTypeOfListTest(PropertyInfo memberSource, PropertyInfo memberDest)
        {
            return CheckAndConfigureTypeOfList(memberSource, memberDest);
        }

        public void CheckAndConfigureMembersMappingTest(PropertyInfo memberSource, PropertyInfo memberDest)
        {
            CheckAndConfigureMembersMapping(memberSource, memberDest);
        }

        public PropertyInfo GetPropertyInfoTest(LambdaExpression expression)
        {
            return GetPropertyInfo(expression);
        }

        public List<MemberAssignment> ChangeSourceTest(PropertyInfo property, ParameterExpression paramSource)
        {
            return ChangeSource(property, paramSource);
        }

        public void  CreateCheckIfNullTest(PropertyInfo memberSource, PropertyInfo memberDest, MapperConfigurationBase mapperExterne)
        {
            CreateCheckIfNull(memberSource, memberDest, mapperExterne);
        }

        public void CreateCommonMemberTest()
        {
            CreateCommonMember();
        }

        public void CheckAndRemoveMemberSourceTest(string propertyName)
        {
            CheckAndRemoveMemberSource(propertyName);
        }

        public void CheckAndRemoveMemberDestTest(string propertyName)
        {
            CheckAndRemoveMemberDest(propertyName);
        }
    }
}
