
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Linq.Expressions

Imports System.Data.Linq
Imports MapperExpression.Core



''' <summary>
''' Class de base pour l'accès au mapping
''' </summary>
Public Class Mapper

    Private Shared current As MapperConfigurationBase

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

            Dim query = GetMapper(Of TSource, TDest)().GetDelegate()
            If query IsNot Nothing Then
                Return query(source)
            End If
        Catch ex As Exception

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

        Dim map = MapperConfigurationRegister.Instance.Find(GetType(TSource), GetType(TDest))
        If map Is Nothing Then
            map = New MapperConfiguration(Of TSource, TDest)()
            MapperConfigurationRegister.Instance.Add(map)
        End If
        Return TryCast(map, MapperConfiguration(Of TSource, TDest))
    End Function
    ''' <summary>
    ''' Efface tout les mappeur existants
    ''' </summary>
    Public Shared Sub Reset()
        MapperConfigurationRegister.Instance.Clear()
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
        Dim configRegister = MapperConfigurationRegister.Instance
        For Each mapper As MapperConfigurationBase In configRegister
            mapper.CreateMappingExpression()
        Next
    End Sub
    ''' <summary>
    ''' Gets the query.
    ''' </summary>
    ''' <typeparam name="TSource">The type of the source.</typeparam>
    ''' <typeparam name="TDest">The type of the dest.</typeparam>
    ''' <returns></returns>
    Public Shared Function GetQuery(Of TSource As Class, TDest As Class)() As Func(Of TSource, TDest)
        Return GetMapper(Of TSource, TDest)().GetDelegate()
    End Function
#End Region

#Region "Méthodes privées"

    Private Shared Function GetMapper(Of TSource As Class, TDest As Class)() As MapperConfiguration(Of TSource, TDest)
        If current Is Nothing OrElse (current IsNot Nothing AndAlso current.TypeDest.FullName <> GetType(TDest).FullName AndAlso current.TypeSource.FullName <> GetType(TSource).FullName) Then
            current = GetMapper(GetType(TSource), GetType(TDest))
        End If

        Return TryCast(current, MapperConfiguration(Of TSource, TDest))

    End Function

    Private Shared Function GetMapper(tSource As Type, tDest As Type) As MapperConfigurationBase
        Dim mapConfig = MapperConfigurationRegister.Instance.Find(tSource, tDest)
        If mapConfig IsNot Nothing Then
            Return mapConfig
        Else
            Throw New Exception("Impossible de trouver la configuration entre le type source" + tSource.Name + " et le type destination" + tDest.Name)
        End If

    End Function
#End Region
End Class

