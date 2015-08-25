
Namespace Exception
    Public Class MapperNotInitializedException
        Inherits System.Exception

        Sub New(typeSource As Type, typeDest As Type)
            MyBase.New("Le mappeur pour le type source '" + typeSource.FullName + "' et le type de destination '" + typeDest.FullName + "' n'est pas initialisé (appelé Mapper.Initialise()")
        End Sub

    End Class
End Namespace
