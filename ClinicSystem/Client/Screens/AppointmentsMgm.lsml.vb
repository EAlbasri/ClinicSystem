
Namespace LightSwitchApplication

    Public Class AppointmentsMgm

        Private AppointmentDialogHelper As DialogHelper
        Private AppointmentServicesDialogHelper As DialogHelper
        Private PaymentDialogHelper As DialogHelper
        Private PrescriptionDialogHelper As DialogHelper

        Private Sub AppointmentsMgm_InitializeDataWorkspace(saveChangesTo As List(Of IDataService))
            AppointmentDialogHelper = New DialogHelper(Me.AppointmentsCollection, "AppointmentAddEdit")
            AppointmentServicesDialogHelper = New DialogHelper(Me.AppointmentServicesCollection, "AppointmentServiceAddEdit")
            PaymentDialogHelper = New DialogHelper(Me.AppointmentTransactionsCollection, "TransactionAdd")
            PrescriptionDialogHelper = New DialogHelper(Me.PrescriptionsCollection, "PrescriptionAddEdit")

            AddHandler Me.FindControl(PatientTab_CONTROL).ControlAvailable, AddressOf PatientTabItems_ControlAvailable
            AddHandler Me.FindControl(MainTab_CONTROL).ControlAvailable, AddressOf MainTabItems_ControlAvailable

            Me.FindControl("InsuranceCompanyPrp").IsReadOnly = True
            PrescriptionsControlls(False)
        End Sub

        Private Sub AppointmentsMgm_Created()
            AppointmentDialogHelper.InitializeUI()
            AppointmentServicesDialogHelper.InitializeUI()
            PaymentDialogHelper.InitializeUI()
        End Sub

#Region "Appointment Add & Edit"

        Private Sub AddAppointment_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(DoctorsList) Then
                If Not IsNothing(DoctorsList.SelectedItem) Then
                    result = AppointmentDialogHelper.CanAdd
                Else
                    result = False
                End If
            Else
                result = False
            End If
        End Sub

        Private Sub AddAppointment_Execute()
            AppointmentDialogHelper.AddEntity()
            AppointmentsCollection.SelectedItem.Doctor = DoctorsList.SelectedItem
            DiscountPrp = 0
            InsuranceControls(False)
            Me.FindControl("UseInsurancePrp").IsEnabled = False
        End Sub

        Private Sub AppointmentOk_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(AppointmentsCollection) Then
                If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                    If Not IsNothing(AppointmentsCollection.SelectedItem.Doctor) And Not IsNothing(AppointmentsCollection.SelectedItem.Patient) And AppointmentServicesCollection.Count > 0 Then
                        result = Not AppointmentsCollection.SelectedItem.Details.ValidationResults.HasErrors
                        Exit Sub
                    End If
                End If
            End If

            result = False
        End Sub

        Private Sub AppointmentOk_Execute()
            ' Write your code here.
            AppointmentDialogHelper.DialogOk()
            AppointmentsCollection.SelectedItem.DeductableAmount = DeductableAmountPrp
            AppointmentsCollection.SelectedItem.CoPayment = CoPaymentPrp
            Save()
        End Sub

        Private Sub AppointmentCancel_Execute()
            ' Write your code here.
            AppointmentDialogHelper.DialogCancel()
        End Sub

        Private Sub EditAppointment_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(AppointmentsCollection) Then
                If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                    result = AppointmentDialogHelper.CanEditSelected
                Else
                    result = False
                End If
            Else
                result = False
            End If
        End Sub

        Private Sub EditAppointment_Execute()
            ' Write your code here.
            AppointmentDialogHelper.EditSelectedEntity()

        End Sub

        Private Sub CancelAppointment_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(AppointmentsCollection) Then
                If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                    result = True
                Else
                    result = False
                End If
            Else
                result = False
            End If
        End Sub

        Private Sub CancelAppointment_Execute()
            ' Write your code here.
            AppointmentsCollection.SelectedItem.Delete()
            Save()
        End Sub

#End Region

