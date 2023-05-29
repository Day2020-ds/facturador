<%@ Page Title="Recuperar contraseña" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Recover.aspx.cs" Inherits="Facturador.GHO.Account.Recover" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>

    <div class="form-horizontal">
        <h4>Introduzca la siguiente información para regenerar la contraseña.</h4>
        <hr />
        <div class="row">
            <div class="col-md-4"></div>
            <div class="col-md-4">
                <!-- Nombre de usuario -->
                <div class="col-md-12">
                    <asp:Label runat="server" AssociatedControlID="UserName" CssClass="control-label">Nombre de usuario</asp:Label>
                    <asp:TextBox runat="server" ID="UserName" CssClass="form-control" placeholder="@nombreusuario"/>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="UserName" Display="Dynamic"
                        CssClass="text-danger" ErrorMessage="El campo nombre de usuario es obligatorio." />
                </div>
                <!-- Email -->
                <div class="col-md-12">
                    <asp:Label runat="server" AssociatedControlID="Correo" CssClass="control-label">Correo electrónico</asp:Label>
                    <asp:TextBox runat="server" ID="Correo" CssClass="form-control" placeholder="alguien@correo.com"/>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="Correo" Display="Dynamic"
                        CssClass="text-danger" ErrorMessage="El campo de correo es obligatorio." />
                    <asp:RegularExpressionValidator runat="server" ErrorMessage="El correo no es valido." Display="Dynamic"
                        CssClass="text-danger" ControlToValidate="Correo" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" />
                </div>
                <!-- Btn Enviar -->
                <div class="col-md-12" style="padding-top:15px;">
                    <asp:Button runat="server" ID="Recuperar" OnClick="Recover_Click" Text="Enviar" CssClass="btn btn-primary" style="width:100%;"/>
                </div>
                <div class="col-md-12">
                    <p>La contraseña sera enviada al correo proporcionado</p>
                </div>
            </div>
            <div class="col-md-4"></div>
        </div>
        <br />
    </div>
</asp:Content>
