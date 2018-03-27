
Namespace LightSwitchApplication

    Public Class ApplicationDataService

        Private Sub Appointments_PreprocessQuery(FromDate As System.Nullable(Of Date), ToDate As System.Nullable(Of Date), ByRef query As System.Linq.IQueryable(Of LightSwitchApplication.AppointmentFiles))
            query = From g In query Where CDate(g.AppointmentDateTime).Date.CompareTo(CDate(ToDate).Date) = 0 Select g
        End Sub
    End Class

End Namespace
