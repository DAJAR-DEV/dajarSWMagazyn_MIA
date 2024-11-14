Imports System
Imports System.Net.Mail
Imports System.Net

Public Class SendMail

    Shared Sub SendEmailMessage(ByVal strFrom As String, ByVal strTo() _
    As String, ByVal strSubject _
    As String, ByVal strMessage _
    As String, ByVal fileList() As String, ByVal user As String, ByVal passwd As String)

        Try
            For Each item As String In strTo

                Dim MailMsg As New MailMessage(New MailAddress(strFrom.Trim()), New MailAddress(item))
                MailMsg.BodyEncoding = Text.Encoding.GetEncoding(1250)

                MailMsg.Subject = strSubject.Trim()
                MailMsg.Body = strMessage.Trim() & vbCrLf
                MailMsg.Priority = MailPriority.Normal
                MailMsg.IsBodyHtml = True

                For Each strfile As String In fileList
                    If Not strfile = "" Then
                        Dim MsgAttach As New Attachment(strfile)
                        MailMsg.Attachments.Add(MsgAttach)
                    End If
                Next

                Try
                    Dim SmtpMail As New SmtpClient
                    SmtpMail.Timeout = 2000000
                    SmtpMail.Port = 25
                    SmtpMail.Host = "smtp.dajar.pl"
                    SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network
                    SmtpMail.Credentials = New NetworkCredential(user, passwd)
                    SmtpMail.Send(MailMsg)
                Catch e As System.Net.Mail.SmtpException
                End Try

            Next
        Catch ex As Exception
        End Try
    End Sub

    Shared Sub SendEmailMessageNew(ByVal strFrom As String, ByVal strTo As List(Of String), ByVal strSubject As String, ByVal strMessage As String, _
    ByVal fileList As List(Of String), ByVal user As String, ByVal passwd As String)

        Try
            For Each item As String In strTo

                Dim MailMsg As New MailMessage(New MailAddress(strFrom.Trim()), New MailAddress(item))
                MailMsg.BodyEncoding = Text.Encoding.GetEncoding(1250)

                MailMsg.Subject = strSubject.Trim()
                MailMsg.Body = strMessage.Trim() & vbCrLf
                MailMsg.Priority = MailPriority.Normal
                MailMsg.IsBodyHtml = True

                For Each strfile In fileList
                    If Not strfile = "" Then
                        Dim MsgAttach As New Attachment(strfile)
                        MsgAttach.Name = MsgAttach.Name.Replace("upload/", "").Replace("zwroty/", "").Replace("reklamacje/", "")
                        MailMsg.Attachments.Add(MsgAttach)
                    End If
                Next

                Try
                    Dim SmtpMail As New SmtpClient
                    SmtpMail.Timeout = 2000000
                    SmtpMail.Port = 25
                    SmtpMail.Host = "smtp.dajar.pl"
                    SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network
                    SmtpMail.Credentials = New NetworkCredential(user, passwd)
                    SmtpMail.Send(MailMsg)
                Catch e As System.Net.Mail.SmtpException
                End Try

            Next
        Catch ex As Exception
        End Try
    End Sub

    Shared Sub SendEmailMessageMulti(ByVal strFrom As String, ByVal strTo As List(Of String), ByVal strSubject As String, ByVal strMessage As String, _
    ByVal fileList As List(Of String), ByVal user As String, ByVal passwd As String)

        Try

            Dim emailTo As String = ""
            For Each item As String In strTo
                emailTo &= item & ","
            Next
            emailTo = emailTo.Substring(0, emailTo.Length - 1)

            Dim MailMsg As New MailMessage
            MailMsg.From = New MailAddress(strFrom.Trim())
            MailMsg.To.Add(emailTo)
            MailMsg.CC.Add("admin.dswm@dajar.pl")
            MailMsg.BodyEncoding = Text.Encoding.GetEncoding(1250)
            MailMsg.Subject = strSubject.Trim()
            MailMsg.Body = strMessage.Trim() & vbCrLf
            MailMsg.Priority = MailPriority.Normal
            MailMsg.IsBodyHtml = True

            ''For Each strfile In fileList
            ''    If Not strfile = "" Then
            ''        Dim MsgAttach As New Attachment(strfile)
            ''        MsgAttach.Name = MsgAttach.Name.Replace("upload/", "").Replace("zwroty/", "").Replace("reklamacje/", "")
            ''        MailMsg.Attachments.Add(MsgAttach)
            ''    End If
            ''Next

            Try
                Dim SmtpMail As New SmtpClient
                SmtpMail.Timeout = 2000000
                SmtpMail.Port = 25
                SmtpMail.Host = "smtp.dajar.pl"
                SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network
                SmtpMail.Credentials = New NetworkCredential(user, passwd)
                SmtpMail.Send(MailMsg)
            Catch e As System.Net.Mail.SmtpException
            End Try

        Catch ex As Exception
        End Try
    End Sub


End Class
