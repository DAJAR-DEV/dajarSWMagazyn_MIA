Imports System.Management

Public Class remoteApp
    Public Shared domainUserName As String = ""
    Public Shared domainName As String = ""
    Public Shared domainIpAdress As String = ""

    Public Sub New()
        LoadProcessDetails()
    End Sub

    Public Shared Function GetIPAddress() As String
        Dim strHostName As String = System.Net.Dns.GetHostName()
        Dim iphe As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(strHostName)
        Dim strIpAdress As String = ""
        For Each ipheal As System.Net.IPAddress In iphe.AddressList
            If ipheal.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
                strIpAdress = ipheal.ToString()
            End If
        Next

        Return strIpAdress
    End Function

    Public Shared Sub LoadProcessDetails()
        Dim currentProcess As Process = Process.GetCurrentProcess()
        Dim procId As String = currentProcess.Id
        GetProcessOwner(procId)
    End Sub

    Public Shared Sub GetProcessOwner(ByVal processId As Integer)
        Dim query As String = "Select * From Win32_Process Where ProcessID = " & processId
        Dim searcher As New ManagementObjectSearcher(query)
        Dim processList As ManagementObjectCollection = searcher.[Get]()

        For Each obj As ManagementObject In processList
            Dim argList As String() = New String() {String.Empty, String.Empty}
            Dim returnVal As Integer = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList))
            If returnVal = 0 Then
                domainUserName = argList(0).ToString
                domainName = argList(1).ToString
                domainIpAdress = GetIPAddress()
            End If
        Next
    End Sub
End Class
