Imports System.Data.Linq

Imports System.Reflection
Imports System.Linq.Expressions
Imports System.Linq
Imports System.Collections.Generic
Imports System.Collections
Imports System
Imports MapperExpression.Exception

Namespace Core

    ''' <summary>
    ''' Class de base de la gestion du mapping
    ''' </summary>
    Public MustInherit Class MapperConfigurationBase

#Region "Variables"

        Protected paramClassSource As ParameterExpression
        Protected delegateCall As [Delegate]
        Protected constructorFunc As Func(Of Type, Object)
        Protected isInitialized As Boolean = False
        Protected propertiesMapping As List(Of Tuple(Of LambdaExpression, LambdaExpression, Boolean))
        Protected propertiesToIgnore As List(Of PropertyInfo)

        Private mUseServiceLocator As Boolean
        Private mTypeSource As Type
        Private mTypeDest As Type
        Private mMemberToMap As List(Of MemberAssignment)

#End Region

#Region "Propriétés"

        ''' <summary>
        ''' Indique si l'on utilise le service d'injection
        ''' </summary>
        Public Property UseServiceLocator() As Boolean
            Get
                Return mUseServiceLocator
            End Get
            Protected Set(value As Boolean)
                mUseServiceLocator = value
            End Set
        End Property


        ''' <summary>
        ''' Gets the type source.
        ''' </summary>
        Public Property TypeSource() As Type
            Get
                Return mTypeSource
            End Get
            Private Set(value As Type)
                mTypeSource = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the type dest.
        ''' </summary>
        Public Property TypeDest() As Type
            Get
                Return mTypeDest
            End Get
            Private Set(value As Type)
                mTypeDest = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the member to map.
        ''' </summary>
        Public Property MemberToMap() As List(Of MemberAssignment)
            Get
                Return mMemberToMap
            End Get
            Protected Set(value As List(Of MemberAssignment))
                mMemberToMap = value
            End Set
        End Property

#End Region

#Region "Constructeur"

        ''' <summary>
        ''' Initializes a new instance of the <see cref="MapperConfigurationBase"/> class.
        ''' </summary>
        ''' <param name="source">The source.</param>
        ''' <param name="destination">The destination.</param>
        Public Sub New(source As Type, destination As Type)
            Me.TypeDest = destination
            Me.TypeSource = source
            Me.paramClassSource = Expression.Parameter(source, "source")
            Me.MemberToMap = New List(Of MemberAssignment)()
        End Sub

#End Region

#Region "Méthodes publiques"

        ''' <summary>
        ''' Changes the original source.
        ''' </summary>
        ''' <param name="property">The property.</param>
        ''' <param name="paramSource">The parameter source.</param>
        ''' <returns></returns>
        Protected Function ChangeSource([property] As PropertyInfo, paramSource As ParameterExpression) As List(Of MemberAssignment)
            If Not isInitialized Then
                Me.CreateMappingExpression(constructorFunc)
            End If
            Dim membersTransformed = New List(Of MemberAssignment)()
            Me.MemberToMap.ForEach(Sub(m)
                                       'Propriété de la classe source référençant la classe enfant
                                       Dim innerProperty = Expression.[Property](paramSource, [property])
                                       'Propriété de la class enfant
                                       If TypeOf m.Expression Is MemberExpression Then
                                           Dim outerProperty = Expression.[Property](innerProperty, TryCast(TryCast(m.Expression, MemberExpression).Member, PropertyInfo))
                                           'Affectation de la propriété de destination à la propriété de la class enfant
                                           Dim expBind = Expression.Bind(m.Member, outerProperty)

                                           membersTransformed.Add(expBind)

                                       End If

                                   End Sub)
            Return membersTransformed
        End Function

        ''' <summary>
        ''' Gets the delegate of mapping.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetDelegate() As [Delegate]
            If Not isInitialized Then
                Throw New MapperNotInitializedException(Me.TypeSource, Me.TypeDest)
            End If
            'Le fait de stocker le delegate réduit considérablement le temps de traitement 
            'Super Perf!!! 
            '(pas de compile de l'expression à chaque appel qui est très consommateur)
            If Me.delegateCall Is Nothing Then
                Dim exp = Me.GetMemberInitExpression()

                Me.delegateCall = Expression.Lambda(exp, paramClassSource).Compile()
            End If
            Return Me.delegateCall
        End Function

        ''' <summary>
        ''' Gets the type of the destination.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetDestinationType() As Type
            If Me.UseServiceLocator Then
                Return constructorFunc(Me.TypeDest).[GetType]()
            End If
            Return Me.TypeDest
        End Function
#End Region

