<%@ Page Title="Configuración" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Configuracion.aspx.cs" Inherits="Facturador.GHO.Admin.Configuracion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="../Content/jQuery.FileUpload/css/jquery.fileupload.css" rel="stylesheet" />
    <script src="../Scripts/jquery-1.10.2.js"></script>
    <script type="text/javascript">
        function showimagepreview(input) {
            if (input.files && input.files[0]) {
                var filerdr = new FileReader();
                filerdr.onload = function (e) {
                    $('#imgDisplay').attr('src', e.target.result);
                }
                filerdr.readAsDataURL(input.files[0]);
            }
        }
    </script>
    <h2><%: Title %>.</h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>

    <div class="form-horizontal">
        <hr />
        <h3>Servisim</h3>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="UsuarioServisim" CssClass="col-md-2 control-label">Usuario</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="UsuarioServisim" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="UsuarioServisim" Display="Dynamic"
                                CssClass="text-danger" ErrorMessage="El campo usuario es obligatorio." ValidationGroup="groupServisim" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="PasswordServisim" CssClass="col-md-2 control-label">Contraseña</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="PasswordServisim" CssClass="form-control" TextMode="Password" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="PasswordServisim" Display="Dynamic"
                                CssClass="text-danger" ErrorMessage="El campo contraseña es obligatorio." ValidationGroup="groupServisim" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="TokenServisim" CssClass="col-md-2 control-label">Token</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="TokenServisim" CssClass="form-control"  TextMode="Password" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="TokenServisim" Display="Dynamic"
                                CssClass="text-danger" ErrorMessage="El campo token es obligatorio." ValidationGroup="groupServisim" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" CssClass="col-md-2 control-label"></asp:Label>
            <div class="col-md-10">
                <asp:Button ID="btnServisim" OnClick="btnServisim_Click" runat="server" Text="Actualizar" ValidationGroup="groupServisim" CssClass="btn btn-primary" />
            </div>
        </div>
        <hr />
        <h3>Aplicación</h3>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="AplicacionNombre" CssClass="col-md-2 control-label">Nombre</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="AplicacionNombre" CssClass="form-control" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" CssClass="col-md-2 control-label"></asp:Label>
            <div class="col-md-10">
                <asp:Button ID="btnNombre" OnClick="btnNombre_Click" runat="server" Text="Actualizar" CssClass="btn btn-primary" />
            </div>
        </div>
        <hr />
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="NumeroSerie" CssClass="col-md-2 control-label">No. de serie</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="NumeroSerie" CssClass="form-control" ReadOnly="true" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="Estatus" CssClass="col-md-2 control-label">Estatus</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="Estatus" CssClass="form-control" ReadOnly="true" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" CssClass="col-md-2 control-label"></asp:Label>
            <div class="col-md-10">
                <span class="btn btn-success fileinput-button">
                    <i class="glyphicon glyphicon-plus"></i>
                    <span>Seleccionar archivo...</span>
                    <input id="fileLicense" type="file" name="fileLicense" accept=".vali" runat="server">
                </span>
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" CssClass="col-md-2 control-label"></asp:Label>
            <div class="col-md-10">
                <asp:Button ID="btnActivar" OnClick="btnActivar_Click" runat="server" Text="Activar" CssClass="btn btn-primary" />
            </div>
        </div>
        <hr />
        <h3>Logotipo</h3>
        <div class="form-group">
            <asp:Label runat="server" CssClass="col-md-2 control-label"></asp:Label>
            <div class="col-md-10">
                <span class="btn btn-success fileinput-button">
                    <i class="glyphicon glyphicon-plus"></i>
                    <span>Seleccionar archivo...</span>
                    <input id="file1" type="file" name="filUpload" accept="image/*" runat="server" onchange="showimagepreview(this)">
                </span>
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" CssClass="col-md-2 control-label"></asp:Label>
            <div>
                <img id="imgDisplay" alt="" src="../Facturacion/Logo/logo.png" style="height: 100px" />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" CssClass="col-md-2 control-label"></asp:Label>
            <div class="col-md-10">
                <asp:Button ID="btnLogo" runat="server" OnClick="btnLogo_Click" Text="Actualizar" CssClass="btn btn-primary" />
            </div>
        </div>
        <hr />
        <h3>Correo</h3>
        <div class="row">
            <div class="col-md-6">
                <div class="form-horizontal">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Host" CssClass="col-md-4 control-label">Host</asp:Label>
                        <div class="col-md-8">
                            <asp:TextBox runat="server" ID="Host" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Host" Display="Dynamic"
                                CssClass="text-danger" ErrorMessage="El campo host es obligatorio." ValidationGroup="groupCorreo" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Puerto" CssClass="col-md-4 control-label">Puerto SMTP</asp:Label>
                        <div class="col-md-8">
                            <asp:TextBox runat="server" ID="Puerto" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Puerto" Display="Dynamic"
                                CssClass="text-danger" ErrorMessage="El campo puerto es obligatorio." ValidationGroup="groupCorreo" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="SSL" CssClass="col-md-4 control-label">SSL</asp:Label>
                        <div class="col-md-8">
                            <asp:CheckBox runat="server" ID="SSL" />
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="form-horizontal">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Nombre" CssClass="col-md-4 control-label">Usuario</asp:Label>
                        <div class="col-md-8">
                            <asp:TextBox runat="server" ID="Nombre" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Nombre" Display="Dynamic"
                                CssClass="text-danger" ErrorMessage="El campo usuario es obligatorio." ValidationGroup="groupCorreo" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-4 control-label">Correo</asp:Label>
                        <div class="col-md-8">
                            <asp:TextBox runat="server" ID="Email" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Email" Display="Dynamic"
                                CssClass="text-danger" ErrorMessage="El campo email es obligatorio." ValidationGroup="groupCorreo" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Contrasenia" CssClass="col-md-4 control-label">Contraseña</asp:Label>
                        <div class="col-md-8">
                            <asp:TextBox runat="server" ID="Contrasenia" CssClass="form-control" TextMode="Password" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Contrasenia" Display="Dynamic"
                                CssClass="text-danger" ErrorMessage="El campo contraseña es obligatorio." ValidationGroup="groupCorreo" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" CssClass="col-md-2 control-label"></asp:Label>
            <div class="col-md-10">
                <asp:Button ID="btnCorreo" runat="server" OnClick="btnCorreo_Click" Text="Actualizar" ValidationGroup="groupCorreo" CssClass="btn btn-primary" />
            </div>
        </div>
    </div>
</asp:Content>
