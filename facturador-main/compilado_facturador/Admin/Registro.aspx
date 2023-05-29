<%@ Page Title="Registro de tickets" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Registro.aspx.cs" Inherits="Facturador.GHO.Admin.Registro" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>
    <link href="../Content/jQuery.FileUpload/css/jquery.fileupload.css" rel="stylesheet" />
    <script src="../Scripts/jquery.ui.widget.js"></script>
    <script src="../Scripts/jQuery.FileUpload/jquery.fileupload.js"></script>


    <script type="text/javascript">
        $(document).ready(function () {
            var uploader = $('#fileupload').fileupload({
                dataType: 'json',
                url: '../Admin/UploadTicket.ashx',
                method: 'POST',
                autoUpload: false,
                change: function (e, data) {
                    $('.progress .progress-bar').css('width', 0 + '%');
                    $('.file_name').html('');
                    var names = new String();
                    $.each(data.files, function (index, file) {
                        names = names + ', ' + file.name;
                    });
                    $('.file_names').html(names.substring(2, 100) + '...');
                },
                add: function (e, data) {
                    $("#btnLoading").on('click', function () {
                        $('#btnLoading').button('loading');
                        $('.progress .progress-bar').css('width', 0 + '%');
                        data.submit();
                    })
                },
                done: function (e, data) {
                    $('.file_name').html(data.result.name);
                },
                stop: function (e) {
                    //alert(uploader.file.files.re);
                    //$(this).fileupload('destroy');
                    $('#btnLoading').button('reset');
                }
            }).on('fileuploadprogressall', function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                $('.progress .progress-bar').css('width', progress + '%');
            });
        });
    </script>


    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>

    <div class="form-horizontal">
        <h5>Seleccione los archivos de texto para ser procesados</h5>
        <hr />
        <div class="form-group">
            <span class="col-md-2 btn btn-success fileinput-button">
                <i class="glyphicon glyphicon-plus"></i>
                <span>Agregar archivos...</span>
                <input id="fileupload" type="file" name="files[]" multiple>
            </span>
            <div class="col-md-10 form-group">
                <label class="form-control file_names"></label>
            </div>
        </div>
        <div class="form-group progress">
            <div class="progress-bar" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%;">
                <span class="sr-only">0% complete</span>
            </div>
        </div>
        <div class="form-group file_name"></div>
        <div class="form-group">
            <button id="btnLoading" type="button" class="btn btn-primary" data-loading-text="Procesando...">Procesar tickets</button>
        </div>
    </div>
</asp:Content>