#Region "Méthodes privées"

        Protected Function GetMapper(tSource As Type, tDest As Type, Optional throwExceptionOnNoFound As Boolean = True) As MapperConfigurationBase
            Dim mapperExterne As MapperConfigurationBase = Nothing

            mapperExterne = MapperConfigurationContainer.Instance.Find(tSource, tDest)
            'on levé une exception si rien n'est trouvé
            If mapperExterne Is Nothing AndAlso throwExceptionOnNoFound Then
                Throw New NoFoundMapperException(tSource, tDest)
            End If

            Return mapperExterne
        End Function

        Protected Sub CreateCommonMember()
            Dim propertiesSource = Me.TypeSource.GetProperties()
            Dim paramDest = Expression.Parameter(Me.TypeDest, "d")
            For Each propSource As PropertyInfo In propertiesSource
                Dim propDest = Me.TypeDest.GetProperty(propSource.Name)
                If propDest IsNot Nothing Then
                    Dim ignorePropDest = propertiesToIgnore.Exists(Function(x) x.Name = propDest.Name)
                    If propDest.CanWrite AndAlso Not ignorePropDest AndAlso propDest.PropertyType = propSource.PropertyType Then
                        Dim expSource = Expression.Lambda(Expression.MakeMemberAccess(paramClassSource, propSource), paramClassSource)
                        Dim expDest = Expression.Lambda(Expression.MakeMemberAccess(paramDest, propDest), paramDest)
                        Me.propertiesMapping.Add(Tuple.Create(Of LambdaExpression, LambdaExpression, Boolean)(expSource, expDest, False))
                    End If
                End If
            Next
        End Sub

        Protected Sub CreateMemberAssignement(memberSource As PropertyInfo, memberDest As PropertyInfo)

            'On supprime l'ancien (si appel plusieurs fois de la méthode)
            Me.MemberToMap.RemoveAll(Function(m) m.Member.Name = memberSource.Name)

            If Not memberDest.CanWrite Then
                Throw New ReadOnlyPropertyException(memberDest)
            End If
            'On regarde si la propriété est une liste
            Dim isList As Boolean = CheckAndConfigureTypeOfList(memberSource, memberDest)
            'Si pas une liste
            If Not isList Then

                CheckAndConfigureMembersMapping(memberSource, memberDest)
            End If
        End Sub

        Private Sub CheckAndConfigureMembersMapping(memberSource As PropertyInfo, memberDest As PropertyInfo)
            Dim mapperExterne As MapperConfigurationBase = Nothing
            'On regarde si la propriété source existe dans la source 
            '(on fait cela dans le cas où la propriété de l'expression n'est pas sur le même objet que celui de base)
            'Exemple
            'On veut mapper
            'destination.LaPropriete = source.SubClass.LaPropriete
            Dim [property] = Me.TypeSource.GetProperty(memberSource.Name)
            If [property] IsNot Nothing AndAlso memberSource.DeclaringType = Me.TypeSource.DeclaringType Then
                'Suppression du mapping d'origine de destination pour cette propriété
                Me.CheckAndRemoveMemberDest(memberDest.Name)
                'Suppression du mapping d'origine pour cette propriété
                Me.CheckAndRemoveMemberSource(memberSource.Name)
                If memberDest.PropertyType = [property].PropertyType Then
                    'Si la propriété correspond bien au type source
                    If [property].ReflectedType = Me.TypeSource Then
                        Dim memberClassSource As MemberExpression = Expression.[Property](paramClassSource, [property])

                        Dim expBind = Expression.Bind(memberDest, memberClassSource)
                        Me.MemberToMap.Add(expBind)
                    End If
                Else
                    mapperExterne = GetMapper(memberSource.PropertyType, memberDest.PropertyType, False)
                    If mapperExterne IsNot Nothing Then
                        CreateCheckIfNull([property], memberDest, mapperExterne)
                    End If
                End If
            Else
                'Si le type est différent on va regarde si l'on n'a pas un mappeur
                If memberSource.PropertyType <> memberDest.PropertyType Then
                    mapperExterne = GetMapper(memberSource.PropertyType, memberDest.PropertyType, False)
                    If mapperExterne IsNot Nothing Then
                        CreateCheckIfNull(memberSource, memberDest, mapperExterne)
                    Else
                        'On lève un exception
                        Throw New NotSameTypePropertyException(memberSource.PropertyType, memberDest.PropertyType)
                    End If
                    'Si on vient de la class de base
                ElseIf memberSource.ReflectedType = Me.TypeSource Then
                    Dim memberClassSource As MemberExpression = Expression.[Property](paramClassSource, memberSource)
                    Dim expBind = Expression.Bind(memberDest, memberClassSource)
                    Me.MemberToMap.Add(expBind)
                Else
                    'cas :
                    'destination.LaPropriete = source.SubClass.LaPropriete
                    If mapperExterne IsNot Nothing Then
                        CreateCheckIfNull(memberSource, memberDest, mapperExterne)
                    Else
                        'On va récupérer l'expression source
                        Dim expMember = Me.propertiesMapping.Find(Function(s) GetPropertyInfo(s.Item1).Name = memberSource.Name)
                        If expMember IsNot Nothing Then
                            'On crée la nouvelle expression
                            Dim memberAccess = Me.CreateMemberAssign(expMember.Item1, expMember.Item3)
                            'Et on assigne (cool)
                            Dim expBind = Expression.Bind(memberDest, memberAccess)
                            Me.MemberToMap.Add(expBind)
                        End If
                    End If
                End If
            End If

        End Sub

        Private Function CheckAndConfigureTypeOfList(memberSource As PropertyInfo, memberDest As PropertyInfo) As Boolean


            If memberSource.PropertyType.GetInterfaces().Count(Function(t) t = GetType(IList)) > 0 Then
                Dim mapperExterne As MapperConfigurationBase = Nothing

                'Type de la liste source
                Dim sourceTypeList = memberSource.PropertyType.GetGenericArguments()(0)
                ' Type de la liste de destination
                Dim destTypeList = memberDest.PropertyType.GetGenericArguments()(0)
                mapperExterne = Me.GetMapper(sourceTypeList, destTypeList)
                'Pour initialisé le mappeur
                mapperExterne.CreateMappingExpression(constructorFunc)
                'Suppression du mapping d'origine pour cette propriété
                Me.CheckAndRemoveMemberSource(memberSource.Name)
                Me.CheckAndRemoveMemberDest(memberDest.Name)
                'Appel de la méthode pour récupère l'expression lambda pour le select
                Dim methodeGetExpression As MethodInfo = mapperExterne.[GetType]().GetMethod("GetLambdaExpression")

                Dim expMappeur As Expression = TryCast(methodeGetExpression.Invoke(mapperExterne, Nothing), Expression)
                'On cherche la méthode select
                Dim selectMethod As MethodInfo = Nothing
                Dim selectsMethod = GetType(Enumerable).GetMethods().Where(Function(m) m.Name = "Select")
                'Pour prendre la bonne (y en a 2)
                For Each m As MethodInfo In selectsMethod
                    Dim parameters = m.GetParameters().Where(Function(p) p.Name.Equals("selector"))
                    For Each p As ParameterInfo In parameters
                        If p.ParameterType.GetGenericArguments().Count() = 2 Then
                            selectMethod = DirectCast(p.Member, MethodInfo)
                            Exit For
                        End If
                    Next
                    If selectMethod IsNot Nothing Then
                        Exit For
                    End If
                Next
                'On crée l'appel à la méthode  Select
                Dim [select] As Expression = Expression.[Call](selectMethod.MakeGenericMethod(sourceTypeList, destTypeList), New Expression() {Expression.[Property](paramClassSource, memberSource), expMappeur})
                'On crée l'appel à la méthode  ToList
                Dim toList As Expression = Expression.[Call](GetType(Enumerable).GetMethod("ToList").MakeGenericMethod(destTypeList), [select])
                Dim asExp As Expression = Expression.TypeAs(toList, memberDest.PropertyType)
                'test si la propriété source est nul
                Dim checkIfNull = Expression.NotEqual(Expression.[Property](paramClassSource, memberSource), Expression.Constant(Nothing))
                Dim expCondition = Expression.Condition(checkIfNull, asExp, Expression.Constant(Nothing, memberDest.PropertyType))
                'Affectation de la propriétés de destination
                Dim expBind = Expression.Bind(memberDest, expCondition)
                Me.MemberToMap.Add(expBind)
                Return True
            End If
            Return False
        End Function

        Protected Sub CreateCheckIfNull(memberSource As PropertyInfo, memberDest As PropertyInfo, mapperExterne As MapperConfigurationBase)
            Dim checkIfNull = Expression.NotEqual(Expression.[Property](paramClassSource, memberSource), Expression.Constant(Nothing))

            'Création de la nouvelle class de destination
            Dim newClassDest As NewExpression = Expression.[New](mapperExterne.GetDestinationType())

            'On crée la nouvelle affectation des propriétés à la source
            Dim newMembers As List(Of MemberAssignment) = mapperExterne.ChangeSource(memberSource, Me.paramClassSource)

            'Initialisation de l'affectation des propriétés de l'objet de destination
            Dim exp As Expression = Expression.MemberInit(newClassDest, newMembers)

            'Création de la condition de test
            Dim expCondition As Expression = Expression.Condition(checkIfNull, exp, Expression.Constant(Nothing, memberDest.PropertyType))

            'Affectation de la propriétés de destination
            Dim expBind As MemberAssignment = Expression.Bind(memberDest, expCondition)
            Me.MemberToMap.Add(expBind)
        End Sub

        Protected Sub CheckAndRemoveMemberDest(properyName As String)
            Dim exp As Predicate(Of MemberAssignment) = Function(m) m.Member.Name = properyName
            If Me.MemberToMap.Exists(exp) Then
                Me.MemberToMap.RemoveAll(exp)
            End If

        End Sub

        Protected Sub CheckAndRemoveMemberSource(properyName As String)
            Dim exp As Predicate(Of MemberAssignment) = Function(m) TypeOf m.Expression Is MemberExpression _
                                                            AndAlso TryCast(m.Expression, MemberExpression).Member.Name = properyName
            If Me.MemberToMap.Exists(exp) Then
                Me.MemberToMap.RemoveAll(exp)
            End If
        End Sub

        Protected Function GetMemberInitExpression() As MemberInitExpression
            Dim typeDest As Type = Me.GetDestinationType()

            Dim newClassDest As NewExpression = Expression.[New](typeDest)

            'new ClassDestination() { Test1 = c1.Test1, Test2 = c1.Test2 };
            Dim exp = Expression.MemberInit(newClassDest, Me.MemberToMap)
            Return exp
        End Function

        Protected Function CreateMemberAssign(propertyExpression As Expression, checkIfNull As Boolean) As Expression
            Dim visitor As New MapperExpressionVisitor(checkIfNull, paramClassSource)
            'Visite l'expression pour sa transformation
            Dim result = visitor.Visit(propertyExpression)

            If result.NodeType = ExpressionType.Lambda Then
                Return TryCast(result, LambdaExpression).Body
            End If
            Return result
        End Function

        Protected Function GetPropertyInfo(propertyExpression As LambdaExpression) As PropertyInfo
            Select Case propertyExpression.Body.NodeType
                Case ExpressionType.Convert
                    Dim operand As Expression = TryCast(propertyExpression.Body, UnaryExpression).Operand
                    If operand.NodeType = ExpressionType.MemberAccess Then
                        Return TryCast(TryCast(operand, MemberExpression).Member, PropertyInfo)
                    Else
                        Throw New NotImplementedException("Ce type d'expression n'est pas prit en charge")
                    End If


                Case ExpressionType.MemberAccess
                    Return TryCast(TryCast(propertyExpression.Body, MemberExpression).Member, PropertyInfo)
                Case Else
                    Throw New NotImplementedException("Ce type d'expression n'est pas valide")
            End Select
        End Function

        Protected Function ForMember(getPropertySource As LambdaExpression, getPropertyDest As LambdaExpression, Optional checkIfNull As Boolean = False) As MapperConfigurationBase

            'Ajout dans la liste pour le traitement ultérieur
            Me.propertiesMapping.Add(Tuple.Create(Of LambdaExpression, LambdaExpression, Boolean)(getPropertySource, getPropertyDest, checkIfNull))
            Return Me
        End Function

        Friend Overridable Sub CreateMappingExpression(constructor As Func(Of Type, Object))
            If Not Me.isInitialized Then
                Me.isInitialized = True
                If Me.UseServiceLocator Then
                    Me.constructorFunc = constructor
                End If
                CreateCommonMember()
                Me.GetDelegate()
            End If
        End Sub

        Friend Function GetDataLoadOptionsLinq() As DataLoadOptions
            Dim options As New DataLoadOptions()
            'On prend tous le objets qui sont lie au data model
            Dim propertiesLinq As IEnumerable(Of Expression) = Me.MemberToMap.Where(Function(p) p.Expression.NodeType = ExpressionType.Conditional _
                                                                                        AndAlso TryCast(TryCast(p.Expression, ConditionalExpression).Test, BinaryExpression).Left.Type.BaseType.Name = "EntityBase") _
                                                                                    .[Select](Function(p) TryCast(TryCast(p.Expression, ConditionalExpression).Test, BinaryExpression).Left)
            Dim memberInserted As New List(Of String)()

            For Each prop As MemberExpression In propertiesLinq.Cast(Of MemberExpression)()
                If Not memberInserted.Contains(prop.Member.Name) Then
                    options.LoadWith(Expression.Lambda(prop))
                    memberInserted.Add(prop.Member.Name)
                End If
            Next
            Return options
        End Function

#End Region

    End Class
End Namespace