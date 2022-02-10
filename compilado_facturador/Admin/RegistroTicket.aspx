<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RegistroTicket.aspx.cs" Inherits="Facturador.GHO.Admin.RegistroTicket" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=Button1.ClientID%>').click(function (evt) {
                var xhr = new XMLHttpRequest();
                var data = new FormData();
                var files = $('#<%=FileUpload1.ClientID%>').get(0).files;
                for (var i = 0; i < files.length; i++) {
                    data.append(files[i].name, files[i]);
                }
                xhr.upload.addEventListener("progress", function (evt) {
                    alert(progress + '%');
                    if (evt.lengthComputable) {
                        var progress = parseInt(data.loaded / data.total * 100, 10);
                        $('.progress .progress-bar').css('width', progress + '%');
                    }
                }, false);
                xhr.open("POST", "../Admin/UploadTicket.ashx");
                xhr.send(data);

                $("#progressbar").progressbar({
                    max: 100,
                    change: function (evt, ui) {
                        alert('%');
                        //$("#progresslabel").text($("#progressbar").progressbar("value") + "%");
                    },
                    complete: function (evt, ui) {
                        $("#progresslabel").text("File upload successful!");
                    }
                });
                evt.preventDefault();
            }.on('fileuploadprogressall', function (e, data) {
                alert('%');
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('.progress .progress-bar').css('width', progress + '%');
                $('#btnLoading').button('loading');
            }));
        });
    </script>
    <div class="form-group">
        <asp:Label ID="Label1" runat="server" Text="Select File(s) to Upload :"></asp:Label>
        <br />
        <br />
        <asp:FileUpload ID="FileUpload1" runat="server" AllowMultiple="true" />
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" Text="Upload" />
        <br />
        <br />
        <div class="progress">
            <div class="progress-bar" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;">
                <span class="sr-only">0% complete</span>
            </div>
        </div>
    </div>
</asp:Content>
