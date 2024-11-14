Imports Oracle.ManagedDataAccess.Client

Partial Public Class index
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("contentKomunikat") = ""
    End Sub
End Class