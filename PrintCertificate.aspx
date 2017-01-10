<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PrintCertificate.aspx.cs"
    Inherits="ClientPages.PrintCertificate" EnableViewState="false" %>
<%@ Register tagPrefix="adv" tagName="GiftCertificate" src="~/UserControls/GiftCertificate.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script type="text/javascript">
        function position_this_window() {
            var x = (screen.availWidth - 770) / 2;
            window.resizeTo(762, 662);
            window.moveTo(Math.floor(x), 50);
        }
    </script>
    <link type="text/css" rel="stylesheet" href="css/styles.css?p=20151228"/>
    <link type="text/css" rel="stylesheet" href="css/styles-extra.css"/>
</head>
<body onload="position_this_window(); window.print();" style="padding: 30px;">
    <adv:GiftCertificate runat="server" />
    <script type="text/javascript" src="js/jq/jquery-1.7.1.min.js"></script>
    <script type="text/javascript" src="js/common.js"></script>
</body>
</html>
