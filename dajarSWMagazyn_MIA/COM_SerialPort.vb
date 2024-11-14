Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.Threading
Imports System.IO.Ports
Imports System.IO

Public Class COM_SerialPort
    Public serialPort As SerialPort
    Private strBuffer As String = String.Empty
    Private Delegate Sub UpdateBufferDelegate(ByVal Text As String)

    Public Sub serialPortLoadConfig()
        serialPort = New SerialPort()
        serialPort.PortName = "COM1"
        serialPort.BaudRate = 9600
        serialPort.Parity = Parity.None
        serialPort.DataBits = 8
        serialPort.StopBits = StopBits.One
        serialPort.Handshake = Handshake.None
        ''serialPort.DtrEnable = True
        serialPort.RtsEnable = False
        serialPort.NewLine = vbCrLf
        serialPort.ReadTimeout = 200
        serialPort.ReceivedBytesThreshold = 1

        AddHandler serialPort.DataReceived, AddressOf DataReceivedHandler
    End Sub

    Public Sub SmsClass()
        AddHandler serialPort.DataReceived, AddressOf DataReceivedHandler
    End Sub

    Public Sub DisplaySerialPortInformation(ByVal cmd As String, ByVal cmd_back As String)
        Console.WriteLine("[send]:" & cmd & vbNewLine & "[back]:" & cmd_back & vbNewLine & "[bufor]:" & strBuffer & vbNewLine)
    End Sub

    Public Function getPortStatus() As Boolean
        Return serialPort.IsOpen
    End Function

    Public Function getBuffer() As String
        Return strBuffer.ToString
    End Function

    Public Function sendCommand(ByVal command As String) As String
        Console.WriteLine("sendCommand[{0}]", command)
        Dim cmd As String = False
        Dim cmd_back As Boolean = False
        If serialPort.IsOpen = True Then
            Try
                cmd = command & vbCrLf
                SendData(cmd, cmd_back)
                DisplaySerialPortInformation(cmd, cmd_back)
                Return cmd_back
            Catch ex As Exception
                Console.WriteLine(ex.Message.ToString)
            End Try
        End If
        Return ""
    End Function

    ' Send data without waiting for specific response
    Private Sub SendData(ByVal Data As String)
        serialPort.Write(Data)
    End Sub

    Public Function GetData() As String
        Console.WriteLine("GetData()")
        Dim cmd As String = False
        Dim cmd_back As Boolean = False
        If serialPort.IsOpen = True Then
            SendData(cmd, cmd_back)
            ''DisplaySerialPortInformation(cmd, cmd_back)
            Return strBuffer
        End If
        Return ""
    End Function

    ' Send data and wait for a specific response
    Private Function SendData(ByVal Data As String, ByVal WaitFor As String) As Boolean
        serialPort.Write(Data)
        Thread.Sleep(20)
        serialPort.ReadTimeout = 500
        ''Return WaitForData(WaitFor)
        Return WaitForData(WaitFor)
    End Function

    Private Function WaitForData(ByVal Data As String, Optional ByVal Timeout As Integer = 1) As Boolean
        Dim StartTime As Date = Date.Now
        Do
            If InStr(strBuffer, Data) > 0 Then
                strBuffer = strBuffer.Substring((InStr(strBuffer, Data) - 1) + Data.Length)
                Return (True)
            End If

            If Date.Now.Subtract(StartTime).TotalSeconds >= Timeout Then
                Return (False)
            End If
        Loop
    End Function

    Private Sub UpdateBuffer(ByVal Text As String)
        strBuffer = Text
    End Sub

    Private Sub DataReceivedHandler(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs)
        Dim dUpdateBuffer As New UpdateBufferDelegate(AddressOf UpdateBuffer)
        Dim strTmp As String = serialPort.ReadExisting
        dUpdateBuffer.Invoke(strTmp)
    End Sub

    Public Function Opens() As Boolean
        If serialPort.IsOpen = False Then
            Try
                serialPort.Open()
                serialPort.ReadTimeout = 500
            Catch ex As Exception
                Console.WriteLine(ex.Message.ToString)
                Return False
            End Try
        End If
        Return True
    End Function

    Public Function Closes() As Boolean
        If serialPort.IsOpen = True Then
            Try
                serialPort.DiscardInBuffer()
                serialPort.Close()
            Catch ex As Exception
                Console.WriteLine(ex.Message.ToString)
                Return False
            End Try
        End If
        Return True
    End Function
End Class
