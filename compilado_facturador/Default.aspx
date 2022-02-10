<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Facturador.GHO._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div style="background-color: white;" class="jumbotron">
        <%--<img src="Facturacion/Logo/logo.png" style="height: 97px;" />
        <img src="Facturacion/Logo/Royal2.png" style="height: 97px; float: right" />--%>
          <div class="col-sm-12 text-center">
              <script src="https://unpkg.com/@lottiefiles/lottie-player@latest/dist/lottie-player.js"></script>
              <lottie-player src="https://assets10.lottiefiles.com/packages/lf20_bR3P9F.json"  background="transparent"  speed="1"  style="width: 200px; height: 300px; margin-bottom: -177px;margin-top: -70px;"  loop  autoplay></lottie-player>
          </div>
        <h1 style="text-align: center; ">Facturador Daysoft</h1>
        <p class="lead" style="text-align: center;">Timbra y consulta tus documentos</p>
        <%--<p>
            <a class="btn  btn-primary btn-large" href="Cliente/Generar.aspx">Factura aqui &raquo;</a>
        </p>--%>
    </div>

    <div class="row" style="visibility:hidden">
        <div class="col-md-10 text-center">
            <h4>Facturación</h4>
        </div>
    </div>

</asp:Content>
