﻿<%@ Page Language="C#" CodeFile="err500.aspx.cs" Inherits="ClientPages.err500" EnableViewState="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <title>Internal Error</title>
        <base href="<%= Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath + (!Request.ApplicationPath.EndsWith("/") ? "/" : string.Empty) %>" />
        <style type="text/css">
            body {background: #2F3032 url('info/images/err_bg.gif') repeat-x scroll 0 0; margin:0px; padding:0px; font-family:Tahoma;}
            #dvErrContainer {margin-left:20px; margin-top:20px; text-align:center;}
            #dvErrContainer .ErrDescription {margin-top:10px; color:#d1d1d1; font:bold 18px Arial;}
            #dvErrContainer .ErrNotify {height:19px; color:white; font-family:arial; font-size:13px; margin-top:11px;}
            .ErrTextCss {width:90%; height:200px; background-color:Gray; text-align:left; padding:10px; margin-top:20px; color:White;}
            a {color:White; font-family:Arial;} 
        </style>
    </head>
    <body>
        <!--<div id="dvErrContainer">
            <div class="ErrDescription" >We're sorry, an internal server error has occurred</div>
            <div class="ErrNotify" >We have logged this error and will investigate the cause.</div>
            <br />
            <a href="." >back to main page</a>
        </div>-->
        <div id="dvErrContainer">
            <div class="ErrDescription">Приносим извинения, произошла внутренняя ошибка на сервере.</div>
            <div class="ErrNotify">Мы записали ошибку в журнал событий и разберемся с причинами её появления.</div>
            <br />
            <a href="." >На главную.</a>
        </div>
        <div style="text-align:center;">
            <img src="info/images/hypnotoad.gif" alt="What?" border="0"/>
        </div>
    </body>
</html>