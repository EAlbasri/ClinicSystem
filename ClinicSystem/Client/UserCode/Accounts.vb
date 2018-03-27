
Namespace LightSwitchApplication

    Public Class Accounts

        Private Sub AccountFiles_Loaded(succeeded As Boolean)
            ' Write your code here.
            Me.SetDisplayNameFromEntity(Me.AccountFiles)
        End Sub

        Private Sub AccountFiles_Changed()
            ' Write your code here.
            Me.SetDisplayNameFromEntity(Me.AccountFiles)
        End Sub

        Private Sub Accounts_Saved()
            ' Write your code here.
            Me.SetDisplayNameFromEntity(Me.AccountFiles)
        End Sub

    End Class

End Namespace