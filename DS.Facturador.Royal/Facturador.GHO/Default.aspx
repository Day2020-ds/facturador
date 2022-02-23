<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Facturador.GHO._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-12 text-center">
       <h1>Facturador Daysoft</h1>
       <h2>Timbra y consulta tus documentos</h2>
    </div>
    <div class="col-md-12" style="height:150px;">
        <script src="https://unpkg.com/@lottiefiles/lottie-player@latest/dist/lottie-player.js"></script>
        <lottie-player src="https://assets10.lottiefiles.com/packages/lf20_bR3P9F.json"  background="transparent"  speed="1"  loop  autoplay></lottie-player>
    </div>
    <!-- íconos -->
    <div class="col-12 col-sm-6 col-md-3">
        <div class="info-box">
            <span class="info-box-icon bg-success elevation-1"><i class="fas fa-plus-circle"></i></span>
            <div class="info-box-content">
                <span class="info-box-text">Timbres disponibles</span>
                <span class="info-box-number">
                    8                                  
                </span>
            </div>
            <!-- /.info-box-content -->
        </div>
    <!-- /.info-box -->
    </div>
    <!-- /.col -->
    <div class="col-12 col-sm-6 col-md-3">
        <div class="info-box mb-3">
            <span class="info-box-icon bg-yellow elevation-1"><i class="fas fa-boxes"></i></span>
            <div class="info-box-content">
                <span class="info-box-text">Total Productos</span>
                <span class="info-box-number">
                    1
                </span>
            </div>
            <!-- /.info-box-content -->
        </div>
    <!-- /.info-box -->
    </div>
    <!-- /.col -->
    <!-- /.col -->
    <div class="col-12 col-sm-6 col-md-3">
        <div class="info-box mb-3">
            <span class="info-box-icon bg-primary elevation-1"><i class="fas fa-file-invoice"></i></span>
            <div class="info-box-content">
                <span class="info-box-text">Documentos timbrados</span>
                <span class="info-box-number">
                    18
                </span>
            </div>
            <!-- /.info-box-content -->
        </div>
    <!-- /.info-box -->
    </div>
    <!-- /.col -->
    <!-- /.col -->
    <div class="col-12 col-sm-6 col-md-3">
        <div class="info-box mb-3">
            <span class="info-box-icon bg-danger elevation-1"><i class="fas fa-ban"></i></span>
            <div class="info-box-content">
                <span class="info-box-text">Documentos cancelados</span>
                <span class="info-box-number">
                    5
                </span>
            </div>
            <!-- /.info-box-content -->
        </div>
    <!-- /.info-box -->
    </div>
    <!-- /.col -->


    <!-- 
    <div class="jumbotron" style="background-color: white;">

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
    -->

    <div class="row" style="visibility:hidden">
        <div class="col-md-10 text-center">
            <h4>Facturación</h4>
        </div>
    </div>

</asp:Content>
