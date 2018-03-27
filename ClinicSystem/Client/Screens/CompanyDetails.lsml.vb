
Namespace LightSwitchApplication

    Public Class CompanyDetails

        Private Sub CompanyDetails_InitializeDataWorkspace(ByVal saveChangesTo As Global.System.Collections.Generic.List(Of Global.Microsoft.LightSwitch.IDataService))
            ' Write your code here.
            'Me.CompanyDetailsFilesProperty = New CompanyDetailsFiles()

            Dim MyComp = CompanyDetailsFilesSet.FirstOrDefault

            If IsNothing(MyComp) Then
                Me.CompanyDetailsFilesProperty = New CompanyDetailsFiles
            Else
                Me.CompanyDetailsFilesProperty = MyComp
                If MyComp.InitialBankBalance > 0 Then
                    Me.FindControl("InitialBankBalance").IsReadOnly = True
                End If
                If MyComp.InitialCashBalance > 0 Then
                    Me.FindControl("InitialCashBalance").IsReadOnly = True
                End If
            End If

            Dim MyAcc = AccountFilesSet.FirstOrDefault

            If IsNothing(MyAcc) Then
                Me.AccountsProperty = New AccountFiles
                Me.CompanyDetailsFilesProperty.Account = Me.AccountsProperty
            End If

        End Sub

        Private Sub CompanyDetails_Saved()
            ' Write your code here.
            'Me.Close(False)
            'Application.Current.ShowDefaultScreen(Me.CompanyDetailsFilesProperty)
        End Sub

    End Class

End Namespace