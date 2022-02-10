<%@ Page Title="CFDI RIP" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CFDIRIP.aspx.cs" Inherits="Facturador.GHO.Usuario.CFDIRIP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>

    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>
    <div class="form-horizontal">
        <h4>CFDI de retenciones e información de pagos</h4>
    </div>
    <div class="form-horizontal">
        <hr />
        <h4>Emisor: </h4>
        <div class="row" id="emisor">
            <div class="col-md-6">
                <div runat="server" id="divEmisor">
                    <div class="form-group">
                        <asp:Label ID="lblEmpresa" runat="server" CssClass="col-md-3 control-label">*Empresa: </asp:Label>
                        <div class="col-md-8">
                            <asp:DropDownList ID="ddlEmpresa" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlEmpresa_SelectedIndexChanged"></asp:DropDownList>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlEmpresa" Display="Dynamic" CssClass="text-danger" ErrorMessage="La empresa es obligatoria." ID="rfvddlEmpresa" />
                        </div>
                    </div>
                    <asp:UpdatePanel ID="updEmisor" runat="server">
                        <ContentTemplate>
                            <div class="form-group">
                                <asp:Label ID="lblRFCEmisor" runat="server" CssClass="col-md-3 control-label">*RFC: </asp:Label>
                                <asp:TextBox ID="txbRFCEmisor" runat="server" MaxLength="13" CssClass="form-control col-md-8"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvtxbRFCEmisor" runat="server" ControlToValidate="txbRFCEmisor" CssClass="text-danger" Display="Dynamic" ErrorMessage="El RFC es obligatorio." />
                                <asp:RegularExpressionValidator ID="revtxbRFCEmisor" runat="server" ControlToValidate="txbRFCEmisor" CssClass="text-danger" Display="Dynamic" ErrorMessage="El RFC no es valido" ValidationExpression="[A-Z,Ñ,&amp;]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]?[A-Z,0-9]?[0-9,A-Z]?"></asp:RegularExpressionValidator>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblRazonSocialEmisor" runat="server" CssClass="col-md-3 control-label">Razon Social: </asp:Label>
                                <asp:TextBox ID="txbRazonSocial" runat="server" CssClass="form-control col-md-8" MaxLength="300"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblCURPEmisor" runat="server" CssClass="col-md-3 control-label">CURP: </asp:Label>
                                <asp:TextBox ID="txbCURPEmisor" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="revtxbCURPEmisor" runat="server" ControlToValidate="txbCURPEmisor" CssClass="text-danger" Display="Dynamic" ErrorMessage="El CURP no es valido." ValidationExpression="([A-Z][A,E,I,O,U,X][A-Z]{2}[0-9]{2}[0-1][0-9][0-3][0-9][M,H][A-Z]{2}[B,C,D,F,G,H,J,K,L,M,N,Ñ,P,Q,R,S,T,V,W,X,Y,Z]{3}[0-9,A-Z][0-9])?"></asp:RegularExpressionValidator>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlEmpresa" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>

        <hr />
        <h4>Datos: </h4>
        <div class="row" id="datos">
            <div class="col-md-6">
                <div runat="server" id="divDatos">
                    <div class="form-group">
                        <asp:Label ID="lblFolioInterno" runat="server" CssClass="col-md-3 control-label">Folio interno: </asp:Label>
                        <asp:TextBox ID="txbFolioInterno" runat="server" MaxLength="20" CssClass="form-control col-md-8"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblClaveRetencion" runat="server" CssClass="col-md-3 control-label">*Tipo de retencion: </asp:Label>
                        <asp:DropDownList ID="ddlClaveRetencion" runat="server" CssClass="form-control col-md-8"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvddlClaveRetencion" runat="server" ControlToValidate="ddlClaveRetencion" CssClass="text-danger" Display="Dynamic" ErrorMessage="El Tipo de retencion es obligatorio."></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblDescripcionRetencion" runat="server" CssClass="col-md-3 control-label">Descripcion de la retencion: </asp:Label>
                        <asp:TextBox ID="txbDescripcionRetencion" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="100"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>
        <hr />
        <h4>Cliente: </h4>
        <div class="row" id="cliente">
            <div class="col-md-6">
                <div runat="server" id="divCliente">
                    <div class="form-group">
                        <asp:Label ID="lblNacionalidad" runat="server" CssClass="col-md-3 control-label">*Nacionalidad: </asp:Label>
                        <asp:DropDownList ID="ddlNacionalidad" runat="server" AutoPostBack="true" CssClass="form-control col-md-8" OnSelectedIndexChanged="ddlNacionalidad_SelectedIndexChanged"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvddlNacionalidad" runat="server" ControlToValidate="ddlNacionalidad" CssClass="text-danger" Display="Dynamic" ErrorMessage="La Nacionalidad es obligatoria."></asp:RequiredFieldValidator>
                    </div>
                    <asp:UpdatePanel ID="updNacionalidad" runat="server">
                        <ContentTemplate>
                            <div class="form-group">
                                <asp:Label ID="lblIdentificador" runat="server" CssClass="col-md-3 control-label">*Identificador: </asp:Label>
                                <asp:DropDownList ID="ddlIdentificadorCliente" runat="server" AutoPostBack="true" CssClass="form-control col-md-8" OnSelectedIndexChanged="ddlIdentificadorCliente_SelectedIndexChanged"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvddlIdentificadorCliente" runat="server" ControlToValidate="ddlIdentificadorCliente" CssClass="text-danger" Display="Dynamic" ErrorMessage="El identificador es obligatorio."></asp:RequiredFieldValidator>
                            </div>
                            <asp:UpdatePanel ID="updFormularioCliente" runat="server">
                                <ContentTemplate>
                                    <div class="form-group">
                                        <asp:Label ID="lblRazonSocialCliente" runat="server" CssClass="col-md-3 control-label">*Razon Social: </asp:Label>
                                        <asp:TextBox ID="txbRazonSocialCliente" runat="server" CssClass="form-control col-md-8" MaxLength="300"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbRazonSocialCliente" runat="server" ControlToValidate="txbRFCCliente" CssClass="text-danger" Display="Dynamic" ErrorMessage="La razon social es obligatoria."></asp:RequiredFieldValidator>
                                    </div>
                                    <div runat="server" id="divClienteNacional" visible="false">
                                        <div class="form-group">
                                            <asp:Label ID="lblRFCCliente" runat="server" CssClass="col-md-3 control-label">*RFC: </asp:Label>
                                            <asp:TextBox ID="txbRFCCliente" runat="server" CssClass="form-control col-md-8" MaxLength="13"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvtxbRFCCliente" runat="server" ControlToValidate="txbRFCCliente" CssClass="text-danger" Display="Dynamic" ErrorMessage="El RFC es obligatorio." />
                                            <asp:RegularExpressionValidator ID="revtxbRFCCliente" runat="server" ControlToValidate="txbRFCCliente" CssClass="text-danger" Display="Dynamic" ErrorMessage="El RFC no es valido" ValidationExpression="[A-Z,Ñ,&amp;]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]?[A-Z,0-9]?[0-9,A-Z]?"></asp:RegularExpressionValidator>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label ID="lblCURPCliente" runat="server" CssClass="col-md-3 control-label">CURP: </asp:Label>
                                            <asp:TextBox ID="txbCURPCliente" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="revtxbCURPCliente" runat="server" ControlToValidate="txbCURPCliente" CssClass="text-danger" Display="Dynamic" ErrorMessage="El CURP no es valido." ValidationExpression="([A-Z][A,E,I,O,U,X][A-Z]{2}[0-9]{2}[0-1][0-9][0-3][0-9][M,H][A-Z]{2}[B,C,D,F,G,H,J,K,L,M,N,Ñ,P,Q,R,S,T,V,W,X,Y,Z]{3}[0-9,A-Z][0-9])?"></asp:RegularExpressionValidator>
                                        </div>
                                    </div>
                                    <div runat="server" id="divClienteExtranjero" class="form-group" visible="false">
                                        <asp:Label ID="lblNumRegIdentFiscalCliente" runat="server" CssClass="col-md-3 control-label">Numero de registro de identificación fiscal: </asp:Label>
                                        <asp:TextBox ID="txbNumRegIdentFiscalCliente" runat="server" CssClass="form-control col-md-8" MaxLength="20"></asp:TextBox>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlIdentificadorCliente" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlNacionalidad" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
        <hr />
        <h4>Periodo: </h4>
        <div class="row" id="periodo">
            <div class="col-md-6">
                <div runat="server" id="divPeriodo">
                    <div class="form-group">
                        <asp:Label ID="lblMesInicial" runat="server" CssClass="col-md-3 control-label">*Mes inicial: </asp:Label>
                        <asp:DropDownList ID="ddlMesInicial" runat="server" AutoPostBack="true" CssClass="form-control col-md-8" OnSelectedIndexChanged="ddlMesInicial_SelectedIndexChanged"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvddlMesInicial" runat="server" ControlToValidate="ddlMesInicial" CssClass="text-danger" Display="Dynamic" ErrorMessage="El Mes inicical es obligatorio."></asp:RequiredFieldValidator>
                    </div>
                    <asp:UpdatePanel ID="updPeriodo" runat="server">
                        <ContentTemplate>
                            <div class="form-group">
                                <asp:Label ID="lblMesFinal" runat="server" CssClass="col-md-3 control-label">*Mes final: </asp:Label>
                                <asp:DropDownList ID="ddlMesFinal" runat="server" CssClass="form-control col-md-8"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvddlMesFinal" runat="server" ControlToValidate="ddlMesFinal" CssClass="text-danger" Display="Dynamic" ErrorMessage="El mes final es obligatorio."></asp:RequiredFieldValidator>
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlMesInicial" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <div class="form-group">
                        <asp:Label ID="lblEjercicioFiscal" runat="server" CssClass="col-md-3 control-label">*Año de ejercicio fiscal: </asp:Label>
                        <asp:DropDownList ID="ddlEjercicioFiscal" runat="server" CssClass="form-control col-md-8"></asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvddlEjercicioFiscal" runat="server" ControlToValidate="ddlEjercicioFiscal" CssClass="text-danger" Display="Dynamic" ErrorMessage="El año de ejercicio fiscal es obligatorio."></asp:RequiredFieldValidator>
                    </div>
                </div>
            </div>
        </div>
        <hr />
        <h4>Totales: </h4>
        <div class="row" id="totales">
            <div class="col-md-6">
                <div runat="server" id="divTotales">
                    <div class="form-group">
                        <asp:Label ID="lblMontoTotalDeOperacion" runat="server" CssClass="col-md-3 control-label">*Monto total de operación: </asp:Label>
                        <asp:TextBox ID="txbMontoTotalDeOperacion" runat="server" CssClass="form-control col-md-8" MaxLength="18" AutoCompleteType="Disabled"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvtxbMontoTotalDeOperacion" runat="server" ControlToValidate="txbMontoTotalDeOperacion" CssClass="text-danger" Display="Dynamic" ErrorMessage="El monto total de operacion es obligatorio."></asp:RequiredFieldValidator>
                        <asp:Label ID="lblValidaciontxbMontoTotalDeOperacion" runat="server" Visible="false" Text="El monto total de operacion no es valido."></asp:Label>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblMontototalExento" runat="server" CssClass="col-md-3 control-label">*Monto total exento: </asp:Label>
                        <asp:TextBox ID="txbMontototalExento" runat="server" CssClass="form-control col-md-8" MaxLength="18" AutoCompleteType="Disabled"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvtxbMontototalExento" runat="server" ControlToValidate="txbMontototalExento" CssClass="text-danger" Display="Dynamic" ErrorMessage="El monto total de exento es obligatorio."></asp:RequiredFieldValidator>
                        <asp:Label ID="lblValidaciontxbMontototalExento" runat="server" Visible="false" Text="El monto total exento no es valido."></asp:Label>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblMontoTotalGravado" runat="server" CssClass="col-md-3 control-label">*Monto total gravado: </asp:Label>
                        <asp:TextBox ID="txbMontoTotalGravado" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvtxbMontoTotalGravado" runat="server" ControlToValidate="txbMontoTotalGravado" CssClass="text-danger" Display="Dynamic" ErrorMessage="El monto total gravado es obligatorio."></asp:RequiredFieldValidator>
                        <asp:Label ID="lblValidaciontxbMontoTotalGravado" runat="server"  Visible="false" Text="El monto total gravado no es valido."></asp:Label>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblMontoTotalRetenciones" runat="server" CssClass="col-md-3 control-label">*Monto total de las retenciones: </asp:Label>
                        <asp:TextBox ID="txbMontoTotalRetenciones" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvtxbMontoTotalRetenciones" runat="server" ControlToValidate="txbMontoTotalRetenciones" CssClass="text-danger" Display="Dynamic" ErrorMessage="El monto total de la retenciones es obligatorio."></asp:RequiredFieldValidator>
                        <asp:Label ID="lblValidaciontxbMontoTotalRetenciones" runat="server" Visible="false" Text="El monto total de las retenciones no es valido."></asp:Label>
                    </div>
                    <hr />
                    <h4>Importe Retenido (opcional)</h4>
                    <div runat="server" id="divImpuestosRetenidos">
                        <div class="form-group">
                            <asp:Label ID="lblBaseImpuesto" runat="server" CssClass="col-md-3 control-label">Base del impuesto: </asp:Label>
                            <asp:TextBox ID="txbBaseImpuesto" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                            <asp:Label ID="lblValidaciontxbBaseImpuesto" runat="server" Visible="false" Text="La base del impuesto no es valido."></asp:Label>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="lblImpuesto" runat="server" CssClass="col-md-3 control-label">Impuesto: </asp:Label>
                            <asp:DropDownList ID="ddlImpuesto" runat="server" CssClass="form-control col-md-8"></asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="lblMontoRetencion" runat="server" CssClass="col-md-3 control-label">*Monto de la retención: </asp:Label>
                            <asp:TextBox ID="txbMontoRetencion" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvtxbMontoRetencion" runat="server" ControlToValidate="txbMontoRetencion" CssClass="text-danger" ValidationGroup="vgImpuestosRetenidos" Display="Dynamic" ErrorMessage="El impuesto retenido es obligatorio."></asp:RequiredFieldValidator>
                            <asp:Label ID="lblValidaciontxbMontoRetencion" runat="server" Visible="false" Text="El monto de la retención no es valido."></asp:Label>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="lblTipoPagoRetencion" runat="server" CssClass="col-md-3 control-label">*Tipo de pago de la retención: </asp:Label>
                            <asp:DropDownList ID="ddlTipoPagoRetencion" runat="server" CssClass="form-control col-md-8"></asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvddlTipoPagoRetencion" runat="server" ControlToValidate="ddlTipoPagoRetencion" CssClass="text-danger" ValidationGroup="vgImpuestosRetenidos" Display="Dynamic" ErrorMessage="El tipo de pago de la retencion es obligatorio."></asp:RequiredFieldValidator>
                        </div>
                        <asp:Button ID="btnImpuestosRetenidos" text="Agregar impuesto" runat="server" ValidationGroup="vgImpuestosRetenidos" OnClick="btnImpuestosRetenidos_Click" />
                        <br />
                        <asp:GridView ID="grvImpRetenidos" runat="server" AutoGenerateColumns="False" BackColor="#CCCCCC" BorderColor="#999999" BorderStyle="Solid" BorderWidth="3px" CellPadding="4" CellSpacing="2" ForeColor="Black" Width="100%">
                            <Columns>
                                <asp:BoundField DataField="BaseRet" HeaderText=" BASE DEL IMPUESTO " ReadOnly="True" SortExpression="BaseRet" >
                                <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="impuetsto" HeaderText=" IMPUESTO " ReadOnly="true" SortExpression="impuetsto" >
                                <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="montoRet" HeaderText=" MONTO RETENIDO " ReadOnly="True" SortExpression="montoRet" >
                                <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                                <asp:BoundField DataField="TipoPagoRet" HeaderText=" TIPO DE PAGO " ReadOnly="True" SortExpression="TipoPagoRet" >
                                <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundField>
                            </Columns>
                            <FooterStyle BackColor="#CCCCCC" HorizontalAlign="Center"/>
                            <HeaderStyle BackColor="Black" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"/>
                            <PagerStyle BackColor="#CCCCCC" ForeColor="Black" HorizontalAlign="Center" />
                            <RowStyle BackColor="White" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                            <SortedAscendingCellStyle BackColor="#F1F1F1" />
                            <SortedAscendingHeaderStyle BackColor="#808080" HorizontalAlign="Center" />
                            <SortedDescendingCellStyle BackColor="#CAC9C9" />
                            <SortedDescendingHeaderStyle BackColor="#383838" />
                        </asp:GridView>
                        
                    </div>
                </div>
            </div>
        </div>
        <hr />
        <h4>Complemento: </h4>
        <div class="row" id="complemento">
            <div class="col-md-6">
                <asp:UpdatePanel ID="updComplementos" runat="server">
                    <ContentTemplate>
                        <div runat="server" id="divComplemento">
                            <div class="form-group">
                                <asp:Label ID="lblComplemento" runat="server" CssClass="col-md-3 control-label">Complemento: </asp:Label>
                                <asp:DropDownList ID="ddlComplemento" runat="server" CssClass="form-control col-md-8" AutoPostBack="true" OnSelectedIndexChanged="ddlComplemento_SelectedIndexChanged"></asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvddlComplemento" runat="server" ErrorMessage="El Complemento es obligatorio" ControlToValidate="ddlComplemento" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                            </div>
                            <div runat="server" id="divComplementoCuerpo">
                   <%-- Complemento Arrendamiento en Fideicomiso --%>
                                <div runat="server" id="divArrendamientoenFideicomiso">
                                    <div class="form-group">
                                        <asp:Label ID="lblPagProvEfecPorFiduc" runat="server" CssClass="col-md-3 control-label">*Importe del pago efectuado por parte del fiduciario al arrendador: </asp:Label>
                                        <asp:TextBox ID="txbPagProvEfecPorFiduc" runat="server" MaxLength="18" CssClass="form-control col-md-8"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvPagProvEfecPorFiduc" runat="server" ErrorMessage="El importe del pago es obligatorio" ControlToValidate="txbPagProvEfecPorFiduc" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidacionPagProvEfecPorFiduc" runat="server" Visible="false" Text="El importe del pago no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblRendimFideicom" runat="server" CssClass="col-md-3 control-label">*Importe de los rendimientos obtenidos en el periodo por el arrendamiento de bienes: </asp:Label>
                                        <asp:TextBox ID="txbRendimFideicom" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvRendimFideicom" runat="server" ErrorMessage="El importe de los rendimientos obtenidos es obligatorio" ControlToValidate="txbRendimFideicom" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidacionRendimFideicom" runat="server" Visible="false" Text="El importe de los rendimientos no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblDeduccCorresp" runat="server" CssClass="col-md-3 control-label">*Importe de las deducciones correspondientes al arrendamiento de los bienes: </asp:Label>
                                        <asp:TextBox ID="txbDeduccCorresp" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvDeduccCorresp" runat="server" ErrorMessage="El importe de las deducciones es obligatorio" ControlToValidate="txbDeduccCorresp" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidacionDeduccCorresp" runat="server" Visible="false" Text="El importe de las deducciones no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontTotRet" runat="server" CssClass="col-md-3 control-label">Monto total de la retención del arrendamiento de los bienes: </asp:Label>
                                        <asp:TextBox ID="txbMontTotRet" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidacionMontTotRet" runat="server" Visible="false" Text="El monto total de la retención no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontResFiscDistFibras" runat="server" CssClass="col-md-3 control-label">Monto del resultado fiscal distribuido por FIBRAS: </asp:Label>
                                        <asp:TextBox ID="txbMontResFiscDistFibras" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidacionMontResFiscDistFibras" runat="server" Visible="false" Text="El monto del resultado fiscal no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontOtrosConceptDistr" runat="server" CssClass="col-md-3 control-label">Monto de otros conceptos distribuidos: </asp:Label>
                                        <asp:TextBox ID="txbMontOtrosConceptDistr" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidacionMontOtrosConceptDistr" runat="server" Visible="false" Text="El monto de otros conceptos distribuidos no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblDescrMontOtrosConceptDistr" runat="server" CssClass="col-md-3 control-label">Descripción de los conceptos distribuidos: </asp:Label>
                                        <div class="col-md-10">
                                            <asp:TextBox ID="txbDescrMontOtrosConceptDistr" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="300"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                &nbsp;
                <%-- Complemento dividendos --%>
                                <div runat="server" id="divDividendos">
                                    <div class="form-group">
                                        <asp:Label ID="lblCveTipDivOUtil" runat="server" CssClass="col-md-3 control-label">*Tipo de dividendo: </asp:Label>
                                        <asp:DropDownList ID="ddlCveTipDivOUtil" runat="server" CssClass="form-control col-md-8"></asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvddlCveTipDivOUtil" runat="server" ErrorMessage="El tipo de dividendo es obligatorio" ControlToValidate="ddlCveTipDivOUtil" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontISRAcredRetMexico" runat="server" CssClass="col-md-3 control-label">*Retencion del dividendo en territorio nacional:</asp:Label>
                                        <asp:TextBox ID="txbMontISRAcredRetMexico" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbMontISRAcredRetMexico" runat="server" ErrorMessage="La retencion del dividendo en territorio nacional es obligatorio" ControlToValidate="txbMontISRAcredRetMexico" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbMontISRAcredRetMexico" runat="server" Visible="false" Text="La retencion del dividendo en territorio nacional no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontISRAcredRetExtranjero" runat="server" CssClass="col-md-3 control-label">*Retencion del dividendo en territorio extranjero: </asp:Label>
                                        <asp:TextBox ID="txbMontISRAcredRetExtranjero" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbMontISRAcredRetExtranjero" runat="server" ErrorMessage="La retencion del dividendo en territorio extranjero es obligatorio" ControlToValidate="txbMontISRAcredRetExtranjero" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbMontISRAcredRetExtranjero" runat="server" Visible="false" Text="La retencion del dividendo en territorio extranjero no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontRetExtDivExt" runat="server" CssClass="col-md-3 control-label">Monto de la retención en el extranjero sobre dividendos del extranjero :</asp:Label>
                                        <asp:TextBox ID="txbMontRetExtDivExt" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidaciontxbMontRetExtDivExt" runat="server" Visible="false" Text="El monto de la retención en el extranjero no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblTipoSocDistrDiv" runat="server" CssClass="col-md-3 control-label">*Sociedad de distribucion del dividendo: </asp:Label>
                                        <asp:DropDownList ID="ddlTipoSocDistrDiv" runat="server" CssClass="form-control col-md-8"></asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvddlTipoSocDistrDiv" runat="server" ErrorMessage="La Sociedad de distribucion del dividendo es obligatorio" ControlToValidate="ddlTipoSocDistrDiv" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontISRAcredNal" runat="server" CssClass="col-md-3 control-label">Monto del ISR acreditable nacional: </asp:Label>
                                        <asp:TextBox ID="txbMontISRAcredNal" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidaciontxbMontISRAcredNal" runat="server" Visible="false" Text="El monto del ISR acreditable nacional no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontDivAcumNal" runat="server" CssClass="col-md-3 control-label">Dividendo acumulable nacional: </asp:Label>
                                        <asp:TextBox ID="txbMontDivAcumNal" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidaciontxbMontDivAcumNal" runat="server" Visible="false" Text="El Dividendo acumulable nacional no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontDivAcumExt" runat="server" CssClass="col-md-3 control-label">Dividendo acumulable extranjero: </asp:Label>
                                        <asp:TextBox ID="txbMontDivAcumExt" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidaciontxbMontDivAcumExt" runat="server" Visible="false" Text="El Dividendo acumulable extranjero no es valido"></asp:Label>
                                    </div>
                                    <hr />
                                    <div class="form-group">
                                        <asp:Label ID="lblProporcionRem" runat="server" CssClass="col-md-3 control-label">Porcentaje de participación los accionistas: </asp:Label>
                                        <asp:TextBox ID="txbProporcionRem" runat="server" CssClass="form-control col-md-8"></asp:TextBox>
                                        <asp:Label ID="lblValidaciontxbProporcionRem" runat="server" Visible="false" Text="El porcentaje de participación los accionistas no es valido"></asp:Label>
                                    </div>
                                </div>
                                &nbsp;
                <%-- Complemento Enajenacion de Acciones --%>
                                <div runat="server" id="divEnajenaciondeAcciones">
                                    <div class="form-group">
                                        <asp:Label ID="lblContratoIntermediacion" runat="server" CssClass="col-md-3 control-label">*Contrato de intermediacion: </asp:Label>
                                        <asp:TextBox ID="txbContratoIntermediacion" runat="server" CssClass="form-control col-md-8" MaxLength="300"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbContratoIntermediacion" runat="server" ErrorMessage="El contrato de intermediacion es obligatorio" ControlToValidate="txbContratoIntermediacion" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblGanancia" runat="server" CssClass="col-md-3 control-label" >*Ganancia: </asp:Label>
                                        <asp:TextBox ID="txbGanancia" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbGanancia" runat="server" ErrorMessage="La ganancia es obligatoria" ControlToValidate="txbGanancia" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbGanancia" runat="server" Visible="false" Text="La ganancia no es valida"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblPerdida" runat="server" CssClass="col-md-3 control-label">*Perdida: </asp:Label>
                                        <asp:TextBox ID="txbPerdida" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbPerdida" runat="server" ErrorMessage="La perdida es obligatoria" ControlToValidate="txbPerdida" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbPerdida" runat="server" Visible="false" Text="La perdida no es valida"></asp:Label>
                                    </div>
                                </div>
                                &nbsp;
                <%-- Complemento Fideicomiso no Empresarial --%>
                                <div runat="server" id="divFideicomisoNoEmpresarial">
                                    <div runat="server" id="divIngresosOEntradas">
                                        <div class="form-group">
                                            <asp:Label ID="lblMontTotEntradasPeriodoIngresosOEntradas" runat="server" CssClass="col-md-3 control-label">*Importe total de los ingresos: </asp:Label>
                                            <asp:TextBox ID="txbMontTotEntradasPeriodoIngresosOEntradas" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvtxbMontTotEntradasPeriodoIngresosOEntradas" runat="server" ErrorMessage="El importe total de los ingresos es obligatorio" ControlToValidate="txbMontTotEntradasPeriodoIngresosOEntradas" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                            <asp:Label ID="lblValidaciontxbMontTotEntradasPeriodoIngresosOEntradas" runat="server" Visible="false" Text="El importe total de los ingresos no es valido"></asp:Label>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label ID="lblPartPropAcumDelFideicomIngresosOEntradas" runat="server" CssClass="col-md-3 control-label">*Parte proporcional de los ingresos acumulables: </asp:Label>
                                            <asp:TextBox ID="txbPartPropAcumDelFideicomIngresosOEntradas" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvtxbPartPropAcumDelFideicomIngresosOEntradas" runat="server" ErrorMessage="La Parte proporcional de los ingresos acumulables es obligatoria" ControlToValidate="txbPartPropAcumDelFideicomIngresosOEntradas" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                            <asp:Label ID="lblValidaciontxbPartPropAcumDelFideicomIngresosOEntradas" runat="server" Visible="false" Text="La Parte proporcional de los ingresos acumulables no es valida"></asp:Label>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label ID="lblPropDelMontTotIngresosOEntradas" runat="server" CssClass="col-md-3 control-label">*Proporción de participación del fideicomitente: </asp:Label>
                                            <asp:TextBox ID="txbPropDelMontTotIngresosOEntradas" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvtxbPropDelMontTotIngresosOEntradas" runat="server" ErrorMessage="La proporción de participación del fideicomitente es obligatoria" ControlToValidate="txbPropDelMontTotIngresosOEntradas" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                            <asp:Label ID="lblValidaciontxbPropDelMontTotIngresosOEntradas" runat="server" Visible="false" Text="La proporción de participación del fideicomitente no es valida"></asp:Label>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label ID="lblConceptoIngresosOEntradas" runat="server" CssClass="col-md-3 control-label">*Descripción del concepto de ingresos de los fideicomisos: </asp:Label>
                                            <asp:TextBox ID="txbConceptoIngresosOEntradas" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="100"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvtxbConceptoIngresosOEntradas" runat="server" ErrorMessage="La descripción del concepto es obligatorio" ControlToValidate="txbConceptoIngresosOEntradas" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div runat="server" id="divDeduccOSalidas">
                                        <div class="form-group">
                                            <asp:Label ID="lblMontTotEgresPeriodoDeduccOSalidas" runat="server" CssClass="col-md-3 control-label">*Importe total de los egresos: </asp:Label>
                                            <asp:TextBox ID="txbMontTotEgresPeriodoDeduccOSalidas" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvtxbMontTotEgresPeriodoDeduccOSalidas" runat="server" ErrorMessage="El importe total de los egresos es obligatorio" ControlToValidate="txbMontTotEgresPeriodoDeduccOSalidas" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                            <asp:Label ID="lblValidaciontxbMontTotEgresPeriodoDeduccOSalidas" runat="server" Visible="false" Text="El importe total de los egresos no es valido"></asp:Label>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label ID="lblPartPropDelFideicomDeduccOSalidas" runat="server" CssClass="col-md-3 control-label">*Parte proporcional de las deducciones autorizadas: </asp:Label>
                                            <asp:TextBox ID="txbPartPropDelFideicomDeduccOSalidas" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvtxbPartPropDelFideicomDeduccOSalidas" runat="server" ErrorMessage="La parte proporcional de las deducciones autorizadas es obligatoria" ControlToValidate="txbPropDelMontTotIngresosOEntradas" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                            <asp:Label ID="lblValidaciontxbPartPropDelFideicomDeduccOSalidas" runat="server" Visible="false" Text="La parte proporcional de las deducciones autorizadas no es valida"></asp:Label>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label ID="lblPropDelMontTotDeduccOSalidas" runat="server" CssClass="col-md-3 control-label">*Proporción de participación del fideicomitente: </asp:Label>
                                            <asp:TextBox ID="txbPropDelMontTotDeduccOSalidas" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvtxbPropDelMontTotDeduccOSalidas" runat="server" ErrorMessage="La proporción de participación del fideicomitente es obligatoria" ControlToValidate="txbPropDelMontTotIngresosOEntradas" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                            <asp:Label ID="lblValidaciontxbPropDelMontTotDeduccOSalidas" runat="server" Visible="false" Text="La proporción de participación del fideicomitente no es valida"></asp:Label>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label ID="lblConceptoDeduccOSalidas" runat="server">*Descripción del concepto de egresos de los fideicomisos: </asp:Label>
                                            <asp:TextBox ID="txbConceptoDeduccOSalidas" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="100"></asp:TextBox> 
                                            <asp:RequiredFieldValidator ID="rfvtxbConceptoDeduccOSalidas" runat="server" ErrorMessage="La Descripción del concepto de egresos de los fideicomisos es obligatoria" ControlToValidate="txbConceptoDeduccOSalidas" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                    <div runat="server" id="divRetEfectFideicomiso">
                                        <div class="form-group">
                                            <asp:Label ID="lblMontRetRelPagFideicRetEfectFideicomiso" runat="server">*Monto de las retenciones con relación al fideicomiso: </asp:Label>
                                            <asp:TextBox ID="txbMontRetRelPagFideicRetEfectFideicomiso" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvtxbMontRetRelPagFideicRetEfectFideicomiso" runat="server" ErrorMessage="El monto de las retenciones es obligatoria" ControlToValidate="txbMontRetRelPagFideicRetEfectFideicomiso" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                            <asp:Label ID="lblValidaciotxbMontRetRelPagFideicRetEfectFideicomiso" runat="server" Text="El monto de las retenciones no es valido"></asp:Label>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label ID="lblDescRetRelPagFideicRetEfectFideicomiso" runat="server">*Descripción de las retenciones: </asp:Label>
                                            <asp:TextBox ID="txbDescRetRelPagFideicRetEfectFideicomiso" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="100"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvtxbDescRetRelPagFideicRetEfectFideicomiso" runat="server" ErrorMessage="La descripción de las retenciones es obligatoria" ControlToValidate="txbDescRetRelPagFideicRetEfectFideicomiso" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        </div>
                                    </div>
                                </div>
                                &nbsp;
                <%-- Complemento Intereses --%>
                                <div runat="server" id="divIntereses">
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkSistFinanciero" runat="server" Text="Los intereses obtenidos en el periodo provienen del sistema financiero" />
                                    </div>
                                    <div class="form-group">
                                        <asp:CheckBox ID="chklRetiroAORESRetInt" runat="server" Text="Los intereses obtenidos fueron retirados en el periodo" />
                                    </div>
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkOperFinancDerivad" runat="server" Text="Los intereses obtenidos corresponden a operaciones financieras derivadas" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontIntNominal" runat="server" CssClass="col-md-3 control-label">*Importe del interés Nóminal: </asp:Label>
                                        <asp:TextBox ID="txbMontIntNominal" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbMontIntNominal" runat="server" ErrorMessage="El importe del interés Nóminal es obligatorio" ControlToValidate="txbMontIntNominal" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbMontIntNominal" runat="server" Visible="false" Text="El importe del interés Nóminal no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontIntReal" runat="server" CssClass="col-md-3 control-label">*Intereses reales: </asp:Label>
                                        <asp:TextBox ID="txbMontIntReal" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbMontIntReal" runat="server" ErrorMessage="Los intereses reales son obligatorios" ControlToValidate="txbMontIntReal" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbMontIntReal" runat="server" Visible="false" Text="Los intereses reales no son validos"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblPerdidaIntereses" runat="server" CssClass="col-md-3 control-label">*Pérdida por los intereses: </asp:Label>
                                        <asp:TextBox ID="txbPerdidaIntereses" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbPerdidaIntereses" runat="server" ErrorMessage="La pérdida por los intereses es obligatoria" ControlToValidate="txbPerdidaIntereses" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbPerdidaIntereses" runat="server" Visible="false" Text="La pérdida por los intereses no es valida"></asp:Label>
                                    </div>
                                </div>
                                &nbsp;
                <%-- Complemento Intereses Hipotecarios --%>
                                <div runat="server" id="divInteresesHipotecarios">
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkCreditoDeInstFinanc" runat="server" Text="El crédito otorgado fue por institución financiera"></asp:CheckBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblSaldoInsoluto" runat="server" CssClass="col-md-3 control-label">*Saldo insoluto al 31 de diciembre del ejercicio inmediato anterior: </asp:Label>
                                        <asp:TextBox ID="txbSaldoInsoluto" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbSaldoInsoluto" runat="server" ErrorMessage="El saldo insoluto al 31 de diciembre es obligatorio" ControlToValidate="txbSaldoInsoluto" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbSaldoInsoluto" runat="server" Visible="false" Text="El saldo insoluto al 31 de diciembre no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblPropDeducDelCredit" runat="server" CssClass="col-md-3 control-label">Proporción deducible del crédito aplicable sobre los intereses reales devengados y pagados: </asp:Label>
                                        <asp:TextBox ID="txbPropDeducDelCredit" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidaciontxbPropDeducDelCredit" runat="server" Visible="false" Text="La proporción deducible del crédito no es valida"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontTotIntNominalesDev" runat="server" CssClass="col-md-3 control-label">Monto total de intereses nominales devengados: </asp:Label>
                                        <asp:TextBox ID="txbMontTotIntNominalesDev" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidaciontxbMontTotIntNominalesDev" runat="server" Visible="false" Text="El monto total de intereses nominales devengados no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontTotIntNominalesDevYPag" runat="server" CssClass="col-md-3 control-label">Monto total de intereses nominales devengados y pagados: </asp:Label>
                                        <asp:TextBox ID="txbMontTotIntNominalesDevYPag" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidaciontxbMontTotIntNominalesDevYPag" runat="server" Visible="false" Text="El monto total de intereses nominales devengados y pagados no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontTotIntRealPagDeduc" runat="server" CssClass="col-md-3 control-label">Monto total de intereses reales pagados deducibles: </asp:Label>
                                        <asp:TextBox ID="txbMontTotIntRealPagDeduc" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidaciontxbMontTotIntRealPagDeduc" runat="server" Visible="false" Text="El monto total de intereses reales pagados deducibles no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblNumContrato" runat="server" CssClass="col-md-3 control-label">Número de contrato del crédito hipotecario: </asp:Label>
                                        <asp:TextBox ID="txbNumContrato" runat="server" CssClass="form-control col-md-8" MaxLength="300"></asp:TextBox>
                                    </div>
                                </div>
                                &nbsp;
                <%-- Complemento Operaciones con Derivados --%>
                                <div runat="server" id="divOperacionesconDerivados">
                                    <div class="form-group">
                                        <asp:Label ID="lblMontGanAcum" runat="server" CssClass="col-md-3 control-label">*Monto de la ganancia acumulable: </asp:Label>
                                        <asp:TextBox ID="txbMontGanAcum" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbMontGanAcum" runat="server" ErrorMessage="El monto de la ganancia acumulable es obligatorio" ControlToValidate="txbMontGanAcum" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbMontGanAcum" runat="server" Visible="false" Text="El monto de la ganancia acumulable no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontPerdDed" runat="server" CssClass="col-md-3 control-label">*Monto de la pérdida deducible: </asp:Label>
                                        <asp:TextBox ID="txbMontPerdDed" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbMontPerdDed" runat="server" ErrorMessage="El monto de la pérdida deducible es obligatorio" ControlToValidate="txbMontPerdDed" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbMontPerdDed" runat="server" Visible="false" Text="El monto de la pérdida deducible no es valido"></asp:Label>
                                    </div>
                                </div>
                                &nbsp;
                <%-- Complemento Pagos a Extranjeros --%>
                                <div runat="server" id="divPagosaExtranjeros">
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkEsBenefEfectDelCobro" runat="server" AutoPostBack="true" Text="El beneficiario del pago es la misma persona que retiene" OnCheckedChanged="chkEsBenefEfectDelCobro_CheckedChanged" />
                                    </div>
                                    <asp:UpdatePanel ID="updEsBenefEfectDelCobro" runat="server">
                                        <ContentTemplate>
                                            <div runat="server" id="divNoBeneficiario">
                                                <div class="form-group">
                                                    <asp:Label ID="lblPaisDeResidParaEfecFiscNoBeneficiario" runat="server" CssClass="col-md-3 control-label">*País de residencia del extranjero: </asp:Label>
                                                    <asp:DropDownList ID="ddlPaisDeResidParaEfecFiscNoBeneficiario" runat="server" CssClass="form-control col-md-8"></asp:DropDownList>
                                                    <asp:RequiredFieldValidator ID="rfvddlPaisDeResidParaEfecFiscNoBeneficiario" runat="server" ErrorMessage="El país de residencia del extranjero es obligatorio" ControlToValidate="ddlPaisDeResidParaEfecFiscNoBeneficiario" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                            <div runat="server" id="divBeneficiario">
                                                <div class="form-group">
                                                    <asp:Label ID="lblRFCBeneficiario" runat="server" CssClass="col-md-3 control-label">*RFC del representante legal en México</asp:Label>
                                                    <asp:TextBox ID="txbRFCBeneficiario" runat="server" CssClass="form-control col-md-8" MaxLength="13"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvtxbRFCBeneficiario" runat="server" ErrorMessage="El RFC del representante legal en México es obligatorio" ControlToValidate="txbRFCBeneficiario" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="revtxbRFCBeneficiario" runat="server" ControlToValidate="txbRFCBeneficiario" CssClass="text-danger" Display="Dynamic" ErrorMessage="El RFC del representante legal en México no es valido" ValidationExpression="[A-Z,Ñ,&amp;]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]?[A-Z,0-9]?[0-9,A-Z]?"></asp:RegularExpressionValidator>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label ID="lblCURPBeneficiario" runat="server" CssClass="col-md-3 control-label">*CURP del representante legal en México</asp:Label>
                                                    <asp:TextBox ID="txbCURPBeneficiario" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvtxbCURPBeneficiario" runat="server" ErrorMessage="El CURP del representante legal en México es obligatorio" ControlToValidate="txbCURPBeneficiario" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                                    <asp:RegularExpressionValidator ID="revtxbCURPBeneficiario" runat="server" ControlToValidate="txbCURPBeneficiario" CssClass="text-danger" Display="Dynamic" ErrorMessage="El CURP del representante legal en México no es valido" ValidationExpression="[A-Z][A,E,I,O,U,X][A-Z]{2}[0-9]{2}[0-1][0-9][0-3][0-9][M,H][A-Z]{2}[B,C,D,F,G,H,J,K,L,M,N,Ñ,P,Q,R,S,T,V,W,X,Y,Z]{3}[0-9,A-Z][0-9]"></asp:RegularExpressionValidator>
                                                </div>
                                                <div class="form-group">
                                                    <asp:Label ID="lblNomDenRazSocBeneficiario" runat="server" CssClass="col-md-3 control-label">*Razón social del contribuyente en México: </asp:Label>
                                                    <asp:TextBox ID="txbNomDenRazSocBeneficiario" runat="server" CssClass="form-control col-md-8" MaxLength="300"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvtxbNomDenRazSocBeneficiario" runat="server" ErrorMessage="La Razón social del contribuyente en México es obligatoria" ControlToValidate="txbNomDenRazSocBeneficiario" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="lblConceptoPagoNoBeneficiarioNoBeneficiario" runat="server" CssClass="col-md-3 control-label">*Tipo de contribuyente sujeto a la retención: </asp:Label>
                                                <asp:DropDownList ID="ddlConceptoPagoPagosaExtranjeros" runat="server" CssClass="form-control col-md-8"></asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvddlConceptoPagoPagosaExtranjeros" runat="server" ErrorMessage="El tipo de contribuyente sujeto a la retención es obligatorio" ControlToValidate="ddlConceptoPagoPagosaExtranjeros" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label ID="lblDescripcionConceptoNoBeneficiario" runat="server" CssClass="col-md-3 control-label">*Descripción de la definición del pago: </asp:Label>
                                                <asp:TextBox ID="tbxDescripcionConceptoPagoPagosaExtranjeros" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="255"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvtbxDescripcionConceptoPagoPagosaExtranjeros" runat="server" ErrorMessage="La descripción de la definición del pago es obligatoria" ControlToValidate="tbxDescripcionConceptoPagoPagosaExtranjeros" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                            </div>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="chkEsBenefEfectDelCobro" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>
                                &nbsp;
                <%-- Complemento Planes de Retiro --%>
                                <div runat="server" id="divPlanesdeRetiro">
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkSistemaFinanc" runat="server" Text="Los planes personales de retiro son del sistema financiero" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontTotAportAnioInmAnterior" runat="server" CssClass="col-md-3 control-label">Monto total de las aportaciones actualizadas en el año inmediato anterior: </asp:Label>
                                        <asp:TextBox ID="txbMontTotAportAnioInmAnterior" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidaciontxbMontTotAportAnioInmAnterior" runat="server" Visible="false" Text="El monto total de las aportaciones actualizadas no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontIntRealesDevengAniooInmAnt" runat="server" CssClass="col-md-3 control-label">*Monto de los intereses reales devengados o percibidos durante el año inmediato anterior: </asp:Label>
                                        <asp:TextBox ID="txbMontIntRealesDevengAniooInmAnt" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbMontIntRealesDevengAniooInmAnt" runat="server" ErrorMessage="El monto de los intereses reales devengados o percibidos es obligatorio" ControlToValidate="txbMontIntRealesDevengAniooInmAnt" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbMontIntRealesDevengAniooInmAnt" runat="server" Visible="false" Text="El monto de los intereses reales devengados o percibidos no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkHuboRetirosAnioInmAntPer" runat="server" Text="Se realizaron retiros de recursos invertidos y sus rendimientos en el ejercicio <br/> inmediato anterior antes de cumplir los requisitos de permanencia" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontTotRetiradoAnioInmAntPer" runat="server" CssClass="col-md-3 control-label">Monto total del retiro realizado antes de cumplir con los requisitos de permanencia: </asp:Label>
                                        <asp:TextBox ID="txbMontTotRetiradoAnioInmAntPer" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidaciontxbMontTotRetiradoAnioInmAntPer" runat="server" Visible="false" Text="El monto total del retiro realizado no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontTotExentRetiradoAnioInmAnt" runat="server" CssClass="col-md-3 control-label">Monto total exento del retiro realizado en el ejercicio inmediato anterior: </asp:Label>
                                        <asp:TextBox ID="txbMontTotExentRetiradoAnioInmAnt" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>                                         
                                        <asp:Label ID="lblValidaciontxbMontTotExentRetiradoAnioInmAnt" runat="server" Visible="false" Text="El monto total exento del retiro realizado no es valido"></asp:Label>
                                   </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontTotExedenteAnioInmAnt" runat="server" CssClass="col-md-3 control-label">Monto total excedente del monto exento del retiro realizado en el ejercicio inmediato anterior: </asp:Label>
                                        <asp:TextBox ID="txbMontTotExedenteAnioInmAnt" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidaciontxbMontTotExedenteAnioInmAnt" runat="server" Visible="false" Text="El monto total excedente del monto exento del retiro realizado no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:CheckBox ID="chkHuboRetirosAnioInmAnt" runat="server" Text="Se realizaron retiros en el ejercicio inmediato anterior" />
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontTotRetiradoAnioInmAnt" runat="server" CssClass="col-md-3 control-label">Monto total del retiro realizado en el ejercicio inmediato anterior:</asp:Label>
                                        <asp:TextBox ID="txbMontTotRetiradoAnioInmAnt" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:Label ID="lblValidaciontxbMontTotRetiradoAnioInmAnt" runat="server" Visible="false" Text="El Monto total del retiro realizado en el ejercicio inmediato anterior no es valido"></asp:Label>
                                    </div>
                                </div>
                                &nbsp;
                <%-- Complemento Premios --%>
                                <div runat="server" id="divPremios">
                                    <div class="form-group">
                                        <asp:Label ID="lblEntidadFederativaPremios" runat="server" CssClass="col-md-3 control-label">*Entidad federativa: </asp:Label>
                                        <asp:DropDownList ID="ddlEntidadFederativaPremios" runat="server" CssClass="form-control col-md-8"></asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="rfvddlEntidadFederativaPremios" runat="server" ErrorMessage="La entidad federativa es obligatoria" ControlToValidate="ddlEntidadFederativaPremios" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontTotPagoPremios" runat="server" CssClass="col-md-3 control-label">*Importe del pago por un premio</asp:Label>
                                        <asp:TextBox ID="txbMontTotPagoPremios" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbMontTotPagoPremios" runat="server" ErrorMessage="El importe del pago por un premio es obligatorio" ControlToValidate="txbMontTotPagoPremios" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbMontTotPagoPremios" runat="server" Visible="false" Text="El importe del pago por un premio no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontTotPagoGravPremios" runat="server" CssClass="col-md-3 control-label">*Importe gravado en la obtención de un premio: </asp:Label>
                                        <asp:TextBox ID="txbMontTotPagoGravPremios" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbMontTotPagoGravPremios" runat="server" ErrorMessage="El importe gravado en la obtención de un premio es obligatorio" ControlToValidate="txbMontTotPagoGravPremios" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbMontTotPagoGravPremios" runat="server" Visible="false" Text="El importe gravado en la obtención de un premio no es valido"></asp:Label>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblMontTotPagoExentPremios" runat="server" CssClass="col-md-3 control-label">*Monto total exento en la obtención de un premio: </asp:Label>
                                        <asp:TextBox ID="txbMontTotPagoExentPremios" runat="server" CssClass="form-control col-md-8" MaxLength="18"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbMontTotPagoExentPremios" runat="server" ErrorMessage="El monto total exento en la obtención de un premio es obligatorio" ControlToValidate="txbMontTotPagoExentPremios" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                        <asp:Label ID="lblValidaciontxbMontTotPagoExentPremios" runat="server" Visible="false" Text="El monto total exento en la obtención de un premio no es valido"></asp:Label>
                                    </div>
                                </div>
                                &nbsp;
                <%-- Complemento Sector Financiero --%>
                                <div runat="server" id="divSectorFinanciero">
                                    <div class="form-group">
                                        <asp:Label ID="lblIdFideicom" runat="server" CssClass="col-md-3 control-label">*Identificador o Número del Fideicomiso: </asp:Label>
                                        <asp:TextBox ID="txbIdFideicom" runat="server" CssClass="form-control col-md-8" MaxLength="20"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbIdFideicom" runat="server" ErrorMessage="El identificador o Número del Fideicomiso es obligatorio" ControlToValidate="txbIdFideicom" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblNomFideicom" runat="server" CssClass="col-md-3 control-label">Nombre del Fideicomiso: </asp:Label>
                                        <asp:TextBox ID="txbNomFideicom" runat="server" CssClass="form-control col-md-8" MaxLength="100"></asp:TextBox>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label ID="lblDescripFideicom" runat="server" CssClass="col-md-3 control-label">*Objeto o fin del Fideicomiso</asp:Label>
                                        <asp:TextBox ID="txbDescripFideicom" runat="server" CssClass="form-control" TextMode="MultiLine" MaxLength="300"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvtxbDescripFideicom" runat="server" ErrorMessage="El Objeto o fin del Fideicomiso es obligatorio" ControlToValidate="txbDescripFideicom" Display="Dynamic" CssClass="text-danger"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlComplemento" />
                    </Triggers>
                </asp:UpdatePanel>
                <div>
                    <asp:Button ID="btnGenerarCFDRIP" Text="Generar CFDRIP" runat="server" OnClick="btnGenerarCFDRIP_Click" />
                </div>
            </div>
        </div>
    </div>

</asp:Content>

