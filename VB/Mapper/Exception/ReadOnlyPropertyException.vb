Imports System
Imports System.Linq
Imports System.Reflection

Namespace Exception
    Public Class ReadOnlyPropertyException
        Inherits System.Exception

        Public Sub New([property] As PropertyInfo)

            MyBase.New("La propriété '" + [property].Name + "' de destination est en lecture seule")
        End Sub
    End Class
End Namespace