#Region "Appointment Services Add & Edit"

        Private Sub AppointmentServicesCollectionAddAndEditNew_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(AppointmentsCollection) Then
                If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                    result = AppointmentServicesDialogHelper.CanAdd
                Else
                    result = False
                End If
            Else
                result = False
            End If
        End Sub

        Private Sub AppointmentServicesCollectionAddAndEditNew_Execute()
            ' Write your code here.
            AppointmentServicesDialogHelper.AddEntity()
            ServicePrp = Nothing
        End Sub

        Private Sub AppointmentServicesCollectionEditSelected_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(AppointmentsCollection) Then
                If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                    If Not IsNothing(AppointmentServicesCollection) Then
                        If Not IsNothing(AppointmentServicesCollection.SelectedItem) Then
                            result = AppointmentServicesDialogHelper.CanEditSelected
                        Else
                            result = False
                        End If
                    Else
                        result = False
                    End If
                Else
                    result = False
                End If
            Else
                result = False
            End If
        End Sub

        Private Sub AppointmentServicesCollectionEditSelected_Execute()
            ' Write your code here.
            AppointmentServicesDialogHelper.EditSelectedEntity()
            ServicePrp = AppointmentServicesCollection.SelectedItem.Service
        End Sub

        Private Sub AppointmentServiceCancel_Execute()
            ' Write your code here.
            AppointmentServicesDialogHelper.DialogCancel()
        End Sub

        Private Sub AppointmentServiceOk_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(AppointmentServicesCollection.SelectedItem) And Not IsNothing(ServicePrp) Then
                result = Not AppointmentServicesCollection.SelectedItem.Details.ValidationResults.HasErrors
            Else
                result = False
            End If
        End Sub

        Private Sub AppointmentServiceOk_Execute()
            ' Write your code here.
            AppointmentServicesDialogHelper.DialogOk()
            If Not IsNothing(ServicePrp.DefaultNote) Then
                If Not IsNothing(AppointmentsCollection.SelectedItem.Note) Then
                    AppointmentsCollection.SelectedItem.Note += ", " & ServicePrp.DefaultNote
                Else
                    AppointmentsCollection.SelectedItem.Note += ServicePrp.DefaultNote
                End If
            End If

            CoPaymentDiscountCalculations()
        End Sub

        Private Sub ServicePrp_Changed()
            If Not IsNothing(ServicePrp) Then
                If Not IsNothing(AppointmentsCollection) Then
                    If Not IsNothing(AppointmentServicesCollection) Then
                        If Not IsNothing(AppointmentServicesCollection.SelectedItem) Then
                            AppointmentServicesCollection.SelectedItem.Service = ServicePrp
                            If AppointmentsCollection.SelectedItem.UseInsurance = True Then
                                Dim PolicyServices = (SelectedPatient.SelectedItem.PatientInsurance.InsurancePolicy.InsurancePolicyServicesCollection).ToList
                                Dim SelectedService = (From f In PolicyServices Where f.Service.Id = ServicePrp.Id).FirstOrDefault
                                If Not IsNothing(SelectedService) Then
                                    AppointmentServicesCollection.SelectedItem.Fees = SelectedService.Amount
                                Else
                                    AppointmentServicesCollection.SelectedItem.Fees = ServicePrp.DefaultFees
                                End If
                            Else
                                AppointmentServicesCollection.SelectedItem.Fees = ServicePrp.DefaultFees
                            End If
                        End If
                    End If
                End If
            End If
        End Sub

#End Region

#Region "Appointment Transaction Add"

        Private Sub AppointmentPayment_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(DoctorsList) Then
                If Not IsNothing(DoctorsList.SelectedItem) Then
                    If Not IsNothing(AppointmentsCollection) Then
                        If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                            If AppointmentsCollection.SelectedItem.Balance > 0 Then
                                result = True
                                Exit Sub
                            End If
                        End If
                    End If
                End If
            End If
            result = False
        End Sub

        Private Sub AppointmentPayment_Execute()
            PaymentDialogHelper.AddEntity()
            AppointmentTransactionsCollection.SelectedItem.TransactionDate = Today.Date
            AppointmentTransactionsCollection.SelectedItem.Account = AccountFilesSet.FirstOrDefault
            AppointmentTransactionsCollection.SelectedItem.PaymentType = "Cash"

        End Sub

        Private Sub AppointmentTransactionCancel_Execute()
            ' Write your code here.
            PaymentDialogHelper.DialogCancel()
        End Sub

        Private Sub AppointmentTransactionOk_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(AppointmentsCollection) Then
                If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                    If Not IsNothing(AppointmentTransactionsCollection) Then
                        If Not IsNothing(AppointmentTransactionsCollection.SelectedItem) Then
                            If AppointmentTransactionsCollection.SelectedItem.TransactionAmount > 0 And Not IsNothing(AppointmentTransactionsCollection.SelectedItem.PaymentType) And AppointmentsCollection.SelectedItem.Balance >= 0 Then
                                result = Not AppointmentTransactionsCollection.SelectedItem.Details.ValidationResults.HasErrors
                                Exit Sub
                            End If
                        End If
                    End If
                End If
            End If
            result = False
        End Sub

        Private Sub AppointmentTransactionOk_Execute()
            AppointmentTransactionsCollection.SelectedItem.Account = AccountFilesSet.FirstOrDefault
            PaymentDialogHelper.DialogOk()
            Save()
        End Sub

