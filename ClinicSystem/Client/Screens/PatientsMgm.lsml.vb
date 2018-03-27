
Namespace LightSwitchApplication

    Public Class PatientsMgm
        Private PrescriptionDialogHelper As DialogHelper
        Private PaymentDialogHelper As DialogHelper
        Private AppointmentDialogHelper As DialogHelper
        Private AppointmentServicesDialogHelper As DialogHelper
        Private PatientDialogHelper As DialogHelper

        Private Sub PatientsMgm_InitializeDataWorkspace(saveChangesTo As List(Of IDataService))
            PrescriptionDialogHelper = New DialogHelper(Me.PrescriptionsCollection, "PrescriptionAddEdit")
            PaymentDialogHelper = New DialogHelper(Me.AppointmentTransactionsCollection, "TransactionAdd")
            AppointmentDialogHelper = New DialogHelper(Me.AppointmentsCollection, "AppointmentAddEdit")
            AppointmentServicesDialogHelper = New DialogHelper(Me.AppointmentServicesCollection, "AppointmentServiceAddEdit")
            PatientDialogHelper = New DialogHelper(Me.PatientFilesSet, "PatientAddEdit")

            InsuranceControls(False)

            AddHandler Me.FindControl(Tab_CONTROL).ControlAvailable, AddressOf TabItems_ControlAvailable
        End Sub

        Private Sub PatientsMgm_Created()
            PrescriptionDialogHelper.InitializeUI()
            PaymentDialogHelper.InitializeUI()
            AppointmentDialogHelper.InitializeUI()
            AppointmentServicesDialogHelper.InitializeUI()
            PatientDialogHelper.InitializeUI()
        End Sub

#Region "Prescription Add & Edit"

        Private Sub AddPrescription_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(PatientFilesSet) Then
                If Not IsNothing(PatientFilesSet.SelectedItem) Then
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
            If Not IsNothing(PatientList) Then
                If Not IsNothing(PatientList.SelectedItem) And Not IsNothing(PrescriptionsCollection) Then
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
        End Sub

        Private Sub EditPrescription_Execute()
            ' Write your code here.
            PrescriptionDialogHelper.EditSelectedEntity()
        End Sub

        Private Sub RemovePrescription_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(PatientList) Then
                If Not IsNothing(PatientList.SelectedItem) And Not IsNothing(PrescriptionsCollection) Then
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
                            If AppointmentTransactionsCollection.SelectedItem.TransactionAmount > 0 And Not IsNothing(AppointmentTransactionsCollection.SelectedItem.PaymentType) Then
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

#Region "Tab Control"
        'TabControl 
        Private Const Tab_CONTROL As String = "PatientTabControl"
        'this is somewhere to store a reference to the grid control 
        Private WithEvents _TabControl As TabControl = Nothing

        Private Sub TabItems_ControlAvailable(send As Object, e As ControlAvailableEventArgs)
            'we know that the control is a grid, but we use TryCast, just in case 
            _TabControl = TryCast(e.Control, TabControl)
            _TabControl.SelectedIndex = 1
            _TabControl.SelectedIndex = 0
            'if the cast failed, just leave, there's nothing more we can do here 
            If (_TabControl Is Nothing) Then Return
        End Sub

        Private Sub _TabControl_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles _TabControl.SelectionChanged

            If _TabControl.SelectedIndex = 4 Then
                PrescriptionControls(True)
            Else
                PrescriptionControls(False)
            End If

            If _TabControl.SelectedIndex = 1 Then
                AppointmentsControls(True)
            Else
                AppointmentsControls(False)

            End If
        End Sub

        Private Sub AppointmentsControls(ByVal Status As Boolean)
            Me.FindControl("AddAppointment").IsVisible = Status
            Me.FindControl("EditAppointment").IsVisible = Status
            Me.FindControl("CancelAppointment").IsVisible = Status
            Me.FindControl("AppointmentPayment").IsVisible = Status
            Me.FindControl("AppointmentPayment").IsVisible = Status
        End Sub

        Private Sub PrescriptionControls(ByVal Status As Boolean)
            Me.FindControl("AddPrescription").IsVisible = Status
            Me.FindControl("EditPrescription").IsVisible = Status
            Me.FindControl("RemovePrescription").IsVisible = Status
        End Sub

#End Region

#Region "Appointment Add Edit"

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

