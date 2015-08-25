
Imports System.Collections.Generic
Imports System.Linq

Namespace Core

    ''' <summary>
    ''' Singleton du stockage des mappeurs
    ''' </summary>
    Friend Class MapperConfigurationContainer
        Inherits List(Of MapperConfigurationBase)

        Private Shared currentInstance As MapperConfigurationContainer

        Friend Shared ReadOnly Property Instance() As MapperConfigurationContainer
            Get
                If currentInstance Is Nothing Then
                    currentInstance = New MapperConfigurationContainer()
                End If
                Return currentInstance
            End Get
        End Property


        Private Sub New()
        End Sub

        Friend Overloads Function Find(source As Type, destination As Type) As MapperConfigurationBase
            Return Me.FirstOrDefault(Function(m) m.TypeSource = source AndAlso m.TypeDest = destination)
        End Function
    End Class

End Namespace
