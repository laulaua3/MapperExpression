using System.Collections.Generic;
using System.Linq.Expressions;
using System;

namespace MapperExpression.Core
{
    internal class MapperExpressionVisitor : ExpressionVisitor
    {
        #region Variables

        private Expression previousMember;

        private readonly bool checkNull;

        private readonly ParameterExpression paramSource;

        private readonly List<MemberExpression> membersTocheck;

        #endregion

        #region Constructeur

        // <summary>
        // Initialise une nouvelle instance de  <see cref="MapperExpressionVisitor"/> classe.
        // </summary>
        // <param name="checkIfNull">Indique si l'on tester la nullité des objets (récursive)</param>
        // <param name="paramClassSource">paramètre de la source</param>
        internal MapperExpressionVisitor(bool checkIfNull, ParameterExpression paramClassSource)
        {
            checkNull = checkIfNull;
            paramSource = paramClassSource;
            membersTocheck = new List<MemberExpression>();
        }

        #endregion

        #region Méthodes surchargées

        /// <summary>
        /// Distribue l'expression à l'une des méthodes de visite les plus spécialisées dans cette classe.
        /// </summary>
        /// <param name="node">Expression à visiter.</param>
        /// <returns>
        /// Expression modifiée, si celle-ci ou toute sous-expression a été modifiée ; sinon, retourne l'expression d'origine.
        /// </returns>
        public override Expression Visit(Expression node)
        {
            if (node == null)
                return node;
            if (checkNull)
            {
                //On traite que nos cas
                switch (node.NodeType)
                {
                    case ExpressionType.MemberAccess:
                        return VisitMember(node as MemberExpression);
                    case ExpressionType.Parameter:
                        return VisitParameter(node as ParameterExpression);
                    case ExpressionType.Convert:
                        return VisitMember((node as UnaryExpression).Operand as MemberExpression);
                    case ExpressionType.Lambda:
                        base.Visit((node as LambdaExpression).Body);
                        break;

                    default:
                        base.Visit(node);
                        break;

                }
                bool isFirst = true;
                //Nous voulons tester tout les sous objets avant d'affecter la valeur
                //Ex: source.SubClass.SubClass2.MaPropriete
                //Ce qui donnera
                //source.SubClass != null ? source.SubClass.SubClass2 != null ? source.SubClass.SubClass2.MaPropriete :null :null
                foreach (MemberExpression item in membersTocheck)
                {

                    if (!isFirst) //Pour ne pas tester la valeur de la propriété de retour
                    {
                        object defaultValue = GetDefaultValue(item.Type);

                        //Création de la vérification de la nullité
                        Expression notNull = Expression.NotEqual(item, Expression.Constant(defaultValue, item.Type));
                        Expression conditional = null;
                        //On crée une condition qui inclue la condition précédente
                        if (previousMember != null)
                        {
                            object defaultPreviousValue = GetDefaultValue(previousMember.Type);
                            conditional = Expression.Condition(notNull, previousMember, Expression.Constant(defaultPreviousValue, previousMember.Type));
                        }
                        //On affecte la condition nouvellement crée qui deviendra la précédente
                        previousMember = conditional;
                    }
                    else //là la propriété demandée
                    {
                        previousMember = item;
                        isFirst = false;
                    }
                }
                return previousMember;
            }
            else
            {
                //retour par défaut (avec changement du paramètre)
                //pour supprimer la validation de l'expression lambda
                if ((node.NodeType == ExpressionType.Lambda))
                {
                    return base.Visit((node as LambdaExpression).Body);
                }
                else
                {
                    return base.Visit(node);
                }
            }
        }

        /// <summary>
        /// Visite <see cref="T:System.Linq.Expressions.ParameterExpression" />.
        /// </summary>
        /// <param name="node">Expression à visiter.</param>
        /// <returns>
        /// Expression modifiée, si celle-ci ou toute sous-expression a été modifiée ; sinon, retourne l'expression d'origine.
        /// </returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            //Pour changer de paramètre
            return paramSource;
        }

        /// <summary>
        /// Visite les enfants de <see cref="T:System.Linq.Expressions.MemberExpression" />.
        /// </summary>
        /// <param name="node">Expression à visiter.</param>
        /// <returns>
        /// Expression modifiée, si celle-ci ou toute sous-expression a été modifiée ; sinon, retourne l'expression d'origine.
        /// </returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            MemberExpression memberAccessExpression = (MemberExpression)base.VisitMember(node);

            //Pour traiter plus tard
            if (memberAccessExpression != null && checkNull)
            {
                //Sachant que l'on visite le premier membre en premier et que l'on descend à chaque fois
                //on insert notre membre courant à la première ligne de la liste pour inverser l'ordre
                //exemple :
                //source.SubClass.SubClass2.MaPropriete
                //la liste serait:
                //MaList(0) = SubClass
                //MaList(1) = SubClass2
                //MaList(2) = MaPropriete
                //
                //mais nous voulons
                //MaList(0) = MaPropriete
                //MaList(1) = SubClass2
                //MaList(2) = SubClass
                membersTocheck.Insert(0, memberAccessExpression);
            }
            return memberAccessExpression;
        }

        /// <summary>
        /// Visite les enfants de <see cref="T:System.Linq.Expressions.UnaryExpression" />.
        /// </summary>
        /// <param name="node">Expression à visiter.</param>
        /// <returns>
        /// Expression modifiée, si celle-ci ou toute sous-expression a été modifiée ; sinon, retourne l'expression d'origine.
        /// </returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            return VisitMember(node.Operand as MemberExpression);
        }

        private object GetDefaultValue(Type typeObject)
        {
            object defaultValue = null;
            //Dans le cas de type valeur (ex :Integer), il faut instancier l'objet pour avoir sa valeur par défaut
            if (typeObject.IsValueType)
            {
                NewExpression exp = Expression.New(typeObject);
                LambdaExpression lambda = Expression.Lambda(exp);
                Delegate constructor = lambda.Compile();
                defaultValue = constructor.DynamicInvoke();
            }
            return defaultValue;
        }

        #endregion
    }
}