Imports System

Namespace Exception
    Public Class MapperExistException
        Inherits System.Exception


        Public Sub New(source As Type, dest As Type)

            MyBase.New("Un mappeur existe déjà pour le type source '" + source.FullName + "' et le type de destination '" + dest.FullName + "'")
        End Sub
    End Class
End Namespace
