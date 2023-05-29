<%@ Page Title="Administrar cuenta" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="Facturador.GHO.Account.Manage" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <div class="form-horizontal">
        <h4>Formulario para cambiar contraseña</h4>
        <hr />
        <div class="row">
            <div class="col-md-4"></div>
            <div class="col-md-4">
                <div class="col-md-12">
                    <p>Ha iniciado sesión como <strong><%: User.Identity.GetUserName() %></strong>.</p>
                    <asp:PlaceHolder runat="server" ID="successMessage" Visible="false" ViewStateMode="Disabled">
                        <p class="text-success" style="color:green;"><%: SuccessMessage %></p>
                    </asp:PlaceHolder>
                </div>
                <!-- Contraseña actual -->
                <div class="col-md-12">
                    <asp:Label runat="server" ID="CurrentPasswordLabel" AssociatedControlID="CurrentPassword" CssClass="control-label">Contraseña actual</asp:Label>
                    <asp:TextBox runat="server" ID="CurrentPassword" TextMode="Password" CssClass="form-control" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="CurrentPassword"
                        CssClass="text-danger" ErrorMessage="El campo de contraseña actual es obligatorio."
                        ValidationGroup="ChangePassword" />
                    <asp:ValidationSummary runat="server" ShowModelStateErrors="true" CssClass="text-danger" />
                </div>
                <!-- Nueva contraseña -->
                <div class="col-md-12">
                    <asp:Label runat="server" ID="NewPasswordLabel" AssociatedControlID="NewPassword" CssClass="control-label">Nueva contraseña</asp:Label>
                    <asp:TextBox runat="server" ID="NewPassword" TextMode="Password" CssClass="form-control" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="NewPassword"
                        CssClass="text-danger" ErrorMessage="La contraseña nueva es obligatoria."
                        ValidationGroup="ChangePassword" />
                </div>
                <!-- Confirmar contraseña -->
                <div class="col-md-12">
                    <asp:Label runat="server" ID="ConfirmNewPasswordLabel" AssociatedControlID="ConfirmNewPassword" CssClass="control-label">Confirmar la nueva contraseña</asp:Label>
                    <asp:TextBox runat="server" ID="ConfirmNewPassword" TextMode="Password" CssClass="form-control" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ConfirmNewPassword"
                        CssClass="text-danger" Display="Dynamic" ErrorMessage="La confirmación de la nueva contraseña es obligatoria."
                        ValidationGroup="ChangePassword" />
                    <asp:CompareValidator runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword"
                        CssClass="text-danger" Display="Dynamic" ErrorMessage="La nueva contraseña y la contraseña de confirmación no coinciden."
                        ValidationGroup="ChangePassword" />
                </div>
                <!-- Btn Cambiar contraseña -->
                <div class="col-md-12" style="padding-top: 15px;margin-bottom: 20px;">
                    <asp:Button runat="server" Text="Cambiar contraseña" ValidationGroup="ChangePassword" OnClick="ChangePassword_Click" CssClass="btn btn-success" style="width: 100%;"/>
                </div>
            </div>
            <div class="col-md-12">
                <section id="passwordForm">
                    <asp:PlaceHolder runat="server" ID="setPassword" Visible="false">
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="changePasswordHolder" Visible="false">
                    </asp:PlaceHolder>
                </section>
                <section id="externalLoginsForm">

                    <asp:ListView runat="server"
                        ItemType="Microsoft.AspNet.Identity.UserLoginInfo"
                        SelectMethod="GetLogins" DeleteMethod="RemoveLogin" DataKeyNames="LoginProvider,ProviderKey">

                        <LayoutTemplate>
                            <h4>Inicios de sesión registrados</h4>
                            <table class="table">
                                <tbody>
                                    <tr runat="server" id="itemPlaceholder"></tr>
                                </tbody>
                            </table>

                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%#: Item.LoginProvider %></td>
                                <td>
                                    <asp:Button runat="server" Text="Quitar" CommandName="Delete" CausesValidation="false"
                                        ToolTip='<%# "Quitar este " + Item.LoginProvider + " inicio de sesión de su cuenta" %>'
                                        Visible="<%# CanRemoveExternalLogins %>" CssClass="btn btn-default" />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:ListView>
                </section>
            </div>
        </div>
    </div>

</asp:Content>
