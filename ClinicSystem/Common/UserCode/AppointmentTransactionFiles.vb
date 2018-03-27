
Namespace LightSwitchApplication

    Public Class AppointmentTransactionFiles

        Private Sub TransactionAmount_Validate(results As EntityValidationResultsBuilder)
            ' results.AddPropertyError("<Error-Message>")
            If Not IsNothing(Appointment) Then
                If Not IsNothing(TransactionAmount) Then
                    If Appointment.Balance < 0 Or TransactionAmount = 0 Then
                        results.AddPropertyError("Wrong Amount")
                    End If
                End If
            End If
        End Sub
    End Class

End Namespace