#End Region

#Region "Prescription Add & Edit"

        Private Sub AddPrescription_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                If Not IsNothing(SelectedPatient.SelectedItem) Then
                    result = PrescriptionDialogHelper.CanAdd
                Else
                    result = False
                End If
            Else
                result = False
            End If
        End Sub

        Private Sub AddPrescription_Execute()
            ' Write your code here.
            PrescriptionDialogHelper.AddEntity()
        End Sub

        Private Sub PrescriptionCancel_Execute()
            ' Write your code here.
            PrescriptionDialogHelper.DialogCancel()
        End Sub

        Private Sub PrescriptionOk_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(PrescriptionsCollection) Then
                If Not IsNothing(PrescriptionsCollection.SelectedItem) And PrescriptionMedicinesCollection.Count > 0 Then
                    If Not IsNothing(PrescriptionsCollection.SelectedItem.Doctor) And Not IsNothing(PrescriptionsCollection.SelectedItem.Patient) Then
                        result = Not PrescriptionsCollection.SelectedItem.Details.ValidationResults.HasErrors
                    Else
                        result = False
                    End If
                Else
                    result = False
                End If
            Else
                result = False
            End If
        End Sub

        Private Sub PrescriptionOk_Execute()
            ' Write your code here.
            PrescriptionDialogHelper.DialogOk()
            Save()
        End Sub

        Private Sub EditPrescription_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(AppointmentsCollection.SelectedItem) Then

                If Not IsNothing(SelectedPatient) Then
                    If Not IsNothing(SelectedPatient) And Not IsNothing(PrescriptionsCollection) Then
                        If Not IsNothing(PrescriptionsCollection.SelectedItem) Then
                            result = PrescriptionDialogHelper.CanEditSelected
                        Else
                            result = False
                        End If
                    Else
                        result = False
                    End If
                Else
                    result = False
                End If
            Else
                result = False
            End If
        End Sub

        Private Sub EditPrescription_Execute()
            ' Write your code here.
            PrescriptionDialogHelper.EditSelectedEntity()
        End Sub

        Private Sub RemovePrescription_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(SelectedPatient) Then
                If Not IsNothing(SelectedPatient) And Not IsNothing(PrescriptionsCollection) Then
                    If Not IsNothing(PrescriptionsCollection.SelectedItem) Then
                        result = True
                    Else
                        result = False
                    End If
                Else
                    result = False
                End If
            Else
                result = False
            End If
        End Sub

        Private Sub RemovePrescription_Execute()
            ' Write your code here.
            PrescriptionsCollection.SelectedItem.Delete()
            Save()
        End Sub

#End Region

#Region "Tab Controls"
        'TabControl 
        Private Const PatientTab_CONTROL As String = "PatientTabControl"
        Private Const MainTab_CONTROL As String = "MainTabControl"

        'this is somewhere to store a reference to the grid control 
        Private WithEvents _PatientTabControl As TabControl = Nothing
        Private WithEvents _MainTabControl As TabControl = Nothing

        Private Sub PatientTabItems_ControlAvailable(send As Object, e As ControlAvailableEventArgs)
            'we know that the control is a grid, but we use TryCast, just in case 
            _PatientTabControl = TryCast(e.Control, TabControl)
            _PatientTabControl.SelectedIndex = 1
            _PatientTabControl.SelectedIndex = 0
            'if the cast failed, just leave, there's nothing more we can do here 
            If (_PatientTabControl Is Nothing) Then Return
        End Sub

        Private Sub MainTabItems_ControlAvailable(send As Object, e As ControlAvailableEventArgs)
            'we know that the control is a grid, but we use TryCast, just in case 
            _MainTabControl = TryCast(e.Control, TabControl)
            _MainTabControl.SelectedIndex = 1
            _MainTabControl.SelectedIndex = 0
            'if the cast failed, just leave, there's nothing more we can do here 
            If (_MainTabControl Is Nothing) Then Return
        End Sub

        Private Sub _PatientTabControl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles _PatientTabControl.SelectionChanged

            If _PatientTabControl.SelectedIndex = 4 Then
                PrescriptionsControlls(True)
                AppointmentsControlls(False)
            Else
                PrescriptionsControlls(False)
            End If

        End Sub

        Private Sub _MainTabControl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles _MainTabControl.SelectionChanged

            If _MainTabControl.SelectedIndex = 0 Then
                AppointmentsControlls(True)
                PrescriptionsControlls(False)
            Else
                AppointmentsControlls(False)
            End If

        End Sub

        Private Sub PrescriptionsControlls(ByVal Status As Boolean)
            Me.FindControl("AddPrescription").IsVisible = Status
            Me.FindControl("EditPrescription").IsVisible = Status
            Me.FindControl("RemovePrescription").IsVisible = Status
        End Sub

        Private Sub AppointmentsControlls(ByVal Status As Boolean)
            Me.FindControl("AddAppointment").IsVisible = Status
            Me.FindControl("EditAppointment").IsVisible = Status
            Me.FindControl("CancelAppointment").IsVisible = Status
            Me.FindControl("AppointmentPayment").IsVisible = Status
        End Sub

