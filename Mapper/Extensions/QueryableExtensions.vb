Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports MapperExpression.Core
Imports System.Linq.Expressions

Namespace Extensions

    Public Module QueryableExtensions
#Region "Extensions IQueryable"

        ''' <summary>
        '''  Trie les éléments d'une séquence dans l'ordre croissant selon une clé.
        ''' </summary>
        ''' <typeparam name="TSource">type de la source</typeparam>
        ''' <typeparam name="TDest">type de destination</typeparam>
        ''' <param name="query">Séquence de valeurs à classer.</param>
        ''' <param name="sortedPropertyDestName">Nom de la colonne de destination</param>
        ''' <returns></returns>
        <Extension()> _
        Public Function OrderBy(Of TSource As Class, TDest As Class)(query As IQueryable(Of TSource), sortedPropertyDestName As String) As IOrderedQueryable(Of TSource)
            Return CreateMethodCallOrdered(Of TSource, TDest)(query, MethodBase.GetCurrentMethod().Name, sortedPropertyDestName)
        End Function

        ''' <summary>
        '''  Trie les éléments d'une séquence dans l'ordre décroissant selon une clé.
        ''' </summary>
        ''' <typeparam name="TSource">type de la source</typeparam>
        ''' <typeparam name="TDest">type de destination</typeparam>
        ''' <param name="query">Séquence de valeurs à classer.</param>
        ''' <param name="sortedPropertyDestName">Nom de la colonne de destination</param>
        ''' <returns></returns>
        <Extension()> _
        Public Function OrderByDescending(Of TSource As Class, TDest As Class)(query As IQueryable(Of TSource), sortedPropertyDestName As String) As IOrderedQueryable(Of TSource)
            Return CreateMethodCallOrdered(Of TSource, TDest)(query, MethodBase.GetCurrentMethod().Name, sortedPropertyDestName)
        End Function


        ''' <summary>
        '''  Trie les éléments d'une séquence dans l'ordre croissant selon une clé.
        ''' </summary>
        ''' <typeparam name="TSource">type de la source</typeparam>
        ''' <typeparam name="TDest">type de destination</typeparam>
        ''' <param name="query">Séquence de valeurs à classer.</param>
        ''' <param name="sortedPropertyDestName">Nom de la colonne de destination</param>
        ''' <returns></returns>
        <Extension()> _
        Public Function ThenBy(Of TSource As Class, TDest As Class)(query As IQueryable(Of TSource), sortedPropertyDestName As String) As IOrderedQueryable(Of TSource)
            Return CreateMethodCallOrdered(Of TSource, TDest)(query, MethodBase.GetCurrentMethod().Name, sortedPropertyDestName)
        End Function

        ''' <summary>
        '''  Trie les éléments d'une séquence dans l'ordre décroissant selon une clé.
        ''' </summary>
        ''' <typeparam name="TSource">type de la source</typeparam>
        ''' <typeparam name="TDest">type de destination</typeparam>
        ''' <param name="query">Séquence de valeurs à classer.</param>
        ''' <param name="sortedPropertyDestName">Nom de la colonne de destination</param>
        ''' <returns></returns>
        <Extension()> _
        Public Function ThenByDescending(Of TSource As Class, TDest As Class)(query As IQueryable(Of TSource), sortedPropertyDestName As String) As IOrderedQueryable(Of TSource)

            Return CreateMethodCallOrdered(Of TSource, TDest)(query, MethodBase.GetCurrentMethod().Name, sortedPropertyDestName)
        End Function

        ''' <summary>
        ''' Projette chaque élément d'une séquence dans un nouveau formulaire en incorporant l'objet de destination
        ''' </summary>
        ''' <typeparam name="TSource">type de la source</typeparam>
        ''' <typeparam name="TDest">type de destination</typeparam>
        ''' <param name="query">Séquence de valeurs à classer.</param>
        ''' <returns></returns>
        <Extension()> _
        Public Function [Select](Of TSource As Class, TDest As Class)(query As IQueryable(Of TSource)) As IQueryable(Of TDest)

            Return query.Select(GetMapper(Of TSource, TDest)().GetLambdaExpression())

        End Function

#End Region

#Region "Méthodes privées"

        Private Function CreateMethodCallOrdered(Of TSource As Class, TDest As Class)(query As IQueryable(Of TSource), methodName As String, sortedPropertySourceName As String) As IOrderedQueryable(Of TSource)
            Dim mapper As MapperConfiguration(Of TSource, TDest) = GetMapper(Of TSource, TDest)()

            Dim prop As PropertyInfo = mapper.GetPropertyInfoSource(sortedPropertySourceName)

            Dim lambda As LambdaExpression = mapper.GetSortedExpression(sortedPropertySourceName)

            Dim resultExp As MethodCallExpression = Expression.[Call](GetType(Queryable), methodName, New Type() {GetType(TSource), prop.PropertyType}, query.Expression, Expression.Quote(lambda))

            Return TryCast(query.Provider.CreateQuery(Of TSource)(resultExp), IOrderedQueryable(Of TSource))
        End Function

        Private Function GetMapper(Of TSource As Class, TDest As Class)() As MapperConfiguration(Of TSource, TDest)
            Return Mapper.GetMapper(Of TSource, TDest)()
        End Function
#End Region
    End Module
End Namespace