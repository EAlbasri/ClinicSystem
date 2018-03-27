
Namespace LightSwitchApplication

    Public Class AccountFiles

        Private Sub Cash_Compute(ByRef result As Decimal)
            ' Set result to the desired field value
            Dim AppTransactions As Double = AppointmentTransactionsCollection.Where(Function(g) g.PaymentType = "Cash").Sum(Function(t) t.TransactionAmount)
            'Dim ExpTransactions As Double = ExpenseTransactionsCollection.Where(Function(g) g.PaymentType = 1).Sum(Function(t) t.Amount)

            result = AppTransactions + Company.InitialCashBalance
        End Sub

        Private Sub Bank_Compute(ByRef result As Decimal)
            ' Set result to the desired field value
            Dim AppTransactions As Double = AppointmentTransactionsCollection.Where(Function(g) Not g.PaymentType = "Cash").Sum(Function(t) t.TransactionAmount)

            result = AppTransactions + Company.InitialBankBalance
        End Sub
    End Class

End Namespace
