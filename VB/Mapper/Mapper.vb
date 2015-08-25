Imports System.Data.Linq
Imports System.Linq.Expressions
Imports MapperExpression.Core
Imports MapperExpression.Exception
Imports System.Reflection


''' <summary>
''' Class de base pour l'accès au mapping
''' </summary>
Public Class Mapper

#Region "Variables"

    Private Shared current As MapperConfigurationBase
    Private Shared constructorFunc As Func(Of Type, Object)

#End Region

#Region "Méthodes publiques"

    ''' <summary>
    ''' Maps the specified source.
    ''' </summary>
    ''' <typeparam name="TSource">The type of the source.</typeparam>
    ''' <typeparam name="TDest">The type of the dest.</typeparam>
    ''' <param name="source">The business.</param>
    ''' <returns></returns>
    Public Shared Function Map(Of TSource As Class, TDest As Class)(source As TSource) As TDest
        Try
            Dim mapper = GetMapper(Of TSource, TDest)()

            Dim query = mapper.GetFuncDelegate()

            If query IsNot Nothing Then
                Dim result As TDest = query(source)

                'Action à exécuter après le mapping
                mapper.ExecuteAfterActions(source, result)

                Return result

            End If
        Catch ex As System.Exception

            Throw ex
        End Try
        Return Nothing
    End Function

    ''' <summary>
    ''' Gets the query expression.
    ''' </summary>
    ''' <typeparam name="TSource">The type of the source.</typeparam>
    ''' <typeparam name="TDest">The type of the dest.</typeparam>
    ''' <returns></returns>
    Public Shared Function GetQueryExpression(Of TSource As Class, TDest As Class)() As Expression(Of Func(Of TSource, TDest))
        Return GetMapper(Of TSource, TDest)().GetLambdaExpression()
    End Function

    ''' <summary>
    ''' Creates the map.
    ''' </summary>
    ''' <typeparam name="TSource">The type of the source.</typeparam>
    ''' <typeparam name="TDest">The type of the dest.</typeparam>
    ''' <returns></returns>
    Public Shared Function CreateMap(Of TSource As Class, TDest As Class)() As MapperConfiguration(Of TSource, TDest)
        Dim map = MapperConfigurationContainer.Instance.Find(GetType(TSource), GetType(TDest))

        If map Is Nothing Then
            map = New MapperConfiguration(Of TSource, TDest)()
            MapperConfigurationContainer.Instance.Add(map)
        End If

        Return TryCast(map, MapperConfiguration(Of TSource, TDest))
    End Function

    ''' <summary>
    ''' Indique le service d'injection à utilisée
    ''' </summary>
    ''' <param name="constructor">The constructor.</param>
    Public Shared Sub ConstructServicesUsing(constructor As Func(Of Type, Object))
        constructorFunc = constructor
    End Sub

    ''' <summary>
    ''' Efface tout les mappeurs existants
    ''' </summary>
    Public Shared Sub Reset()
        MapperConfigurationContainer.Instance.Clear()
    End Sub

    ''' <summary>
    ''' Gets the data load options for linqToSql.
    ''' </summary>
    ''' <typeparam name="TSource">The type of the source.</typeparam>
    ''' <typeparam name="TDest">The type of the dest.</typeparam>
    ''' <returns></returns>
    Public Shared Function GetDataLoadOptionsLinq(Of TSource As Class, TDest As Class)() As DataLoadOptions
        Dim map = GetMapper(Of TSource, TDest)()
        Return map.GetDataLoadOptionsLinq()
    End Function

    ''' <summary>
    ''' Initialises les mappeurs.
    ''' </summary>
    Public Shared Sub Initialize()
        Dim configRegister As MapperConfigurationContainer = MapperConfigurationContainer.Instance
        For Each mapper As MapperConfigurationBase In configRegister
            mapper.CreateMappingExpression(constructorFunc)
        Next
    End Sub

    ''' <summary>
    ''' Gets the query.
    ''' </summary>
    ''' <typeparam name="TSource">The type of the source.</typeparam>
    ''' <typeparam name="TDest">The type of the dest.</typeparam>
    ''' <returns></returns>
    Public Shared Function GetQuery(Of TSource As Class, TDest As Class)() As Func(Of TSource, TDest)
        Return GetMapper(Of TSource, TDest)().GetFuncDelegate()
    End Function

#End Region

#Region "Méthodes privées"

    ''' <summary>
    ''' Gets the mapper.
    ''' </summary>
    ''' <typeparam name="TSource">The type of the source.</typeparam>
    ''' <typeparam name="TDest">The type of the dest.</typeparam>
    ''' <returns></returns>
    Friend Shared Function GetMapper(Of TSource As Class, TDest As Class)() As MapperConfiguration(Of TSource, TDest)
        If current Is Nothing _
            OrElse (current IsNot Nothing _
                                      AndAlso current.TypeDest.FullName <> GetType(TDest).FullName _
                                      AndAlso current.TypeSource.FullName <> GetType(TSource).FullName) Then
            current = GetMapper(GetType(TSource), GetType(TDest))
        End If

        Return TryCast(current, MapperConfiguration(Of TSource, TDest))

    End Function

    ''' <summary>
    ''' Gets the mapper.
    ''' </summary>
    ''' <param name="tSource">The t source.</param>
    ''' <param name="tDest">The t dest.</param>
    ''' <returns></returns>
    ''' <exception cref="System.Exception">Impossible de trouver la configuration entre le type source + tSource.Name +  et le type destination + tDest.Name</exception>
    Friend Shared Function GetMapper(tSource As Type, tDest As Type) As MapperConfigurationBase
        Dim mapConfig = MapperConfigurationContainer.Instance.Find(tSource, tDest)
        If mapConfig IsNot Nothing Then
            Return mapConfig
        Else
            Throw New NoFoundMapperException(tSource, tDest)
        End If
    End Function

    
#End Region

End Class



