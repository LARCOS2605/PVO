Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Globalization
Imports System.Data.Entity

Public Class UsersContext
    Inherits DbContext

    Public Sub New()
        MyBase.New("DefaultConnection")
    End Sub

    Public Property UserProfiles As DbSet(Of UserProfile)
End Class

<Table("UserProfile")>
Public Class UserProfile
    <Key()>
    <DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)>
    Public Property UserId As Integer

    Public Property UserName As String
End Class

Public Class RegisterExternalLoginModel
    <Required()>
    <Display(Name:="Nombre de usuario")>
    Public Property UserName As String

    Public Property ExternalLoginData As String
End Class

Public Class LocalPasswordModel
    <Required()>
    <DataType(DataType.Password)>
    <Display(Name:="Contraseña actual")>
    Public Property OldPassword As String

    '<StringLength(100, ErrorMessage:="El número de caracteres de {0} debe ser al menos {2}.", MinimumLength:=6)> _
    <Required()>
    <DataType(DataType.Password)>
    <Display(Name:="Nueva contraseña")>
    Public Property NewPassword As String

    '<Compare("NewPassword", ErrorMessage:="La nueva contraseña y la contraseña de confirmación no coinciden.")> _
    <DataType(DataType.Password)>
    <Display(Name:="Confirmar la nueva contraseña")>
    Public Property ConfirmPassword As String

    Public Property id As String
End Class

Public Class LoginModel
    <Required()>
    <Display(Name:="Nombre de usuario")>
    Public Property UserName As String

    <Required()>
    <DataType(DataType.Password)>
    <Display(Name:="Contraseña")>
    Public Property Password As String

    <Display(Name:="¿Recordar cuenta?")>
    Public Property RememberMe As Boolean
    Public Property customControlAutosizing As Boolean


    Public Property ValidationMessage As String = Nothing
End Class

Public Class RegisterModel
    <Required()>
    <Display(Name:="Nombre de usuario")>
    Public Property UserName As String

    <Required()>
    <StringLength(100, ErrorMessage:="El número de caracteres de {0} debe ser al menos {2}.", MinimumLength:=6)>
    <DataType(DataType.Password)>
    <Display(Name:="Contraseña")>
    Public Property Password As String

    <DataType(DataType.Password)>
    <Display(Name:="Confirmar contraseña")>
    <Compare("Password", ErrorMessage:="La contraseña y la contraseña de confirmación no coinciden.")>
    Public Property ConfirmPassword As String
End Class

Public Class ExternalLogin
    Public Property Provider As String
    Public Property ProviderDisplayName As String
    Public Property ProviderUserId As String
End Class
