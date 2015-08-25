Imports System.Linq.Expressions
Imports System.Collections.Generic

Namespace Core
    Friend Class MapperExpressionVisitor
        Inherits ExpressionVisitor

#Region "Variables"

        Private previousMember As Expression

        Private ReadOnly checkNull As Boolean

        Private ReadOnly paramSource As ParameterExpression

        Private ReadOnly membersTocheck As List(Of MemberExpression)

#End Region

#Region "Constructeur"

        ''' <summary>
        ''' Initialise une nouvelle instance de  <see cref="MapperExpressionVisitor"/> classe.
        ''' </summary>
        ''' <param name="checkIfNull">Indique si l'on tester la nullité des objets (récursive)</param>
        ''' <param name="paramClassSource">paramètre de la source</param>
        Friend Sub New(checkIfNull As Boolean, paramClassSource As ParameterExpression)
            checkNull = checkIfNull
            paramSource = paramClassSource
            membersTocheck = New List(Of MemberExpression)()
        End Sub

#End Region

#Region "Méthodes surchargées"

        ''' <summary>
        ''' Distribue l'expression à l'une des méthodes de visite les plus spécialisées dans cette classe.
        ''' </summary>
        ''' <param name="node">Expression à visiter.</param>
        ''' <returns>
        ''' Expression modifiée, si celle-ci ou toute sous-expression a été modifiée ; sinon, retourne l'expression d'origine.
        ''' </returns>
        Public Overrides Function Visit(node As Expression) As Expression

            If checkNull Then
                'On traite que nos cas
                Select Case node.NodeType
                    Case ExpressionType.MemberAccess
                        Return Me.VisitMember(TryCast(node, MemberExpression))
                    Case ExpressionType.Parameter
                        Return Me.VisitParameter(TryCast(node, ParameterExpression))
                    Case ExpressionType.Convert
                        Return Me.VisitMember(TryCast(TryCast(node, UnaryExpression).Operand, MemberExpression))
                    Case ExpressionType.Lambda
                        MyBase.Visit((TryCast(node, LambdaExpression)).Body)
                        Exit Select
                    Case Else
                        MyBase.Visit(node)
                        Exit Select
                End Select
                Dim isFirst As Boolean = True
                'Nous voulons tester tout les sous objets avant d'affecter la valeur
                'Ex:
                'Expression d'origine : 
                'source.SubClass.SubClass2.MaPropriete
                'Expression transformée :
                'If(source.SubClass <> Nothing, If(source.SubClass.SubClass2 <> Nothing, source.SubClass.SubClass2.MaPropriete, Nothing), Nothing)
                For Each item As MemberExpression In membersTocheck

                    If Not isFirst Then 'pour ne pas tester la valeur à affecter

                        Dim defaultValue As Object = GetDefaultValue(item.Type)

                        'Création de la vérification de la nullité
                        Dim notNull As Expression = Expression.NotEqual(item, Expression.Constant(defaultValue, item.Type))
                        Dim conditional As Expression = Nothing
                        'On crée une condition qui inclue la condition précédente
                        If previousMember IsNot Nothing Then
                            Dim defaultPreviousValue As Object = GetDefaultValue(previousMember.Type)
                            conditional = Expression.Condition(notNull, previousMember, Expression.Constant(defaultPreviousValue, previousMember.Type))
                        End If
                        'On affecte la condition nouvellement crée qui deviendra la précédente
                        previousMember = conditional
                    Else 'là, la propriété demandée
                        previousMember = item
                        isFirst = False
                    End If
                Next
                Return previousMember
            Else
                'retour par défaut (avec changement du paramètre)
                'pour supprimer la validation de l'expression lambda
                If (node.NodeType = ExpressionType.Lambda) Then
                    Return MyBase.Visit((TryCast(node, LambdaExpression)).Body)
                Else
                    Return MyBase.Visit(node)
                End If
            End If
        End Function

        ''' <summary>
        ''' Visite <see cref="T:System.Linq.Expressions.ParameterExpression" />.
        ''' </summary>
        ''' <param name="node">Expression à visiter.</param>
        ''' <returns>
        ''' Expression modifiée, si celle-ci ou toute sous-expression a été modifiée ; sinon, retourne l'expression d'origine.
        ''' </returns>
        Protected Overrides Function VisitParameter(node As ParameterExpression) As Expression
            'Pour changer de paramètre
            Return paramSource
        End Function

        ''' <summary>
        ''' Visite les enfants de <see cref="T:System.Linq.Expressions.MemberExpression" />.
        ''' </summary>
        ''' <param name="node">Expression à visiter.</param>
        ''' <returns>
        ''' Expression modifiée, si celle-ci ou toute sous-expression a été modifiée ; sinon, retourne l'expression d'origine.
        ''' </returns>
        Protected Overrides Function VisitMember(node As MemberExpression) As Expression
            Dim memberAccessExpression As MemberExpression = DirectCast(MyBase.VisitMember(node), MemberExpression)
            'Pour traiter plus tard
            If memberAccessExpression IsNot Nothing AndAlso checkNull Then
                'Sachant que l'on visite le premier membre en premier et que l'on descend à chaque fois
                'on insert notre membre courant à la première ligne de la liste pour inverser l'ordre
                'exemple :
                'source.SubClass.SubClass2.MaPropriete
                'la liste serait avec Add:
                'MaList(0) = SubClass
                'MaList(1) = SubClass2
                'MaList(2) = MaPropriete

                'mais nous voulons
                'MaList(0) = MaPropriete
                'MaList(1) = SubClass2
                'MaList(2) = SubClass
                membersTocheck.Insert(0, memberAccessExpression)
            End If
            Return memberAccessExpression
        End Function

        ''' <summary>
        ''' Visite les enfants de <see cref="T:System.Linq.Expressions.UnaryExpression" />.
        ''' </summary>
        ''' <param name="node">Expression à visiter.</param>
        ''' <returns>
        ''' Expression modifiée, si celle-ci ou toute sous-expression a été modifiée ; sinon, retourne l'expression d'origine.
        ''' </returns>
        Protected Overrides Function VisitUnary(node As UnaryExpression) As Expression
            Return Me.VisitMember(TryCast(node.Operand, MemberExpression))
        End Function

        Private Function GetDefaultValue(typeObject As Type) As Object
            Dim defaultValue As Object = Nothing
            'Dans le cas de type valeur (ex :Integer), il faut instancier l'objet pour avoir sa valeur par défaut
            If typeObject.BaseType = GetType(ValueType) Then
                Dim exp = Expression.[New](typeObject)
                Dim lambda = Expression.Lambda(exp)
                Dim constructor = lambda.Compile()
                defaultValue = constructor.DynamicInvoke()
            End If
            Return defaultValue
        End Function


#End Region

    End Class
End Namespace