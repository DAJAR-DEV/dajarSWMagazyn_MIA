Partial Public Class admin
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Session("mysuper_user") = Nothing Then
            Session.Abandon()
            Response.Redirect("index.aspx")
        Else
            Session.Remove("contentKomunikat")
        End If
    End Sub

End Class