<%@ Page Title="Iniciar sesión" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Facturador.GHO.Account.Login" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <div class="col-sm-12 text-center">
              <script src="https://unpkg.com/@lottiefiles/lottie-player@latest/dist/lottie-player.js"></script>
              <lottie-player src="https://assets2.lottiefiles.com/packages/lf20_bHVEVT.json"  background="transparent"  speed="1"  style="width: 111px; height: 300px; margin-bottom: -177px;margin-top: -70px; margin-left: 358px;"  loop  autoplay></lottie-player>
          </div>

    <h2><%: Title %>.</h2>

    <h4>Utilice una cuenta local para iniciar sesión.</h4>
    <hr />
    <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
        <p class="text-danger">
            <asp:Literal runat="server" ID="FailureText" />
        </p>
    </asp:PlaceHolder>
    <div class="row">
        <div class="col-md-6">
            <section id="loginForm">
                <div class="form-horizontal">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="UserName" CssClass="col-md-4 control-label">Nombre de usuario</asp:Label>
                        <div class="col-md-8">
                            <asp:TextBox runat="server" ID="UserName" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="UserName"
                                CssClass="text-danger" ErrorMessage="El campo de nombre de usuario es obligatorio." />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-4 control-label">Contraseña</asp:Label>
                        <div class="col-md-8">
                            <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="El campo de contraseña es obligatorio." />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-md-offset-2 col-md-8">
                            <div class="checkbox">
                                <asp:CheckBox runat="server" ID="RememberMe" />
                                <asp:Label runat="server" AssociatedControlID="RememberMe">¿Recordar cuenta?</asp:Label>
                            </div>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-md-offset-2 col-md-8">
                            <asp:Button runat="server" OnClick="LogIn" Text="Iniciar sesión" CssClass="btn btn-default" />
                        </div>
                    </div>
                </div>
                <p>
                    <asp:HyperLink runat="server" ID="RecoverHyperLink" ViewStateMode="Disabled" NavigateUrl="~/Account/Recover.aspx">Recuperar contraseña.</asp:HyperLink>
                </p>
            </section>
        </div>

        <div class="col-md-6" style="vertical-align: middle; align-content: center; text-align: center; visibility:hidden">
            <img src="../Facturacion/Logo/logo.png" style="height: 113px;" />
        </div>
    </div>
</asp:Content>
