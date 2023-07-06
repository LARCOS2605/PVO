@ModelType PortalGeneral.ClientesViewModel
@Code
    ViewData("Title") = Idiomas.My.Resources.Resource.Tittle_CreateCliente
    ViewData("I_CU") = "class=active"
    ViewData("SubPage") = Idiomas.My.Resources.Resource.SubPage_Cliente
End Code
<style>

</style>

@Using Html.BeginForm(Nothing, Nothing, Nothing, FormMethod.Post, New With {.id = "formClientes"})
    @Html.AntiForgeryToken()
    @Html.ValidationSummary(True)

    @<div class="col-lg-12 col-ml-12">
        <div class="col-12 mt-5">
            <div class="card">
                <div class="card-body">
                    <h4 class="header-title">@Idiomas.My.Resources.Resource.Tittle_RegCliente</h4>
                    <p>@Idiomas.My.Resources.Resource.lbl_TipoCliente</p>

                    <div class="form-group">
                        @*Tipo de Cliente*@
                        <br>
                        <label class="col-form-label">Empresa</label>
                        <input type="radio" name="TipEmp" id="TipEmp" onclick="TipoCliente()">
                        <label class="col-form-label">Persona</label>
                        <input type="radio" name="TipPer" id="TipPer" onclick="TipoCliente()">
                    </div>
                    <input type="hidden" name="PersonaFisica" id="PersonaFisica" />
                    <input type="hidden" name="TipoPago" id="TipoPago" />
                    <div class="form-group" id="NomEmpresa" style="display:none">
                        @*Nombre de la Empresa*@
                        @Html.LabelFor(Function(model) model.Empresa, "*Razón Social:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.Empresa, New With {.class = "form-control", .id = "NombreEmpresa"})
                        @Html.ValidationMessageFor(Function(model) model.Empresa)
                    </div>
                    <div class="form-group" id="NomPersona" style="display:none">
                        @*Nombre*@
                        @Html.LabelFor(Function(model) model.Nombre, "*Nombre:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.Nombre, New With {.class = "form-control", .id = "NombrePersona"})
                        @Html.ValidationMessageFor(Function(model) model.Nombre)
                    </div>
                    <div class="form-group" id="ApePersona" style="display:none">
                        @*Apellido*@
                        @Html.LabelFor(Function(model) model.Apellido, "*Apellido:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.Apellido, New With {.class = "form-control", .id = "ApellidoPersona"})
                        @Html.ValidationMessageFor(Function(model) model.Apellido)
                    </div>
                    <div class="form-group">
                        @*Rfc*@
                        @Html.LabelFor(Function(model) model.Rfc, "*RFC:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.Rfc, New With {.class = "form-control", .id = "Rfc", .onblur = "ValidaRfcRepresentante(this.value)"})
                        @Html.ValidationMessageFor(Function(model) model.Rfc)
                    </div>
                    <div class="form-group">
                        @*Telefono*@
                        @Html.LabelFor(Function(model) model.Telefono, "Teléfono:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.Telefono, New With {.class = "form-control", .maxlength = "10"})
                        @Html.ValidationMessageFor(Function(model) model.Telefono)
                    </div>
                    <div class="form-group" id="Celular" style="display:none">
                        @*Celular*@
                        @Html.LabelFor(Function(model) model.Celular, "Celular:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.Celular, New With {.class = "form-control", .maxlength = "10"})
                        @Html.ValidationMessageFor(Function(model) model.Celular)
                    </div>
                    <!--<div class="form-group">-->
                    @*Nombre Alta SAT*@
                    <!--@Html.LabelFor(Function(model) model.NombreSat, "*Nombre Alta SAT:", New With {.class = "col-form-label"})
        @Html.TextBoxFor(Function(model) model.NombreSat, New With {.class = "form-control"})
        @Html.ValidationMessageFor(Function(model) model.NombreSat)
    </div>-->
                    <div class="form-group">
                        @*Regimen Fiscal*@
                        @Html.LabelFor(Function(model) model.RegimenFiscal, "*Régimen Fiscal SAT:", New With {.class = "col-form-label"})
                        @Html.DropDownList("RegimenFiscal", Nothing, Idiomas.My.Resources.Resource.cbb_Regimen, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.RegimenFiscal)
                    </div>
                    <p>@Idiomas.My.Resources.Resource.lbl_TipoPago</p>
                    <div class="form-group">
                        @*Tipo de Pago*@
                        <br>
                        <label class="col-form-label">Contado</label>
                        <input type="radio" name="TipContado" id="TipContado" onclick="TipodePago()">
                        <label class="col-form-label">Crédito</label>
                        <input type="radio" name="TipCredito" id="TipCredito" onclick="TipodePago()">
                    </div>
                    <div class="form-group">
                        @*Correo*@
                        @Html.LabelFor(Function(model) model.Correo, "*Correo Electrónico:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.Correo, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.Correo)
                    </div>

                    <!--<h4 class="header-title">@Idiomas.My.Resources.Resource.lbl_DatosCliente</h4>
    <div class="form-group">-->
                    @*Cp*@
                    <!--@Html.LabelFor(Function(model) model.CpDom, "*Código Postal:", New With {.class = "col-form-label"})
        @Html.TextBoxFor(Function(model) model.CpDom, New With {.class = "form-control"})
        @Html.ValidationMessageFor(Function(model) model.CpDom)
    </div>
    <button type="button" onclick="Domicilio(); coloniadom();" class="btn btn-success" data-toggle="tooltip">@Idiomas.My.Resources.Resource.btn_BuscarCp</button>
    <div class="form-group">-->
                    @*Pais*@
                    <!--@Html.LabelFor(Function(model) model.PaisDom, "*País:", New With {.Class = "col-form-label"})
        @Html.TextBoxFor(Function(model) model.PaisDom, New With {.class = "form-control", .readonly = "readonly"})
        @Html.ValidationMessageFor(Function(model) model.PaisDom)
    </div>
    <div class="form-group">-->
                    @*Estado*@
                    <!--@Html.LabelFor(Function(model) model.EstadoDom, "*Estado:", New With {.class = "col-form-label"})
        @Html.TextBoxFor(Function(model) model.EstadoDom, New With {.class = "form-control", .readonly = "readonly"})
        @Html.ValidationMessageFor(Function(model) model.EstadoDom)
    </div>
    <div class="form-group">-->
                    @*Ciudad y/o Municipio*@
                    <!--@Html.LabelFor(Function(model) model.MunicipioDom, "*Ciudad y/o Municipio:", New With {.class = "col-form-label"})
        @Html.TextBoxFor(Function(model) model.MunicipioDom, New With {.class = "form-control", .readonly = "readonly"})
        @Html.ValidationMessageFor(Function(model) model.MunicipioDom)
    </div>
    <div class="form-group">-->
                    @*Colonia*@
                    <!--@Html.LabelFor(Function(model) model.ColoniaDom, "*Colonia:", New With {.class = "col-form-label"})
        @Html.DropDownList("ColoniaDom", Nothing, Idiomas.My.Resources.Resource.cbb_Colonias, New With {.class = "form-control", .id = "Colonia"})
        @Html.ValidationMessageFor(Function(model) model.ColoniaDom)
    </div>
    <div class="form-group">-->
                    @*Calle*@
                    <!--@Html.LabelFor(Function(model) model.CalleDom, "*Calle:", New With {.class = "col-form-label"})
        @Html.TextBoxFor(Function(model) model.CalleDom, New With {.class = "form-control"})
        @Html.ValidationMessageFor(Function(model) model.CalleDom)
    </div>
    <div class="form-group">-->
                    @*Número Exterior*@
                    <!--@Html.LabelFor(Function(model) model.NumExtDom, "*Número Exterior:", New With {.class = "col-form-label"})
        @Html.TextBoxFor(Function(model) model.NumExtDom, New With {.class = "form-control"})
        @Html.ValidationMessageFor(Function(model) model.NumExtDom)
    </div>-->
                    <div class="form-group">
                        @*Mostrador Asignado*@
                        @Html.LabelFor(Function(model) model.Ubicacion, "*Mostrador Asignado:", New With {.class = "col-form-label"})
                        @Html.DropDownList("Ubicacion", Nothing, Idiomas.My.Resources.Resource.cbb_Ubicacion, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.Ubicacion)
                    </div>
                    <h4 class="header-title">@Idiomas.My.Resources.Resource.lbl_DatosCliente</h4>
                    <div class="form-group">
                        @*Cp Fiscal*@
                        @Html.LabelFor(Function(model) model.CpFis, "*Código Postal:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.CpFis, New With {.class = "form-control", .maxlength = "5"})
                        @Html.ValidationMessageFor(Function(model) model.CpFis)
                    </div>
                    <button type="button" onclick="DomicilioFiscal(); coloniaFis();" class="btn btn-success" data-toggle="tooltip">@Idiomas.My.Resources.Resource.btn_BuscarCp</button>
                    @If User.IsInRole("SuperAdmin") Or User.IsInRole("CxC") Then
                        @<button type = "button" onclick="NuevoDomicilio()" Class="btn btn-success" data-toggle="tooltip">Agregar C.P./Colonia no localizada</button>
                    End If
                    <div Class="form-group">
                        @*Pais Fiscal*@
                        @Html.LabelFor(Function(model) model.PaisFis, "*País:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.PaisFis, New With {.class = "form-control", .readonly = "readonly"})
                        @*@Html.DropDownList("Paises", Nothing, Idiomas.My.Resources.Resource.cbb_Pais, New With {.class = "form-control", .readonly = "readonly"})*@
                        @Html.ValidationMessageFor(Function(model) model.PaisFis)
                    </div>
                    <div class="form-group">
                        @*Estado Fiscal*@
                        @Html.LabelFor(Function(model) model.EstadoFis, "*Estado:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.EstadoFis, New With {.class = "form-control", .readonly = "readonly"})
                        @*@Html.DropDownList("Estados", Nothing, Idiomas.My.Resources.Resource.cbb_Estado, New With {.class = "form-control"})*@
                        @Html.ValidationMessageFor(Function(model) model.EstadoFis)
                    </div>
                    <div class="form-group" style="display:none">
                        @*Clave Estado Fiscal*@
                        @Html.LabelFor(Function(model) model.InternalId_Estado, "clave", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.InternalId_Estado, New With {.class = "form-control", .readonly = "readonly"})
                        @*@Html.DropDownList("Estados", Nothing, Idiomas.My.Resources.Resource.cbb_Estado, New With {.class = "form-control"})*@
                        @Html.ValidationMessageFor(Function(model) model.InternalId_Estado)
                    </div>
                    <div class="form-group">
                        @*Municipio Fiscal*@
                        @Html.LabelFor(Function(model) model.MunicipioFis, "Municipio:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.MunicipioFis, New With {.class = "form-control", .readonly = "readonly"})
                        @Html.ValidationMessageFor(Function(model) model.MunicipioFis)
                    </div>
                    <div class="form-group">
                        @*Ciudad Fiscal*@
                        @Html.LabelFor(Function(model) model.CiudadFis, "*Ciudad:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.CiudadFis, New With {.class = "form-control", .readonly = "readonly"})
                        @Html.ValidationMessageFor(Function(model) model.CiudadFis)
                    </div>
                    <div class="form-group">
                        @*Colonia Fiscal*@
                        @Html.LabelFor(Function(model) model.ColoniaFis, "Colonia:", New With {.class = "col-form-label"})
                        @Html.DropDownList("ColoniaFis", Nothing, Idiomas.My.Resources.Resource.cbb_Colonias, New With {.class = "form-control", .id = "ColoniaFis"})
                        @Html.ValidationMessageFor(Function(model) model.ColoniaFis)
                    </div>
                    <div class="form-group">
                        @*Calle Fiscal*@
                        @Html.LabelFor(Function(model) model.CalleFis, "*Calle:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.CalleFis, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.CalleFis)
                    </div>
                    <div class="form-group">
                        @*Numero Interior Fiscal*@
                        @Html.LabelFor(Function(model) model.NumIntFis, "Número Interior:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.NumIntFis, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.NumIntFis)
                    </div>
                    <div class="form-group">
                        @*Número Exterior Fiscal*@
                        @Html.LabelFor(Function(model) model.NumExtFis, "*Número Exterior:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.NumExtFis, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.NumExtFis)
                    </div>
                    <div class="form-group">
                        @*Piso*@
                        @Html.LabelFor(Function(model) model.Piso, "Piso:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.Piso, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.Piso)
                    </div>
                    <!--<div class="form-group">-->
                    @*Atencion*@
                    <!--@Html.LabelFor(Function(model) model.Atencion, "Atención:", New With {.class = "col-form-label"})
        @Html.TextBoxFor(Function(model) model.Atencion, New With {.class = "form-control"})
        @Html.ValidationMessageFor(Function(model) model.Atencion)
    </div>-->
                    <div class="form-group">
                        @*Destinatario*@
                        @Html.LabelFor(Function(model) model.Destinatario, "*Destinatario:", New With {.class = "col-form-label"})
                        @Html.TextBoxFor(Function(model) model.Destinatario, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.Destinatario)
                    </div>
                    <div class="form-group">
                        @*Comentarios*@
                        @Html.LabelFor(Function(model) model.Comentarios, "Comentarios:", New With {.class = "col-form-label"})
                        @Html.TextAreaFor(Function(model) model.Comentarios, New With {.class = "form-control"})
                        @Html.ValidationMessageFor(Function(model) model.Comentarios)
                    </div>
                    <!--<div class="form-group">-->
                    @*Direccion 1*@
                    <!--@Html.LabelFor(Function(model) model.Direc, "Dirección 1:", New With {.class = "col-form-label"})
        @Html.TextAreaFor(Function(model) model.Direc, New With {.class = "form-control"})
        @Html.ValidationMessageFor(Function(model) model.Direc)
    </div>-->
                    <p>@Idiomas.My.Resources.Resource.lbl_DatosObligatorios</p>
                    @*<h4 class="header-title">@Idiomas.My.Resources.Resource.lbl_DatosObligatorios</h4>*@
                </div>
                <div class="card-footer">
                    <div class="form-group">
                        <a onclick="Almacenar()" style="color: white;" class="btn btn-primary mt-4 pl-4 pr-4">@Idiomas.My.Resources.Resource.btn_GuardarCliente</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

End Using

<div style="display:none;">
    @Using Html.BeginForm("BuscarInformacionCliente", "Clientes", FormMethod.Post, New With {.id = "BuscarInformacion"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="CodigoPostal" id="create-CodigoPostal" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("BuscarInformacionFiscal", "Clientes", FormMethod.Post, New With {.id = "BuscarFiscal"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="CodigoPostalFis" id="create-CodigoPostal" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("BuscarInformacionColonia", "Clientes", FormMethod.Post, New With {.id = "BuscarColonia"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="CodigoPostalCol" id="create-CodigoPostal" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("BuscarInformacionColoniaFis", "Clientes", FormMethod.Post, New With {.id = "BuscarColoniaFis"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="CodigoPostalColFis" id="create-CodigoPostal" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("BuscarInformacionNueva", "Clientes", FormMethod.Post, New With {.id = "BuscarInformacionNueva"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="idEstado" id="idEstado" />

    End Using
</div>

<div style="display:none;">
    @Using Html.BeginForm("BuscarInformacionNuevoDomicilio", "Clientes", FormMethod.Post, New With {.id = "GuardarNuevoDomicilio"})
        @Html.AntiForgeryToken()

        @<input type="hidden" name="CodigoPostal" id="CodigoPostal" />
        @<input type="hidden" name="Pais" id="Pais" />
        @<input type="hidden" name="Estado" id="Estado" />
        @<input type="hidden" name="Municipio" id="Municipio" />
        @<input type="hidden" name="Colonia" id="Colonia" />

    End Using
</div>

<div class="modal fade bd-example-modal-lg modal-sl" id="ModalEdicion">
    <div class="modal-dialog modal-lg modal-sl">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Nuevo Domicilio</h5>
                <button type="button" class="close" data-dismiss="modal"><span>&times;</span></button>
            </div>
            <div class="modal-body" id="datos">
                <div class="form-group">
                    <div class="row">
                        <div class="col-lg-6">
                            <label for="input-cp" class="col-form-label">Código Postal:</label>
                            <input class="form-control" type="text" id="input-cp">
                        </div>
                        <div class="col-lg-6">
                            <label for="input-estado" class="col-form-label">Estado:</label>
                            <select id="Estados_Select" class="form-control" onchange="InfoNuevo()">
                                <option> --- Selecciona un Estado --- </option>
                                @For Each RegEstados As Catalogo_Estados In ViewBag.l_estados
                                    @<option  value="@RegEstados.idCatalogoEstados">@RegEstados.Descripcion</option>
                                Next
                            </select>
                        </div>
                        <div class="col-lg-6">
                            <label for="input-municipio" class="col-form-label">Municipio:</label>
                            <select id="Municipio_Select" class="form-control">
                                <option> --- Selecciona un Municipio --- </option>
                            </select>
                        </div>
                        <div class="col-lg-6">
                            <label for="input-colonia" class="col-form-label">Colonia:</label>
                            <input class="form-control" type="text" id="input-colonia">
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer">
                <button type="button" onclick="GuardarDomicilio()" class="btn btn-primary" data-dismiss="modal">Guardar</button>
                <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
            </div>
        </div>
    </div>
</div>

@Section Scripts

    <script>

            function NuevoDomicilio() {
                /*toastr.info("Nuevo Domicilio...");*/
                $('#input-cp').val("");
                $('#Estados_Select').val("");
                $('#Municipio_Select').empty();
                $('#input-colonia').val("");
                $('#ModalEdicion').modal('toggle');
        }

        function GuardarDomicilio() {
            try {

                var cp = $('#input-cp').val();
                var idPais = "151";
                var idEstado = $('#Estados_Select').val();
                var idMunicipio = $('#Municipio_Select').val();
                var Colonia = $('#input-colonia').val();

                if ($('#input-cp').val() == "") {
                    toastr.error('El Campo "Código Postal" no puede ir vacío.');
                    return false
                }
                if ($('#Estados_Select').val() == null) {
                    toastr.error('El Campo "Estado" no puede ir vacío.');
                    return false
                }
                if ($('#Municipio_Select').val() == null) {
                    toastr.error('El Campo "Municipio" no puede ir vacío.');
                    return false
                }
                if ($('#input-colonia').val() == "") {
                    toastr.error('El Campo "Colonia" no puede ir vacío.');
                    return false
                }

                //toastr.info('Buscando información...');
                var form = $("#GuardarNuevoDomicilio");

             
                form.children("[name='CodigoPostal']").val(cp);
                form.children("[name='Pais']").val(idPais);
                form.children("[name='Estado']").val(idEstado);
                form.children("[name='Municipio']").val(idMunicipio);
                form.children("[name='Colonia']").val(Colonia);
                var dataForm = form.serialize();

                $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                    if (data.Tipo == 1) {
                        toastr.info('Información Guardada Correctamente.');
                    } else {
                        toastr.error('Información no Guardada...');
                    }
                });


            } catch (error) {
                hideLoading();
            }
        }

        function InfoNuevo() {
            try {
                var estado = $('#Estados_Select').val();

                ; if (estado == "") {
                    toastr.error("Para buscar información, es necesario ingresar el estado");
                    $("#Municipio_Select").empty();
                    return false;
                }

                toastr.info('Buscando información...');
                var form = $("#BuscarInformacionNueva");

                $("#Municipio_Select").empty();

                form.children("[name='idEstado']").val(estado);
                var dataForm = form.serialize();

                $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                    if (data.Tipo == 1) {
                        if (data.Valor.length != 0) {

                            $.each(data.Valor, function (index, value) {
                                $("#Municipio_Select").append('<option value="' + value.Clave + '">' + value.Municipio + '</option>');
                            });
                        }
                        else {
                            toastr.error('No se pudo obtener la información, revise los parámetros de búsqueda...');
                        }
                    } else {
                        toastr.error('Información no localizada...');0
                    }
                });


            } catch (error) {
                hideLoading();
            }
        }

        function ValidaRfcRepresentante(cadena) {
            cadena = cadena.trim().replace(/\s/g, "");
            const rfcFisica = /^([A-ZÑ&]{4}) ?(?:- ?)?(\d{2}(?:0[1-9]|1[0-2])(?:0[1-9]|[12]\d|3[01])) ?(?:- ?)?([A-Z\d]{2})([A\d])$/;
            const rfcMoral = /^([A-ZÑ&]{3,4}) ?(?:- ?)?(\d{2}(?:0[1-9]|1[0-2])(?:0[1-9]|[12]\d|3[01])) ?(?:- ?)?([A-Z\d]{2})([A\d])$/;

            if (cadena.length > 0) {
                if (cadena.length == 13) {
                    var validado = cadena.match(rfcFisica);
                    if (!validado) {//Coincide con el formato general del regex?
                        $("#Rfc").val("");
                        alert("RFC no valido, vuelva a intentarlo.");
                        return false;
                    }
                    return true;
                } else if (cadena.length == 12) {
                    var validado = cadena.match(rfcMoral);
                    if (!validado) {//Coincide con el formato general del regex?
                        $("#Rfc").val("")
                        alert("RFC no valido, vuelva a intentarlo.");
                        return false;
                    }
                    return true;

                }
                else {
                    $("#Rfc").val("")
                    alert("El RFC del Representante Legal, no corresponde a un patrón de RFC de persona física.");
                    return false;
                }
            }
        }

        function TipodePago() {
            try {

                var pago = $("input[type=radio][name=TipContado]:checked").val(); //Pago
                var credito = $("input[type=radio][name=TipCredito]:checked").val(); //Credito


                if (pago) {
                    $("input[type=radio][name=TipCredito]").prop('checked', false);
                }

                if (credito) {
                    $("input[type=radio][name=TipContado]").prop('checked', false);
                }

            } catch (error) {
                hideLoading();
            }
        }

        function TipoCliente() {
            try {

                var value = $("input[type=radio][name=TipEmp]:checked").val();
                var value2 = $("input[type=radio][name=TipPer]:checked").val();

                if (value) {
                    $("#NomPersona").hide();
                    $("#ApePersona").hide();
                    $("#Celular").hide();
                    $("#NomEmpresa").show();
                    $("input[type=radio][name=TipPer]").prop('checked', false);
                }

                if (value2) {
                    $("#NomPersona").show();
                    $("#ApePersona").show();
                    $("#Celular").show();
                    $("#NomEmpresa").hide();
                    $("input[type=radio][name=TipEmp]").prop('checked', false);
                }

            } catch (error) {
                hideLoading();
            }
        }
       

        function coloniaFis() {
            try {
                var CP = $('#CpFis').val();

;                if (CP == "") {
                    toastr.error("Para buscar la colonia, es necesario ingresar el código postal");
                    $("#ColoniaFis").empty();
                    return false;
                }

                toastr.info('Buscando las Colonias...');
                var form = $("#BuscarColoniaFis");

                $("#ColoniaFis").empty();

                form.children("[name='CodigoPostalColFis']").val(CP);
                var dataForm = form.serialize();

                $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {

                    if (data.Tipo == 1) {
                        if (data.Valor.length != 0) {

                            $.each(data.Valor, function (index, value) {
                                $("#ColoniaFis").append('<option name="' + value.idColonia + '">' + value.idColonia + " - " + value.Colonia + '</option>');
                            });
                        }
                        else {
                            toastr.error('No se pudo obtener las colonias, revise los parámetros de búsqueda...');
                        }
                    } else {
                        toastr.error('Colonias no localizadas...');
                    }
                });


            } catch (error) {
                hideLoading();
            }
        }

        function coloniadom() {
            try {
                var CP = $('#CpDom').val();

                if (CP == "") {
                    toastr.error("Para buscar la colonia, es necesario ingresar el código postal");
                    $("#Colonia").empty();
                    return false;
                }

                toastr.info('Buscando las Colonias...');
                var form = $("#BuscarColonia");

                $("#Colonia").empty();

                form.children("[name='CodigoPostalCol']").val(CP);
                var dataForm = form.serialize();

                $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
                    console.log(data)
                    if (data.Tipo == 1) {
                        if (data.Valor.length != 0) {

                            $.each(data.Valor, function (index, value) {
                                $("#Colonia").append('<option name="' + value.idColonia + '">' + value.idColonia + " - " + value.Colonia + '</option>');
                            });
                        }
                        else {
                            toastr.error('No se pudo obtener las colonias, revise los parámetros de búsqueda...');
                        }
                    } else {
                        toastr.error('Colonias no localizadas...');
                    }
                });


            } catch (error) {
                hideLoading();
            }
        }


        function DomicilioFiscal() {
            try {
                var CP = $('#CpFis').val();

                if (CP == "") {
                    toastr.error("Para buscar el domicilio, es necesario ingresar el código postal");
                    $('#PaisFis').val("");
                    $('#EstadoFis').val("");
                    $('#MunicipioFis').val("");
                    $('#CiudadFis').val("");
                    return false;
                }

                toastr.info('Buscando Información...');
                var form = $("#BuscarFiscal");

                $('#PaisFis').val("");
                $('#EstadoFis').val("");
                $('#MunicipioFis').val("");
                $('#CiudadFis').val("");

                form.children("[name='CodigoPostalFis']").val(CP);
                var dataForm = form.serialize();

                $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
                    console.log(data)
                    if (data.Tipo == 1) {
                        if (data.Valor.length != 0) {

                            $.each(data.Valor, function (index, value) {
                                $('#PaisFis').val(value.idPais + " - " + value.Pais);
                                $('#InternalId_Estado').val(value.NsInternalEstado);
                                $('#EstadoFis').val(value.Estado);
                                $('#MunicipioFis').val(value.idMunicipio + " - " + value.Municipio);
                                $('#CiudadFis').val(value.idLocalidad + " - " + value.Localidad);

                                toastr.info('Información localizada...');
                            });
                        }
                        else {
                            toastr.error('No se pudo obtener el domicilio, revise los parámetros de búsqueda...');
                        }
                    } else {
                        toastr.error('Código Postal no localizado...');
                    }
                });


            } catch (error) {
                hideLoading();
            }
        }

        function Domicilio() {
            try {
                var CP = $('#CpDom').val();

                if (CP == "") {
                    toastr.error("Para buscar el domicilio, es necesario ingresar el código postal");
                    $('#PaisDom').val("");
                    $('#EstadoDom').val("");
                    $('#MunicipioDom').val("");
                    return false;
                }

                toastr.info('Buscando Información...');
                var form = $("#BuscarInformacion");

                $('#PaisDom').val("");
                $('#EstadoDom').val("");
                $('#MunicipioDom').val("");
                $("#Colonia").empty();

                form.children("[name='CodigoPostal']").val(CP);
                var dataForm = form.serialize();

                $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
                    console.log(data)
                    if (data.Tipo == 1) {
                        if (data.Valor.length != 0) {

                        $.each(data.Valor, function (index, value) {
                            $('#PaisDom').val(value.idPais + " - " + value.Pais);
                            $('#EstadoDom').val(value.Estado);
                            $('#MunicipioDom').val(value.idMunicipio + " - " + value.Municipio);

                                    toastr.info('Información localizada...');
                            });
                         }
                         else {
                            toastr.error('No se pudo obtener el domicilio, revise los parámetros de búsqueda...');
                        }
                    } else {
                        toastr.error('Código Postal no localizado...');
                    }
                });


            } catch (error) {
                hideLoading();
            }
        }


        $(document).ready(function () {

            $("#Ubicacion").chosen({
                width: "100%",
                placeholder_text_multiple: "--- Seleccione una Opción ---"
            });

            $("#Ubicacion").on('change', function (evt, params) {
                var selected = $(this).val();
                if (selected != null) {
                    if (selected.indexOf('29910406') >= 0) {
                        $(this).val('29910406').trigger("chosen:updated");
                    }
                }
            });

            $("#Ubicacion_Mostrador").hide();
            $("#Ubicacion_Reg_chosen").hide();
            $("#Mostradortext").hide();
        });

        function Almacenar() {


            var check1 = $('#TipEmp').prop("checked");
            var check2 = $('#TipPer').prop("checked");

            if ($("input[type=radio][name=TipContado]:checked").is(':checked')) {
                $('#TipoPago').val("true");

            }

            if ($("input[type=radio][name=TipCredito]:checked").is(':checked')) {
                $('#TipoPago').val("false");

            }

            if (check1 == false && check2 == false) {
                toastr.error('El Campo "Tipo de Cliente" no puede ir vacio.');
                return false
            }


            if ($("input[type=radio][name=TipEmp]:checked").is(':checked')) {
                $('#PersonaFisica').val("true");
                var dato1 = $('#NombreEmpresa').val();
                var c_letras_emp = dato1.length;
                if ($('#NombreEmpresa').val() == "") {
                    toastr.error('El Campo "Nombre de la Empresa" no puede ir vacio.');
                    return false
                }
                if (c_letras_emp < 4) {
                    toastr.error('El Campo "Nombre de la Empresa" debe contener un nombre valido.');
                    return false
                }
            }

            if ($("input[type=radio][name=TipPer]:checked").is(':checked')) {
                $('#PersonaFisica').val("false");
                var dato = $('#NombrePersona').val();
                var c_letras_per = dato.length;

                var dato_p = $('#ApellidoPersona').val();
                var c_letras_ape = dato_p.length;

                if ($('#NombrePersona').val() == "") {
                    toastr.error('El Campo "Nombre" no puede ir vacio.');
                    return false
                }

                if ($('#ApellidoPersona').val() == "") {
                    toastr.error('El Campo "Apellido" no puede ir vacio.');
                    return false
                }

                if (c_letras_per < 3) {
                    toastr.error('El Campo "Nombre" debe contener un nombre valido.');
                    return false
                }

                if (c_letras_ape < 3) {
                    toastr.error('El Campo "Apellido" debe contener un apellido valido.');
                    return false
                }
            }

            if ($('#Rfc').val() == "") {
                toastr.error('El Campo "RFC" no puede ir vacio.');
                return false
            }

            if ($('#NombreSat').val() == "") {
                toastr.error('El Campo "Nombre Alta SAT" no puede ir vacio.');
                return false
            }

            if ($('#RegimenFiscal').val() == "") {
                toastr.error('El Campo "Régimen Fiscal SAT" no puede ir vacio.');
                return false
            }

            if ($('#Correo').val() == "") {
                toastr.error('El Campo "Correo Electrónico" no puede ir vacio.');
                return false
            }

            //if ($('#PaisDom').val() == "") {
            //    toastr.error('El Campo "País" en el domicilio del cliente no puede ir vacio.');
            //    return false
            //}

            //if ($('#EstadoDom').val() == "") {
            //    toastr.error('El Campo "Estado" en el domicilio del cliente no puede ir vacio.');
            //    return false
            //}

            //if ($('#MunicipioDom').val() == "") {
            //    toastr.error('El Campo "Ciudad y/o Municipio" en el domicilio del cliente no puede ir vacio.');
            //    return false
            //}

            //if ($('#CalleDom').val() == "") {
            //    toastr.error('El Campo "Calle" en el domicilio del cliente no puede ir vacio.');
            //    return false
            //}

            //if ($('#NumExtDom').val() == "") {
            //    toastr.error('El Campo "Número Exterior" en el domicilio del cliente no puede ir vacio.');
            //    return false
            //}

            //if ($('#CpDom').val() == "") {
            //    toastr.error('El Campo "Código Postal" en el domicilio del cliente no puede ir vacio.');
            //    return false
            //}

            if ($('#Ubicacion').val() == "") {
                toastr.error('El Campo "Mostrador Asignado" en el domicilio del cliente no puede ir vacio.');
                return false
            }

            if ($('#PaisFis').val() == "") {
                toastr.error('El Campo "Ciudad y/o Municipio" en el domicilio del cliente no puede ir vacio.');
                return false
            }

            if ($('#Destinatario').val() == "") {
                toastr.error('El Campo "Destinatario" en el domicilio del cliente no puede ir vacio.');
                return false
            }

            if ($('#CalleFis').val() == "") {
                toastr.error('El Campo "Calle" en el domicilio del cliente no puede ir vacio.');
                return false
            }

            if ($('#NumExtFis').val() == "") {
                toastr.error('El Campo "Número Exterior" en el domicilio del cliente no puede ir vacio.');
                return false
            }

            if ($('#CpFis').val() == "") {
                toastr.error('El Campo "Código Fiscal" en el domicilio del cliente no puede ir vacio.');
                return false
            }

            if ($('#CiudadFis').val() == "") {
                toastr.error('El Campo "Ciudad" en el domicilio del cliente no puede ir vacio.');
                return false
            }

            if ($('#EstadoFis').val() == "") {
                toastr.error('El Campo "Estado" en el domicilio del cliente no puede ir vacio.');
                return false
            }

            if ($('input[name="l_Roles"][value*="ajero"]:checked').length > 0) {
                var Mostrador = $('#Ubicacion').val();

                if (Mostrador == "") {
                    toastr.error('No se puede registrar el cliente, debido a que no se ha seleccionado algun mostrador...');
                    return false
                }
            }

            if ($('input[name="l_Roles"][value*="endedor"]:checked').length > 0) {
                var Mostrador = $('#Ubicacion').val();

                if (Mostrador == "") {
                    toastr.error('No se puede registrar el cliente, debido a que no se ha seleccionado algun mostrador...');
                    return false
                }
            }

            toastr.info('Registrando Cliente...');
            $('#formClientes').submit();
        }





    </script>
End Section