#Region "Appointment Services Add Edit"

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
        End Sub

        Private Sub ServicePrp_Changed()
            If Not IsNothing(ServicePrp) Then
                If Not IsNothing(AppointmentsCollection) Then
                    If Not IsNothing(AppointmentServicesCollection) Then
                        If Not IsNothing(AppointmentServicesCollection.SelectedItem) Then
                            AppointmentServicesCollection.SelectedItem.Service = ServicePrp
                            AppointmentServicesCollection.SelectedItem.Fees = ServicePrp.DefaultFees
                        End If
                    End If
                End If
            End If
        End Sub

#End Region

#Region "Patient Add Edit"

        Private Sub PatientCancel_Execute()
            ' Write your code here.
            PatientDialogHelper.DialogCancel()
        End Sub

        Private Sub PatientFilesListAddAndEditNew_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            result = PatientDialogHelper.CanAdd
        End Sub

        Private Sub PatientFilesListAddAndEditNew_Execute()
            ' Write your code here.
            PatientDialogHelper.AddEntity()
        End Sub

        Private Sub PatientOk_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(PatientFilesSet) Then
                If Not IsNothing(PatientFilesSet.SelectedItem) Then
                    If Not IsNothing(PatientFilesSet.SelectedItem.FirstName) And Not IsNothing(PatientFilesSet.SelectedItem.LastName) Then
                        If Not IsNothing(InsuranceCompaniesPrp) Then
                            If Not IsNothing(InsurancePolicyPrp) Then
                                If Not IsNothing(PolicyNumberPrp) Then
                                    result = PatientDialogHelper.CanAdd
                                Else
                                    result = False
                                End If
                            Else
                                result = False
                            End If
                        Else
                            result = PatientDialogHelper.CanAdd
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

        Private Sub PatientOk_Execute()
            ' Write your code here.
            If Not IsNothing(InsuranceCompaniesPrp) Then
                PatientFilesSet.SelectedItem.PatientInsurance = New PatientInsuranceFiles
                PatientFilesSet.SelectedItem.PatientInsurance.InsurancePolicy = InsurancePolicyPrp
                PatientFilesSet.SelectedItem.PatientInsurance.ExpiryDate = ExpiryDatePrp
                PatientFilesSet.SelectedItem.PatientInsurance.PolicyNumber = PolicyNumberPrp
            Else
                If Not IsNothing(PatientFilesSet.SelectedItem.PatientInsurance) Then
                    PatientFilesSet.SelectedItem.PatientInsurance.Delete()
                End If
            End If

            PatientDialogHelper.DialogOk()

            Save()
        End Sub

        Private Sub InsuranceCompaniesPrp_Changed()
            If IsNothing(InsuranceCompaniesPrp) Then
                InsuranceControls(False)
            Else
                InsuranceControls(True)
            End If
        End Sub

        Private Sub InsuranceControls(ByVal Status As Boolean)
            Me.FindControl("PolicyNumberPrp").IsVisible = Status
            Me.FindControl("ExpiryDatePrp").IsVisible = Status
            Me.FindControl("InsurancePolicyPrp").IsVisible = Status
        End Sub

        Private Sub PatientFilesListEditSelected_CanExecute(ByRef result As Boolean)
            ' Write your code here.
            If Not IsNothing(PatientFilesSet) Then
                If Not IsNothing(PatientFilesSet.SelectedItem) Then
                    result = PatientDialogHelper.CanEditSelected
                    Exit Sub
                End If
            End If

            result = False
        End Sub

        Private Sub PatientFilesListEditSelected_Execute()
            ' Write your code here.
            PatientDialogHelper.EditSelectedEntity()
            If Not IsNothing(PatientFilesSet.SelectedItem.PatientInsurance) Then
                InsuranceCompaniesPrp = PatientFilesSet.SelectedItem.PatientInsurance.InsurancePolicy.InsuranceCompany
                ExpiryDatePrp = PatientFilesSet.SelectedItem.PatientInsurance.ExpiryDate
                InsurancePolicyPrp = PatientFilesSet.SelectedItem.PatientInsurance.InsurancePolicy
                PolicyNumberPrp = PatientFilesSet.SelectedItem.PatientInsurance.PolicyNumber
            Else
                InsuranceCompaniesPrp = Nothing
                PolicyNumberPrp = 0
            End If
            'PatientFilesSet.SelectedItem.PatientInsuranceCollection.
        End Sub

#End Region

    End Class

End Namespace
