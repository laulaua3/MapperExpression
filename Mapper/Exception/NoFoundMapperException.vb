Imports System

Namespace Exception
    Public Class NoFoundMapperException
        Inherits System.Exception

        Public Sub New(source As Type, dest As Type)

            MyBase.New("Le mapping pour les types '" + source.Name + "' et '" + dest.Name + "' ne sont pas configurés")
        End Sub
    End Class
End Namespace
