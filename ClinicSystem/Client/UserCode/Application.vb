Imports Microsoft.LightSwitch.Threading


Namespace LightSwitchApplication

    Public Class Application

    End Class

    Public Class DialogHelper
        Private _screen As Microsoft.LightSwitch.Client.IScreenObject
        Private _collection As Microsoft.LightSwitch.Client.IVisualCollection
        Private _dialogName As String

        Private _isEditing As Boolean = False
        Private _entity As IEntityObject

        Public Sub New(ByVal visualCollection As Microsoft.LightSwitch.Client.IVisualCollection,
                            ByVal dialogName As String)
            _screen = visualCollection.Screen
            _collection = visualCollection
            _dialogName = dialogName
        End Sub

        Public Sub InitializeUI()
            'This code may not work in Beta 2.  Fixed in final release.  

            AddHandler _screen.FindControl(_dialogName).ControlAvailable,
                Sub(sender As Object, e As ControlAvailableEventArgs)
                    Dim childWindow As System.Windows.Controls.ChildWindow = e.Control
                    childWindow.HasCloseButton = False
                    AddHandler childWindow.Closed,
                               Sub(s1 As Object, e1 As EventArgs)
                                   If _entity IsNot Nothing Then
                                       DirectCast(_entity.Details, IEditableObject).CancelEdit()
                                   End If
                               End Sub
                End Sub
        End Sub

        Public Function CanEditSelected() As Boolean
            Return _collection.CanEdit() AndAlso (Not _collection.SelectedItem Is Nothing)
        End Function

        Public Function CanAdd() As Boolean
            Return _collection.CanAddNew()
        End Function
        Public Sub AddEntity()
            _isEditing = False
            _collection.AddNew()
            _screen.FindControl(_dialogName).DisplayName = "Add " +
                 _collection.Details.GetModel.ElementType.Name
            BaseOpenDialog()
        End Sub

        Public Sub EditSelectedEntity()
            _isEditing = True
            _screen.FindControl(_dialogName).DisplayName = "Edit " +
                 _collection.Details.GetModel.ElementType.Name
            BaseOpenDialog()
        End Sub

        Private Sub BaseOpenDialog()
            _entity = _collection.SelectedItem()
            If _entity IsNot Nothing Then
                Dispatchers.Main.Invoke(Sub()
                                            DirectCast(_entity.Details, IEditableObject).EndEdit()
                                            DirectCast(_entity.Details, IEditableObject).BeginEdit()
                                        End Sub)
                _screen.OpenModalWindow(_dialogName)
            End If
        End Sub

        Public Sub DialogOk()
            If _entity IsNot Nothing Then
                Dispatchers.Main.Invoke(Sub()
                                            DirectCast(_entity.Details, IEditableObject).EndEdit()
                                        End Sub)
                _screen.CloseModalWindow(_dialogName)
            End If
        End Sub

        Public Sub DialogCancel()
            If _entity IsNot Nothing Then
                Dispatchers.Main.Invoke(Sub()
                                            DirectCast(_entity.Details, IEditableObject).CancelEdit()
                                        End Sub)
                If _isEditing = False Then
                    _entity.Delete()
                End If
                _screen.CloseModalWindow(_dialogName)
            End If
        End Sub
    End Class

End Namespace
