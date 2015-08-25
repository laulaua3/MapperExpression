Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Collections
Imports System.Data.Linq

Namespace Core


    ''' <summary>
    ''' Mappeur générique
    ''' </summary>
    ''' <typeparam name="TSource">The type of the source.</typeparam>
    ''' <typeparam name="TDest">The type of the dest.</typeparam>
    Public Class MapperConfiguration(Of TSource, TDest)
        Inherits MapperConfigurationBase

        Private delegateExpression As Func(Of TSource, TDest)

        Private customPropertiesMapping As List(Of Tuple(Of Expression(Of Func(Of TSource, Object)), Expression(Of Func(Of TDest, Object))))


        Public Sub New()
            MyBase.New(GetType(TSource), GetType(TDest))

            Me.customPropertiesMapping = New List(Of Tuple(Of Expression(Of Func(Of TSource, Object)), Expression(Of Func(Of TDest, Object))))()
        End Sub

#Region "Méthodes public"

        ''' <summary>
        ''' Gets the lambda expression.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetLambdaExpression() As Expression(Of Func(Of TSource, TDest))
            'Class dest
            Dim newClassDest = Expression.[New](GetType(TDest))
            'new ClassDestination() { Test1 = c1.Test1, Test2 = c1.Test2 };
            Dim exp = Expression.MemberInit(newClassDest, Me.MemberToMap)
            ' Expression<Func<ClassSource, ClassDestination>> lambdaExecute = (c1) => new ClassDestination() { Test1 = c1.Test1, Test2 = c1.Test2 };
            Return Expression.Lambda(Of Func(Of TSource, TDest))(exp, paramClassSource)
        End Function
        ''' <summary>
        ''' Gets the delegate of the expression.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetDelegate() As Func(Of TSource, TDest)
            If delegateExpression = Nothing Then
                delegateExpression = GetLambdaExpression().Compile()
            End If
            Return delegateExpression
        End Function
        ''' <summary>
        ''' Fors the member.
        ''' </summary>
        ''' <param name="getPropertySource">The get property source.</param>
        ''' <param name="getPropertyDest">The get property dest.</param>
        ''' <returns></returns>
        Public Function ForMember(getPropertySource As Expression(Of Func(Of TSource, Object)), getPropertyDest As Expression(Of Func(Of TDest, Object))) As MapperConfiguration(Of TSource, TDest)
            'Ajout dans la liste pour le traitement ultérieur
            Me.customPropertiesMapping.Add(Tuple.Create(Of Expression(Of Func(Of TSource, Object)), Expression(Of Func(Of TDest, Object)))(getPropertySource, getPropertyDest))
            Return Me
        End Function

        ''' <summary>
        ''' Ignores the specified property dest.
        ''' </summary>
        ''' <param name="propertyDest">The property dest.</param>
        ''' <returns></returns>
        Public Function Ignore(propertyDest As Expression(Of Func(Of TDest, Object))) As MapperConfiguration(Of TSource, TDest)

            Dim [property] = Me.GetPropertyInfo(propertyDest)
            If [property] IsNot Nothing Then

                CheckAndRemoveMemberDest([property].Name)
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Gets the data load options for LinqToSql.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetDataLoadOptionsLinq() As DataLoadOptions
            Dim options As New DataLoadOptions()
            Dim propertiesLinq = Me.customPropertiesMapping.Where(Function(p) Me.GetPropertyInfo(p.Item1).PropertyType.BaseType.Name = "EntityBase").Select(Function(x) x.Item1)
            For Each prop As Expression(Of Func(Of TSource, Object)) In propertiesLinq
                options.LoadWith(prop)
            Next
            Return options
        End Function
        ''' <summary>
        ''' Creates the member assignement.
        ''' </summary>
        ''' <exception cref="Exception">
        ''' La propriété ' + memberDest.Name + ' de destination est en lecture seule
        ''' or
        ''' La propriété ' + memberSource.Name + 'n'existe pas pour le type ' + memberSource.DeclaringType.ToString() + '
        ''' </exception>
        Public Overrides Sub CreateMappingExpression()
            If Not _isInitialized Then
                'on le met avant le traitement pour éviter les boucles récursives
                Me._isInitialized = True
                Me.CreateCommonMember()

                Dim i As Integer = 0
                For index = 1 To Me.customPropertiesMapping.Count
                    Dim getPropertySource As Expression(Of Func(Of TSource, Object)) = Me.customPropertiesMapping(i).Item1
                    Dim getPropertyDest As Expression(Of Func(Of TDest, Object)) = Me.customPropertiesMapping(i).Item2
                    Dim memberSource As PropertyInfo = Me.GetPropertyInfo(getPropertySource)
                    Dim memberDest As PropertyInfo = Me.GetPropertyInfo(getPropertyDest)
                    CreateMemberAssignement(memberSource, memberDest, True)
                Next
                'création du délégate
                Me.GetDelegate()
            End If

        End Sub




#End Region

#Region " Méthodes privées"

        Private Function GetPropertyInfo(Of T)(propertyExpression As Expression(Of Func(Of T, Object))) As PropertyInfo
            Select Case propertyExpression.Body.NodeType
                Case ExpressionType.Convert
                    Return TryCast(TryCast(TryCast(propertyExpression.Body, UnaryExpression).Operand, MemberExpression).Member, PropertyInfo)

                Case ExpressionType.MemberAccess
                    Return TryCast(TryCast(propertyExpression.Body, MemberExpression).Member, PropertyInfo)
                Case Else
                    Throw New NotImplementedException("Ce type d'expression n'est pas valide")
            End Select
        End Function
#End Region
    End Class
End Namespace
