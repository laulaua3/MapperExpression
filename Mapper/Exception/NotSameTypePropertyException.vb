Namespace Exception
    Public Class NotSameTypePropertyException
        Inherits System.Exception

        Public Sub New(typeSource As Type, typeDest As Type)
            MyBase.New("Les propriétés source et de destination ne sont pas du même type(source est de type " + typeSource.Name + " et destination est de type " + typeDest.Name + ")")
        End Sub
    End Class
End Namespace
