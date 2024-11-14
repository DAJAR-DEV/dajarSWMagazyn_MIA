<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="index.aspx.vb" Inherits="dajarSWMagazyn_MIA.index" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>dajarSystemWspomaganiaMagazynu by MM</title>
    <meta name="format-detection" content="telephone=no"/>
    <link rel="apple-touch-icon" href="favicon.ico"/>
    <meta name="viewport" content="initial-scale=1.0, maximum-scale=5.0"/>
    <link rel="stylesheet" type="text/css" href="style.css"/>
</head>
<body>
<form id="form2" runat="server">
    <div class="wrapper">
        <div class="index-container">
            <hr>
            <img src="Dajar_logo.png" alt="dajarSystemZwrotow" class="logo"/>
            <hr>
            <div id="main">
                <div onclick="location.href='/login.aspx'" class="index-tile">
                    <img src="login.gif" alt="login img" class="tile-img"/>
                    LOGOWANIE
                </div>
                <div onclick="location.href='/storage.aspx'" class="index-tile">
                    <img src="storage.gif" alt="storage img" class="tile-img"/>
                    MAGAZYN
                </div>
                <div onclick="location.href='/package.aspx'" class="index-tile">
                    <img src="package.gif" alt="package img" class="tile-img"/>
                    PAKOWANIE
                </div>
            </div>
            <hr>
        </div>
    </div>
</form>
</body>

</html>