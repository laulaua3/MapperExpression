Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Linq.Expressions
Imports System.Reflection
Namespace Core
    ''' <summary>
    ''' Singleton des mappeurs
    ''' </summary>
    Friend Class MapperConfigurationRegister
        Inherits List(Of MapperConfigurationBase)

        Private Shared _instance As MapperConfigurationRegister
        Private Shared _object As Object = New Object()
        Friend Shared ReadOnly Property Instance() As MapperConfigurationRegister
            Get
                SyncLock _object
                    If _instance Is Nothing Then
                        _instance = New MapperConfigurationRegister()
                    End If
                    Return _instance
                End SyncLock
            End Get
        End Property

        Private Sub New()
        End Sub

        Friend Overloads Function Find(source As Type, destination As Type) As MapperConfigurationBase

            Return MyBase.Find(Function(m) m.TypeSource = source AndAlso m.TypeDest = destination)
        End Function
    End Class
End Namespace

