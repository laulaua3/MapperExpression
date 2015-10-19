using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MapperExpression.Core
{
    internal class ConverterExpressionVisitor<TSource, TDest> : ExpressionVisitor
    {
        private ParameterExpression paramClassSource;
        private MapperConfiguration<TSource, TDest> mapper;


        public ParameterExpression Parameter
        {
            get
            {
                return paramClassSource;
            }
        }
        public ConverterExpressionVisitor()
        {
            mapper = Mapper.GetMapper(typeof(TSource), typeof(TDest)) as MapperConfiguration<TSource, TDest>;
            paramClassSource = Expression.Parameter(typeof(TDest), "d");
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return paramClassSource;
        }

        public override Expression Visit(Expression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Lambda:

                    return base.Visit((node as LambdaExpression).Body);
                default:

                    return base.Visit(node);
            }

        }
        protected override Expression VisitMember(MemberExpression node)
        {
            Expression exp = base.Visit(node.Expression);
            //For children class
            if (exp.NodeType == ExpressionType.MemberAccess && (exp as MemberExpression).Member.DeclaringType.IsClass)
            {
                var subMapper = Mapper.GetMapper(node.Member.ReflectedType, exp.Type);
                //Need to call dynamicaly the methode because it inside the class MapperConfiguration<>
                MethodInfo methodGetPropertyInfoDest = subMapper.GetType().GetMethod("GetPropertyInfoDest", BindingFlags.NonPublic | BindingFlags.Instance);
                //For the performances
                Func<string, PropertyInfo> executeDelegate = (Func<string, PropertyInfo>)Delegate.CreateDelegate(typeof(Func<string, PropertyInfo>), subMapper, methodGetPropertyInfoDest);
                PropertyInfo expMappeur = executeDelegate(node.Member.Name);
                return Expression.MakeMemberAccess(exp, expMappeur);
            }
            else
            {
                var property = mapper.GetPropertyInfoDest(node.Member.Name);

                return Expression.MakeMemberAccess(paramClassSource, property);
            }

        }
    }
}
