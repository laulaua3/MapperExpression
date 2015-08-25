Imports System.Collections.Generic
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Reflection
Imports MapperExpression.Exception

Namespace Core

    ''' <summary>
    ''' Mappeur principal
    ''' </summary>
    ''' <typeparam name="TSource">The type of the source.</typeparam>
    ''' <typeparam name="TDest">The type of the dest.</typeparam>
    Public Class MapperConfiguration(Of TSource, TDest)
        Inherits MapperConfigurationBase

#Region "Variables"

        Private ReadOnly actionsAfterMap As List(Of Action(Of TSource, TDest))

#End Region

#Region "Constructeur"

        ''' <summary>
        ''' Instancie un nouvelle instance de MapperConfiguration
        ''' </summary>
        Friend Sub New()
            MyBase.New(GetType(TSource), GetType(TDest))
            Me.propertiesMapping = New List(Of Tuple(Of LambdaExpression, LambdaExpression, Boolean))()
            Me.propertiesToIgnore = New List(Of PropertyInfo)()
            Me.actionsAfterMap = New List(Of Action(Of TSource, TDest))()
        End Sub

#End Region

#Region "Méthodes publiques"

        ''' <summary>
        ''' Obtient l'expression lambda du mapping
        ''' </summary>
        ''' <returns></returns>
        Public Function GetLambdaExpression() As Expression(Of Func(Of TSource, TDest))
            Dim exp = Me.GetMemberInitExpression()
            '  (source) => new ClassDestination() { Test1 = source.Test1, Test2 = source.Test2 };
            Return Expression.Lambda(Of Func(Of TSource, TDest))(exp, paramClassSource)
        End Function

        ''' <summary>
        ''' Obtient un delegate du mapping
        ''' </summary>
        ''' <returns></returns>
        Public Function GetFuncDelegate() As Func(Of TSource, TDest)
            Return TryCast(MyBase.GetDelegate(), Func(Of TSource, TDest))
        End Function

        ''' <summary>
        ''' Configure le mapping pour une propriété source et une propriété de destination du même type
        ''' </summary>
        ''' <param name="getPropertySource">Expression représentant la propriété source</param>
        ''' <param name="getPropertyDest">Expression représentant la propriété de destination</param>
        ''' <param name="checkIfNull">Vérifie si la propriété source n'est pas nul avant d'affecté la valeur (récursif)</param>
        ''' <returns></returns>
        Public Overloads Function ForMember(getPropertySource As Expression(Of Func(Of TSource, Object)), getPropertyDest As Expression(Of Func(Of TDest, Object)), Optional checkIfNull As Boolean = False) As MapperConfiguration(Of TSource, TDest)
            'Ajout dans la liste pour le traitement ultérieur
            MyBase.ForMember(TryCast(getPropertySource, LambdaExpression), TryCast(getPropertyDest, LambdaExpression), checkIfNull)
            Return Me
        End Function

        ''' <summary>
        ''' Ignore la propriété de destination lors du mapping.
        ''' </summary>
        ''' <param name="propertyDest">Expression représentant la propriété à ignorer</param>
        ''' <returns></returns>
        Public Function Ignore(propertyDest As Expression(Of Func(Of TDest, Object))) As MapperConfiguration(Of TSource, TDest)
            propertiesToIgnore.Add(Me.GetPropertyInfo(propertyDest))
            Return Me
        End Function

        ''' <summary>
        ''' Action à exécuter après le mapping
        ''' </summary>
        ''' <param name="actionAfterMap">Action a réalisée</param>
        ''' <returns></returns>
        Public Function AfterMap(actionAfterMap As Action(Of TSource, TDest)) As MapperConfiguration(Of TSource, TDest)
            Me.actionsAfterMap.Add(actionAfterMap)
            Return Me
        End Function

        ''' <summary>
        ''' Exécute les actions après le mapping
        ''' </summary>
        ''' <param name="source">The source.</param>
        ''' <param name="dest">The dest.</param>
        Public Sub ExecuteAfterActions(source As TSource, dest As TDest)

            For i As Integer = 0 To Me.actionsAfterMap.Count - 1
                Me.actionsAfterMap(i)(source, dest)

            Next
        End Sub

        ''' <summary>
        ''' Crée l'expression lambda du mapping
        ''' </summary>
        ''' <param name="constructor"></param>
        Friend Overrides Sub CreateMappingExpression(constructor As Func(Of Type, Object))

            If Not isInitialized Then
                'on le met avant le traitement pour éviter les boucles récursives
                Me.isInitialized = True
                Me.constructorFunc = constructor
                Me.CreateCommonMember()

                For i As Integer = 0 To Me.propertiesMapping.Count - 1

                    Dim getPropertySource As LambdaExpression = Me.propertiesMapping(i).Item1
                    Dim getPropertyDest As LambdaExpression = Me.propertiesMapping(i).Item2
                    'On va chercher les propriétés choisies
                    Dim memberSource As PropertyInfo = Me.GetPropertyInfo(getPropertySource)
                    Dim memberDest As PropertyInfo = Me.GetPropertyInfo(getPropertyDest)
                    Me.CreateMemberAssignement(memberSource, memberDest)

                Next
                'création du delegate
                Me.GetFuncDelegate()

            End If
        End Sub

        ''' <summary>
        ''' Crée le mappeur inverse du mappeur courant
        ''' </summary>
        ''' <returns>le nouveau mappeur</returns>
        Public Function ReverseMap() As MapperConfiguration(Of TDest, TSource)
            'On recherche le mapper inverse
            Dim map As MapperConfiguration(Of TDest, TSource) = TryCast(GetMapper(GetType(TDest), GetType(TSource), False), MapperConfiguration(Of TDest, TSource))
            'on lève un exception si celui existe déjà
            If map IsNot Nothing Then
                Throw New MapperExistException(GetType(TDest), GetType(TSource))
            End If
            map = New MapperConfiguration(Of TDest, TSource)()
            Me.CreateCommonMember()
            'On parcours les propriétés de mapping de l'existant et on crée les relations inverses
            For i As Integer = 0 To Me.propertiesMapping.Count - 1

                Dim item As Tuple(Of LambdaExpression, LambdaExpression, Boolean) = Me.propertiesMapping(i)
                Dim propertyDest As PropertyInfo = Me.GetPropertyInfo(item.Item1)
                'Si la propriété de destination n'est pas en lecture seul
                If propertyDest.CanWrite Then
                    map.ForMember(item.Item2, item.Item1, item.Item3)
                End If

            Next
            MapperConfigurationContainer.Instance.Add(map)
            Return map
        End Function

        ''' <summary>
        ''' Indique si l'on utilise le service d'injection
        ''' </summary>
        Public Function ConstructUsingServiceLocator() As MapperConfiguration(Of TSource, TDest)
            Me.UseServiceLocator = True
            Return Me
        End Function

        ''' <summary>
        ''' Gets the sorted expression.
        ''' </summary>
        ''' <typeparam name="TResult">The type of the result.</typeparam>
        ''' <param name="propertySource">The property source.</param>
        ''' <returns></returns>
        ''' <exception cref="PropertyNoExistException"></exception>
        Friend Function GetSortedExpression(Of TResult)(propertySource As String) As Expression(Of Func(Of TSource, TResult))

            Dim exp = Me.propertiesMapping.Find(Function(x) Me.GetPropertyInfo(x.Item2).Name = propertySource)
            If exp Is Nothing Then
                Throw New PropertyNoExistException(propertySource, GetType(TSource))
            End If

            Return CType(exp.Item1, Expression(Of Func(Of TSource, TResult)))
        End Function

        Friend Function GetSortedExpression(propertySource As String) As LambdaExpression
            Dim result As Expression = Nothing
            Dim exp = Me.propertiesMapping.Find(Function(x) Me.GetPropertyInfo(x.Item2).Name = propertySource)
            If exp Is Nothing Then
                Throw New PropertyNoExistException(propertySource, GetType(TDest))
            End If
            Dim [property] = Me.GetPropertyInfo(exp.Item2)
            Dim visitor = New MapperExpressionVisitor(False, Me.paramClassSource)
            result = visitor.Visit(exp.Item1)
            Return Expression.Lambda(result, Me.paramClassSource)

        End Function

        Friend Function GetPropertyInfoSource(propertyName As String) As PropertyInfo
            Dim exp = Me.propertiesMapping.Find(Function(x) Me.GetPropertyInfo(x.Item2).Name = propertyName)
            If exp Is Nothing Then
                Throw New PropertyNoExistException(propertyName, GetType(TDest))
            End If
            Dim [property] = Me.GetPropertyInfo(exp.Item2)
            Return [property]
        End Function

#End Region

    End Class
End Namespace