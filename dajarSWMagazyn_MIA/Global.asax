<%@ Application Language="VB"%>
<%@ Import Namespace="Oracle.ManagedDataAccess.Client" %>

<script type="text/VB" runat="server">

Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
    OracleConfiguration.SqlNetAllowedLogonVersionClient = OracleAllowedLogonVersionClient.Version11
End Sub

</script>