#End Region

#Region "Associate Prp With Appointment Variables"

        Dim CoPayment, Discount As Double

        Private Sub SelectedPatient_Changed(e As Collections.Specialized.NotifyCollectionChangedEventArgs)
            If Not IsNothing(SelectedPatient) Then
                If Not IsNothing(SelectedPatient.SelectedItem) Then
                    If AppointmentsCollection.SelectedItem.Id = 0 Then
                        If Not IsNothing(SelectedPatient.SelectedItem.PatientInsurance) Then
                            Me.FindControl("UseInsurancePrp").IsEnabled = True
                            UseInsurancePrp = True
                            InsuranceCompanyPrp = SelectedPatient.SelectedItem.PatientInsurance.InsurancePolicy.InsuranceCompany
                            DeductableAmountPrp = SelectedPatient.SelectedItem.PatientInsurance.InsurancePolicy.DeductableAmount
                            CoPayment = SelectedPatient.SelectedItem.PatientInsurance.InsurancePolicy.CoPayment
                            Discount = SelectedPatient.SelectedItem.PatientInsurance.InsurancePolicy.Discount
                            CoPaymentDiscountCalculations()
                        Else
                            Me.FindControl("UseInsurancePrp").IsEnabled = False
                            InsuranceCompanyPrp = Nothing
                            DeductableAmountPrp = 0
                            CoPaymentPrp = 0
                            DiscountPrp = 0
                            UseInsurancePrp = False
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub CoPaymentDiscountCalculations()
            CoPaymentPrp = (AppointmentsCollection.SelectedItem.ServicesTotal * CoPayment)
            DiscountPrp = (AppointmentsCollection.SelectedItem.ServicesTotal * Discount)
        End Sub

        Private Sub DiscountPrp_Changed()
            If Not IsNothing(AppointmentsCollection) Then
                If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                    AppointmentsCollection.SelectedItem.Discount = DiscountPrp
                End If
            End If
        End Sub

        Private Sub DeductableAmountPrp_Changed()
            If Not IsNothing(AppointmentsCollection) Then
                If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                    AppointmentsCollection.SelectedItem.DeductableAmount = DeductableAmountPrp
                End If
            End If
        End Sub

        Private Sub CoPaymentPrp_Changed()
            If Not IsNothing(AppointmentsCollection) Then
                If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                    AppointmentsCollection.SelectedItem.CoPayment = CoPaymentPrp
                End If
            End If
        End Sub

        Private Sub InsuranceCompanyPrp_Changed()
            If Not IsNothing(AppointmentsCollection) Then
                If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                    AppointmentsCollection.SelectedItem.InsuranceCompany = InsuranceCompanyPrp
                End If
            End If
        End Sub

        Private Sub UseInsurancePrp_Changed()
            If UseInsurancePrp = True Then
                InsuranceControls(True)
                If Not IsNothing(AppointmentsCollection) Then
                    If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                        AppointmentsCollection.SelectedItem.UseInsurance = True
                        CoPaymentDiscountCalculations()
                    End If
                End If
            Else
                InsuranceControls(False)
                DiscountPrp = 0
                If Not IsNothing(AppointmentsCollection) Then
                    If Not IsNothing(AppointmentsCollection.SelectedItem) Then
                        AppointmentsCollection.SelectedItem.UseInsurance = False
                        DeductableAmountPrp = 0
                        CoPaymentPrp = 0
                        DiscountPrp = 0
                    End If
                End If

            End If
        End Sub

        Private Sub InsuranceControls(ByVal Status As Boolean)
            Me.FindControl("CoPaymentPrp").IsEnabled = Status
            Me.FindControl("DeductableAmountPrp").IsEnabled = Status
        End Sub

#End Region

        Private Sub ToDatePrp_Changed()

        End Sub
    End Class

End Namespace
