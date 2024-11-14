
''<%@ WebHandler Language="VB" Class="PrintPDFHandler" %>

Imports System
Imports System.Web
Imports Neodynamic.SDK.Web
Imports Oracle.ManagedDataAccess.Client
Imports System.Data


Public Class PrintPDFHandler : Implements IHttpHandler

    Dim sqlexp As String = ""

    '############### IMPORTANT!!! ############
    ' If your website requires AUTHENTICATION, then you MUST configure THIS Handler file
    ' to be ANONYMOUS access allowed!!!
    '######################################### 


    Public Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        If WebClientPrint.ProcessPrintJob(context.Request.Url.Query) Then
            WebClientPrint.LicenseOwner = "Dajar Sp. z o.o. - 1 WebApp Lic - 1 WebServer Lic"
            WebClientPrint.LicenseKey = "273D8F1AB12B42847C2E9FE7717B6756A2CA0026"

            Using conn As New OracleConnection(dajarSWMagazyn_MIA.MyFunction.connString)
                conn.Open()

                ''Dim useDefaultPrinter As Boolean = (context.Request("useDefaultPrinter") = "checked")
                Dim useDefaultPrinter As Boolean = True
                ''Dim printerName As String = context.Server.UrlDecode(context.Request("printerName"))
                ''MsgBox("context.Server.UrlDecode" & context.Server.UrlDecode(context.Request("sessionId")))
                ''MsgBox("context.Request(sessionId)" & context.Request("sessionId"))
                Dim session_id As String = context.Server.UrlDecode(context.Request("sessionId"))
                Dim shipment_id As String = HttpContext.Current.Application("shipment_id_" + session_id)
                Dim tracking_number As String = HttpContext.Current.Application("tracking_number_" + session_id)
                Dim firma_id As String = HttpContext.Current.Application("firma_id_" + session_id)
                Dim kod_dyspo As String = HttpContext.Current.Application("kod_dyspo_" + session_id)
                Dim mylogin As String = HttpContext.Current.Application("mylogin_" + session_id)
                Dim myhash As String = HttpContext.Current.Application("myhash_" + session_id)
                Dim remote_ip As String = dajarSWMagazyn_MIA.MyFunction.GetRemoteIp()

                If session_id IsNot Nothing Then
                    ''MsgBox("sessionId=" & SessionId)

                    sqlexp = "select recno from dp_swm_mia_print where remote_ip='" & remote_ip & "' and nr_zamow='" & kod_dyspo & "' and login='" & mylogin & "' and hash='" & myhash & "' and shipment_id='" & shipment_id & "' and session_id='" & session_id & "' and status='N'"
                    Dim print_recno As String = dajarSWMagazyn_MIA.MyFunction.GetStringFromSqlExp(sqlexp, conn)
                    If print_recno <> "" Then
                        dajarSWMagazyn_MIA.MyFunction.UpdatePrintDyspozycja(print_recno, shipment_id, session_id, "Y")
                        'full path of the PDF file to be printed
                        Dim server_link As String = ""

                        If firma_id = "INPOST_ALLEGRO" Then
                            server_link = "\\10.1.0.64\iis\dajarSWMagazyn_MIA\upload\" & firma_id.Replace("INPOST_ALLEGRO", "INPOST") & "\inpost_" & shipment_id.ToString().Replace(".pdf", "") & ".pdf"
                        ElseIf firma_id.Contains("GEIS") Then
                            server_link = "\\10.1.0.64\iis\dajarSWMagazyn_MIA\upload\geis\" & shipment_id.ToString().Replace(".pdf", "") & ".pdf"
                        ElseIf firma_id = "DHL" Or firma_id = "DHL_PS" Then
                            server_link = "\\10.1.0.64\iis\dajarSWMagazyn_MIA\upload\dhl\" & shipment_id.ToString().Replace(".pdf", "") & ".pdf"
                        Else
                            server_link = "\\10.1.0.64\iis\dajarSWMagazyn_MIA\upload\" & firma_id.Replace("_API", "") & "\" & shipment_id.ToString().Replace(".pdf", "") & ".pdf"
                        End If

                        Dim fileName As String = "file-" + Guid.NewGuid().ToString("N")

                        'Create a ClientPrintJob obj that will be processed at the client side by the WCPP
                        Dim cpj As New ClientPrintJob()
                        'let the user to select the printer by a print dialog box
                        cpj.ClientPrinter = New DefaultPrinter()

                        Dim file As New PrintFilePDF(server_link, fileName)
                        file.Sizing = Sizing.Fit

                        cpj.PrintFileGroup.Add(file)
                        'set the commands to send to the printer
                        cpj.PrintFile = file

                        context.Response.ContentType = "application/octet-stream"
                        context.Response.BinaryWrite(cpj.GetContent())
                        context.Response.End()

                    End If
                End If
                conn.Close()
            End Using
        End If

    End Sub


    Public ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
