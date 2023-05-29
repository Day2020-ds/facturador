<%@ Page Title="Iniciar sesión" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Facturador.GHO.Account.Login" Async="true" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    
    
    <div class="row justify-content-center">
            <div class="col-md-9 col-lg-12 col-xl-10">
                <div class="card shadow-lg o-hidden border-0 my-5">
                    <div class="card-body p-0">
                        <div class="row">
                            <div class="col-lg-6 img-login">
                                <img src="../Content/Image/img01.jpeg" alt="Login"/>
                            </div>
                            <div class="col-lg-6 form-login">
                                <div class="p-5">
                                    <div class="text-center">
                                        <h3 class="text-dark mb-4">Facturación Electrónica</h3>
                                        <h4 class="text-dark mb-4">Bienvenido</h4>
                                    </div>
                                    <div class="col-sm-12 text-center">
                                        <script src="https://unpkg.com/@lottiefiles/lottie-player@latest/dist/lottie-player.js"></script>
                                        <lottie-player src="https://assets2.lottiefiles.com/packages/lf20_bHVEVT.json"  background="transparent"  speed="1"  loop  autoplay></lottie-player>
                                    </div>
                                    <div class="col-md-12">
                                        <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                                            <p class="text-danger">
                                                <asp:Literal runat="server" ID="FailureText" />
                                            </p>
                                        </asp:PlaceHolder>
                                    </div>
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <label for="UserName"><strong>Usuario: </strong></label>
                                            <asp:TextBox runat="server" ID="UserName" CssClass="form-control" placeholder="Nombre de usuario"/>
                                            <asp:RequiredFieldValidator runat="server" ControlToValidate="UserName"
                                                CssClass="text-danger" ErrorMessage="El campo de nombre de usuario es obligatorio." />
                                        </div>
                                      <div class="form-group">
                                        <label for="Password"><strong>Contraseña: </strong></label>
                                        <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" placeholder="* * * * * * * * "/>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Password" CssClass="text-danger" ErrorMessage="El campo de contraseña es obligatorio." />
                                      </div>
                                      <asp:Button runat="server" OnClick="LogIn" Text="Iniciar sesión" CssClass="btn-login" />
                                      <hr>
                                      <p class="message">¿Olvidó su contraseña? 
                                          <asp:HyperLink runat="server" ID="RecoverHyperLink" ViewStateMode="Disabled" NavigateUrl="~/Account/Recover.aspx">Recuperar contraseña.</asp:HyperLink>
                                      </p>
                                    </div>                                    
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    


    <!-- 

    <h2><%: Title %>.</h2>

    <h4>Utilice una cuenta local para iniciar sesión.</h4>
    <hr />
    <asp:PlaceHolder runat="server" ID="ErrorMessage0" Visible="false">
        <p class="text-danger">
            <asp:Literal runat="server" ID="FailureText0" />
        </p>
    </asp:PlaceHolder>
    <div class="row">
        <div class="col-md-6">
            <section id="loginForm">
                <div class="form-horizontal">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="UserName0" CssClass="col-md-4 control-label">Nombre de usuario</asp:Label>
                        <div class="col-md-8">
                            <asp:TextBox runat="server" ID="UserName0" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="UserName0"
                                CssClass="text-danger" ErrorMessage="El campo de nombre de usuario es obligatorio." />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Password0" CssClass="col-md-4 control-label">Contraseña</asp:Label>
                        <div class="col-md-8">
                            <asp:TextBox runat="server" ID="Password0" TextMode="Password" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Password0" CssClass="text-danger" ErrorMessage="El campo de contraseña es obligatorio." />
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
                    <asp:HyperLink runat="server" ID="RecoverHyperLink0" ViewStateMode="Disabled" NavigateUrl="~/Account/Recover.aspx">Recuperar contraseña.</asp:HyperLink>
                </p>
            </section>
        </div>

        <div class="col-md-6" style="vertical-align: middle; align-content: center; text-align: center; visibility:hidden">
            <img src="../Facturacion/Logo/logo.png" style="height: 113px;" />
        </div>
    </div>
    -->
</asp:Content>
