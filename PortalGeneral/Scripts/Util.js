function showLoading(message, element) {
    var loadingBody = '<div class="bubblingG"><span id="bubblingG_1"></span><span id="bubblingG_2"></span><span id="bubblingG_3"></span></div>';

    if (element) {
        if (message) {
            $(element).block({
                message: loadingBody + '</br><h3 id="text_load">' + message + '</h3>',
                css: { backgroundColor: 'initial', border: 'none', color: '#fff' }
            });
        } else {
            $(element).block({
                message: loadingBody,
                css: { backgroundColor: 'initial', border: 'none', color: '#fff' }
            });
        }
    }
    else {
        if(message){
            $.blockUI({
                message: loadingBody + '</br><h3>' + message + '</h3>',
                css: { backgroundColor: 'initial', border: 'none', color: '#fff' }
            });
        } else {
            $.blockUI({
                message: loadingBody,
                css: { backgroundColor: 'initial', border: 'none', color: '#fff' }
            });
        }
    }
}

function hideLoading(element) {
    if (element) {
        $(element).unblock();
    }
    else {
        $.unblockUI();
    }
}

function showAlert(message) {
    
    if(message){
        var alertHTML = '<div class="alert alert-warning fade in">' +
                        '<button class="close" data-dismiss="alert"><span>×</span></button>' +
                        '<strong>' + message + '</strong></div>';

        $("#page-alert").append(alertHTML);
    }
   
}

function solonumDecimales(evt) {
    var code = (evt.which) ? evt.which : evt.keyCode;
        if(code==8 || code==9 || code==37 || code==39 || code==46)
        {
            return true;
        }
        else if(code>=48 && code<=57)
        {
            //is a number
            return true;
        }
        else
        {
            return false;
        }
}

function soloNumeros(evt) {
    var code = (evt.which) ? evt.which : evt.keyCode;
    if (code == 8 || code == 9 || code == 37 || code == 39) {
        return true;
    }
    else if (code >= 48 && code <= 57) {
        //is a number
        return true;
    }
    else {
        return false;
    }
}

function isValidEmail(mail) {
    return /^\w+([\.\+\-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,4})+$/.test(mail);
}

function ValidaRfcRepresentante(cadena) {
    cadena = cadena.trim().replace(/\s/g, "");
    const rfcFisica = /^([A-ZÑ&]{4}) ?(?:- ?)?(\d{2}(?:0[1-9]|1[0-2])(?:0[1-9]|[12]\d|3[01])) ?(?:- ?)?([A-Z\d]{2})([A\d])$/;
    const curps = /^([A-Z][AEIOUX][A-Z]{2}\d{2}(?:0[1-9]|1[0-2])(?:0[1-9]|[12]\d|3[01])[HM](?:AS|B[CS]|C[CLMSH]|D[FG]|G[TR]|HG|JC|M[CNS]|N[ETL]|OC|PL|Q[TR]|S[PLR]|T[CSL]|VZ|YN|ZS)[B-DF-HJ-NP-TV-Z]{3}[A-Z\d])(\d)$/;

    if (cadena.length > 0) {
        if (cadena.length == 13) {
            var validado = cadena.match(rfcFisica);
            if (!validado) {//Coincide con el formato general del regex?
                alert(msg_rfcIncorrecto);
                return false;
            }
            return true;
        } else if (cadena.length == 18) {
            var validado = cadena.match(curps);
            if (!validado) {//Coincide con el formato general del regex?
                alert(msg_rfcIncorrecto);
                return false;
            }
            return true;

        }
        else {
            $('#RFC').val("");
            toastr.error("El RFC del Representante Legal, no corresponde a un patrón de RFC correcto.");
            return false;
        }
    }
}

function AlmacenarMostrador() {

    var url = window.location.pathname;
    var form = $("#ActualizarMostrador");
    var id = $('#Mostradores_Select option:selected').val();

    if ((id == "--- Seleccione un Mostrador ---") || (id == "")) {
        toastr.error('El Mostrador seleccionado es invalido...');
        return false;
    }

    if (url == "/OrdenesVenta/CrearOrdenDeVenta") {
        toastr.error('No se puede cambiar de mostrador en el contexto actual...');
        return false;
    }

    form.children("[name='id']").val(id);
    var dataForm = form.serialize();

    $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
        if (data.Tipo == 1) {
            $('#title_M').text("Punto de Ventas : Mostrador " + id);
            toastr.success('El mostrador seleccionado ha sido guardado con exito...');

            /*$('#ModalSelection').modal('toggle');*/
        }
        else {
            toastr.error('Hubo un problema al guardar el mostrador, Intentelo más tarde...');
        }
    });

}


function AlmacenarMostradorInicial() {

    var url = window.location.pathname;
    var form = $("#ActualizarMostrador");
    var id = $('#Mostradores_Select option:selected').val();

    if ((id == "--- Seleccione un Mostrador ---") || (id == "")) {
        toastr.error('El Mostrador seleccionado es invalido...');
        return false;
    }

    if (url == "/OrdenesVenta/CrearOrdenDeVenta") {
        toastr.error('No se puede cambiar de mostrador en el contexto actual...');
        return false;
    }

    form.children("[name='id']").val(id);
    var dataForm = form.serialize();

    $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
        if (data.Tipo == 1) {
            $('#title_M').text("Punto de Ventas : Mostrador " + id);
            toastr.success('El mostrador seleccionado ha sido guardado con exito...');

            $('#ModalSelection').modal('toggle');
        }
        else {
            toastr.error('Hubo un problema al guardar el mostrador, Intentelo más tarde...');
        }
    });

}


function ChangeMostradorConfig() {

    var url = window.location.pathname;
    var form = $("#ActualizarMostrador");
    var id = $('#Mostradores_Select_config option:selected').text();

    if ((id == "--- Seleccione un Mostrador ---") || (id == "")) {
        toastr.error('El Mostrador seleccionado es invalido...');
        return false;
    }

    if (url == "/OrdenesVenta/CrearOrdenDeVenta") {
        toastr.error('No se puede cambiar de mostrador en el contexto actual...');
        return false;
    }

    form.children("[name='id']").val(id);
    var dataForm = form.serialize();

    $.post(form.attr("action"), dataForm, function (data, textStatus, xhr) {
        if (data.Tipo == 1) {
            $('#title_M').text("Punto de Ventas : Mostrador " + id);
            toastr.success('El mostrador seleccionado ha sido guardado con exito...');
        }
        else {
            toastr.error('Hubo un problema al guardar el mostrador, Intentelo más tarde...');
        }
    });

}

function DisableBack() {
    window.history.forward();
}