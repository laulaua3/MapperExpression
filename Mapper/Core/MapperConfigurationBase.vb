Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Collections

Namespace Core


    ''' <summary>
    ''' Class de base de la gestion du mapping
    ''' </summary>
    Public MustInherit Class MapperConfigurationBase


        Protected paramClassSource As ParameterExpression


        Protected _isInitialized As Boolean = False

        ''' <summary>
        ''' Gets the type source.
        ''' </summary>
        ''' <value>
        ''' The type source.
        ''' </value>
        Public Property TypeSource() As Type
            Get
                Return m_TypeSource
            End Get
            Private Set(value As Type)
                m_TypeSource = value
            End Set
        End Property
        Private m_TypeSource As Type
        ''' <summary>
        ''' Gets the type dest.
        ''' </summary>
        ''' <value>
        ''' The type dest.
        ''' </value>
        Public Property TypeDest() As Type
            Get
                Return m_TypeDest
            End Get
            Private Set(value As Type)
                m_TypeDest = value
            End Set
        End Property
        Private m_TypeDest As Type
        ''' <summary>
        ''' Gets or sets the member to map.
        ''' </summary>
        ''' <value>
        ''' The member to map.
        ''' </value>
        Public Property MemberToMap() As List(Of MemberAssignment)
            Get
                Return m_MemberToMap
            End Get
            Protected Set(value As List(Of MemberAssignment))
                m_MemberToMap = value
            End Set
        End Property
        Private m_MemberToMap As List(Of MemberAssignment)

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

        ''' <summary>
        ''' Changes the original source.
        ''' </summary>
        ''' <param name="property">The property.</param>
        ''' <param name="paramSource">The parameter source.</param>
        ''' <returns></returns>
        Public Function ChangeSource([property] As PropertyInfo, paramSource As ParameterExpression) As List(Of MemberAssignment)
            If Not _isInitialized Then
                Me.CreateMappingExpression()
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
        ''' Creates the mapping expression.
        ''' </summary>
        Public Overridable Sub CreateMappingExpression()
            CreateCommonMember()
        End Sub
        ''' <summary>
        ''' Gets the mapper.
        ''' </summary>
        ''' <param name="tSource">The t source.</param>
        ''' <param name="tDest">The t dest.</param>
        ''' <param name="exceptionOnNoFound">if set to <c>true</c> [exception on no found].</param>
        ''' <returns></returns>
        ''' <exception cref="Exception">Les types ' + tSource.Name + ' et ' + tDest.Name + ' ne sont pas du même type ou ne sont pas mappés</exception>
        Protected Function GetMapper(tSource As Type, tDest As Type, Optional exceptionOnNoFound As Boolean = True) As MapperConfigurationBase
            Dim mapperExterne As MapperConfigurationBase = Nothing

            mapperExterne = MapperConfigurationRegister.Instance.Find(tSource, tDest)
            'on levé une exception si rien n'est trouvé
            If mapperExterne Is Nothing AndAlso exceptionOnNoFound Then
                Throw New Exception("Les types '" + tSource.Name + "' et '" + tDest.Name + "' ne sont pas du même type ou ne sont pas mappés")
            End If

            Return mapperExterne
        End Function

        ''' <summary>
        ''' Création de la liaison des propriétés source et de destination générique
        ''' </summary>
        Protected Sub CreateCommonMember()
            Dim propertiesSource = Me.TypeSource.GetProperties()

            For Each propSource As PropertyInfo In propertiesSource
                Dim propDest = Me.TypeDest.GetProperty(propSource.Name)
                If Not (propDest Is Nothing) AndAlso propDest.CanWrite Then
                    CreateMemberAssignement(propSource, propDest, False)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Creates the member assignement.
        ''' </summary>
        ''' <param name="memberSource">The member source.</param>
        ''' <param name="memberDest">The member dest.</param>
        ''' <param name="exceptionMapperNoFound">if set to <c>true</c> [exception mapper no found].</param>
        ''' <exception cref="Exception">
        ''' La propriété ' + memberDest.Name + ' de destination est en lecture seule
        ''' or
        ''' La propriété ' + memberSource.Name + 'n'existe pas pour le type ' + memberSource.DeclaringType.ToString() + '
        ''' </exception>
        Protected Sub CreateMemberAssignement(memberSource As PropertyInfo, memberDest As PropertyInfo, exceptionMapperNoFound As Boolean)
            Dim mapperExterne As MapperConfigurationBase = Nothing
            'On supprime l'ancien (si appel plusieurs fois de la méthode)
            Me.MemberToMap.RemoveAll(Function(m) m.Member.Name = memberSource.Name)

            If Not memberDest.CanWrite Then
                Throw New Exception("La propriété '" + memberDest.Name + "' de destination est en lecture seule")
            End If
            'Cas des listes
            If memberSource.PropertyType.GetInterfaces().Count(Function(t) t = GetType(IList)) > 0 Then
                'Type de la liste source
                Dim sourceTypeList = memberSource.PropertyType.GetGenericArguments()(0)
                ' Type de la liste de destination
                Dim destTypeList = memberDest.PropertyType.GetGenericArguments()(0)
                mapperExterne = Me.GetMapper(sourceTypeList, destTypeList)
                mapperExterne.CreateMappingExpression()

                Me.CheckAndRemoveMemberDest(memberDest.Name)
                'Suppression du mapping d'origine pour cette propriété
                Me.CheckAndRemoveMemberSource(memberSource.Name)


                'Appel de la méthode pour récupère l'expression lambda pour le select
                Dim methodeGetExpression As MethodInfo = mapperExterne.[GetType]().GetMethod("GetLambdaExpression")
                Dim expression__1 = TryCast(methodeGetExpression.Invoke(mapperExterne, Nothing), Expression)
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
                    If Not (selectMethod Is Nothing) Then
                        Exit For
                    End If
                Next
                'On crée l'appel à la méthode  Select
                Dim [select] As Expression = Expression.[Call](selectMethod.MakeGenericMethod(sourceTypeList, destTypeList), New Expression() {Expression.[Property](paramClassSource, memberSource), expression__1})
                'On crée l'appel à la méthode  ToList
                Dim toList As Expression = Expression.[Call](GetType(Enumerable).GetMethod("ToList").MakeGenericMethod(destTypeList), [select])
                Dim asExp As Expression = Expression.TypeAs(toList, memberDest.PropertyType)
                'test si la propriété source est nul
                Dim checkIfNull = Expression.NotEqual(Expression.[Property](paramClassSource, memberSource), Expression.Constant(Nothing))
                Dim expCondition = Expression.Condition(checkIfNull, asExp, Expression.Constant(Nothing, memberDest.PropertyType))
                'Affectation de la propriétés de destination
                Dim expBind = Expression.Bind(memberDest, expCondition)
                Me.MemberToMap.Add(expBind)
            Else
                'Si le type est différent on va regarde si l'on n'a pas un mappeur
                If memberSource.PropertyType <> memberDest.PropertyType Then

                    mapperExterne = GetMapper(memberSource.PropertyType, memberDest.PropertyType, exceptionMapperNoFound)
                End If
                'On regarde si la propriété source existe dans la source
                Dim [property] = memberSource.ReflectedType.GetProperty(memberSource.Name)
                If [property] <> Nothing Then
                    'Suppression du mapping d'origine de destination pour cette propriétés
                    Me.CheckAndRemoveMemberDest(memberDest.Name)
                    'Suppression du mapping d'origine pour cette propriétés
                    Me.CheckAndRemoveMemberSource(memberSource.Name)
                    If mapperExterne Is Nothing Then
                        mapperExterne = MapperConfigurationRegister.Instance.Find(memberSource.PropertyType, memberDest.PropertyType)
                    End If
                    If Not (mapperExterne Is Nothing) Then
                        Dim checkIfNull = Expression.NotEqual(Expression.[Property](paramClassSource, [property]), Expression.Constant(Nothing))
                        'Création de la nouvelle class de destination
                        Dim newClassDest = Expression.[New](mapperExterne.TypeDest)

                        'On crée la nouvelle affectation des propriétés avec la source
                        Dim newMembers = mapperExterne.ChangeSource([property], Me.paramClassSource)

                        'Initialisation de l'affectation des propriétés de l'objet de destination
                        Dim exp = Expression.MemberInit(newClassDest, newMembers)

                        'Création de la condition de test
                        Dim expCondition = Expression.Condition(checkIfNull, exp, Expression.Constant(Nothing, memberDest.PropertyType))

                        'Affectation de la propriétés de destination
                        Dim expBind = Expression.Bind(memberDest, expCondition)
                        Me.MemberToMap.Add(expBind)
                    ElseIf memberDest.PropertyType = [property].PropertyType Then
                        Dim memberClassSource As MemberExpression = Expression.[Property](paramClassSource, [property])
                        Dim expBind = Expression.Bind(memberDest, memberClassSource)
                        Me.MemberToMap.Add(expBind)
                    End If
                Else
                    Throw New Exception("La propriété '" + memberSource.Name + "'n'existe pas pour le type '" + memberSource.DeclaringType.ToString() + "'")
                End If
            End If

        End Sub

        ''' <summary>
        ''' Check  and remove member dest.
        ''' </summary>
        ''' <param name="properyName">Name of the propery.</param>
        Protected Sub CheckAndRemoveMemberDest(properyName As String)
            If Me.MemberToMap.Exists(Function(m) m.Member.Name = properyName) Then
                Me.MemberToMap.RemoveAll(Function(m) m.Member.Name = properyName)
            End If

        End Sub

        ''' <summary>
        ''' Check  and remove member source.
        ''' </summary>
        ''' <param name="properyName">Name of the propery.</param>
        Protected Sub CheckAndRemoveMemberSource(properyName As String)
            If Me.MemberToMap.Exists(Function(m) TypeOf m.Expression Is MemberExpression AndAlso TryCast(m.Expression, MemberExpression).Member.Name = properyName) Then
                Me.MemberToMap.RemoveAll(Function(m) TypeOf m.Expression Is MemberExpression AndAlso TryCast(m.Expression, MemberExpression).Member.Name = properyName)
            End If

        End Sub
    End Class

End Namespace
