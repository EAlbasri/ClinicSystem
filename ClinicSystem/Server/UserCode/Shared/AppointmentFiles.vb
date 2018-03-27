
Namespace LightSwitchApplication

    Public Class AppointmentFiles

        Private Sub ServicesTotal_Compute(ByRef result As Decimal)
            ' Set result to the desired field value
            result = AppointmentServicesCollection.Sum(Function(g) g.Fees)
        End Sub

        Private Sub AppointmentTotal_Compute(ByRef result As Decimal)
            ' Set result to the desired field value
            result = (ServicesTotal - Discount) - DeductableAmount
        End Sub

        Private Sub Discount_Validate(results As EntityValidationResultsBuilder)
            ' results.AddPropertyError("<Error-Message>")
            If Discount > ServicesTotal Then
                results.AddPropertyError("Discount Amount should be <= Services total amount")
            End If
        End Sub

        Private Sub Balance_Compute(ByRef result As Decimal)
            ' Set result to the desired field value
            result = AppointmentTotal - AppointmentTransactionsCollection.Sum(Function(g) g.TransactionAmount)
        End Sub
    End Class

End Namespace
