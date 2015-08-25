Imports System

Imports System.Linq

Imports System.Reflection

Namespace Exception
    Public Class PropertyNoExistException
        Inherits System.Exception
        Public Sub New([property] As PropertyInfo)

            Me.New([property].Name, [property].DeclaringType)
        End Sub

        Public Sub New(propertyName As String, typeObject As Type)

            MyBase.New("La propriété '" + propertyName + "' n'existe pas pour le type '" + typeObject.ToString() + "'")
        End Sub

    End Class
End Namespace
