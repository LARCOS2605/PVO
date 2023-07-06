Imports System.Data.Entity
Imports System.Threading.Tasks
Imports AutoMapper

Public Class CapaCorreosController
    Inherits System.Web.Mvc.Controller

#Region "Constructores"
    Public db As New PVO_NetsuiteEntities
    Public AutoMap As New AutoMappeo
    Public Consulta As New ConsultasController
#End Region

    Public Async Function GenerarCorreoInvitacionUsuarios(ByVal nombre As String, ByVal VerificationToken As String, ByVal correo As String, ByVal contraseña As String) As Task(Of List(Of String))

        Dim parametros As Dictionary(Of String, String) = Await Consulta.ConstruirDiccionarioParametros()
        Dim protocol
        Dim cuerpo As New List(Of String)
        Dim requestAutority = ""

        If Not IsNothing(Request) Then
            protocol = Request.Url.Scheme
        Else
            protocol = "https"
        End If

        requestAutority = "187.217.160.254:1900"

        Dim urlBase = protocol + "://" + requestAutority

        Dim linkVericationAccount = urlBase + "/Account/ActivateAccount/" + VerificationToken

#Region "Construccion Correo"
        'cuerpo.Add(String.Format("<!DOCTYPE html> <html><body>"))

        ''Redaccion y Cuerpo
        cuerpo.Add(String.Format("<body style='width:100%;font-family:Open Sans, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0'> 
  <div class='es-wrapper-color' style='background-color:#EFF2F7'> 
   <!--[if gte mso 9]>
			<v:background xmlns:v='urn:schemas-microsoft-com:vml' fill='t'>
				<v:fill type='tile' color='#eff2f7'></v:fill>
			</v:background>
		<![endif]--> 
   <table class='es-wrapper' width='100%' cellspacing='0' cellpadding='0' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top'> 
     <tr style='border-collapse:collapse'> 
      <td valign='top' style='padding:0;Margin:0'> 
       <table class='es-header' cellspacing='0' cellpadding='0' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:#052358;background-repeat:repeat;background-position:center top'> 
         <tr style='border-collapse:collapse'> 
          <td align='center' bgcolor='transparent' style='padding:0;Margin:0;background-color:transparent'> 
           <table class='es-header-body' cellspacing='0' cellpadding='0' bgcolor='#1f4995' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#1f4995;width:600px'> 
             <tr style='border-collapse:collapse'> 
              <td align='left' style='Margin:0;padding-left:15px;padding-right:15px;padding-top:20px;padding-bottom:20px'> 
               <table cellspacing='0' cellpadding='0' width='100%' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                 <tr style='border-collapse:collapse'> 
                  <td class='es-m-p0r' valign='top' align='center' style='padding:0;Margin:0;width:570px'> 
                   <table width='100%' cellspacing='0' cellpadding='0' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' class='es-m-txt-c' style='padding:0;Margin:0;font-size:0px'><a target='_blank' href='https://www.muellesmaf.com.mx/' style='-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:none;color:#FFFFFF;font-size:12px'><img src='https://i0.wp.com/www.muellesmaf.com.mx/wp-content/uploads/2022/07/cropped-Logo-maf-solo.png?w=513&ssl=1' alt style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic' height='100'></a></td> 
                     </tr> 
                   </table></td> 
                 </tr> 
               </table></td> 
             </tr> 
           </table></td> 
         </tr> 
       </table> 
       <table class='es-content' cellspacing='0' cellpadding='0' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%'> 
         <tr style='border-collapse:collapse'> 
          <td align='center' style='padding:0;Margin:0'> 
           <table class='es-content-body' cellspacing='0' cellpadding='0' bgcolor='#fefefe' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FEFEFE;width:600px'> 
             <tr style='border-collapse:collapse'> 
              <td align='left' style='Margin:0;padding-left:15px;padding-right:15px;padding-top:40px;padding-bottom:40px'> 
               <table cellpadding='0' cellspacing='0' width='100%' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                 <tr style='border-collapse:collapse'> 
                  <td align='center' valign='top' style='padding:0;Margin:0;width:570px'> 
                   <table cellpadding='0' cellspacing='0' width='100%' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:0;Margin:0'><h1 style='Margin:0;line-height:31px;mso-line-height-rule:exactly;font-family:roboto,helvetica neue, helvetica, arial, sans-serif;font-size:26px;font-style:normal;font-weight:bold;color:#3C4858'>Hola, " + nombre + "</h1></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td style='padding:0;Margin:0;padding-top:10px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:21px;color:#8492A6;font-size:14px;text-align:justify'>Se ha dado de alta una cuenta con esta dirección de correo en el Portal PVO MAF.<br><br><strong>Para poder activar su cuenta, es necesario dar click en la siguiente opción:</strong><br><br></p></td>
                     </tr> 
                     
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:0;Margin:0'><span class='es-button-border' style='border-style:solid;border-color:#0C66FF;background:#0C66FF;border-width:0px;display:inline-block;border-radius:0px;width:auto'><a href='" + linkVericationAccount + "' class='es-button es-button-1' target='_blank' style='mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;color:#FFFFFF;font-size:14px;border-style:solid;border-color:#0C66FF;border-width:15px 30px;display:inline-block;background:#0C66FF;border-radius:0px;font-family:Open Sans, sans-serif;font-weight:bold;font-style:normal;line-height:17px;width:auto;text-align:center'>Activar Cuenta</a></span></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td style='padding:0;Margin:0;padding-top:10px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:21px;color:#8492A6;font-size:14px;text-align:justify'><br><strong>Si el enlace no funciona al hacer clic en él, puede copiarlo en la ventana de su navegador o teclearlo directamente.</strong></p></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:20px;Margin:0;font-size:0'> 
                       <table border='0' width='100%' height='100%' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                         <tr style='border-collapse:collapse'> 
                          <td style='padding:0;Margin:0;border-bottom:2px solid #0c66ff;background:none;height:1px;width:100%;margin:0px'></td> 
                         </tr> 
                       </table></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:0;Margin:0;padding-top:10px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:21px;color:#0c66ff;font-size:14px'>" + linkVericationAccount + "</p></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:20px;Margin:0;font-size:0'> 
                       <table border='0' width='100%' height='100%' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                         <tr style='border-collapse:collapse'> 
                          <td style='padding:0;Margin:0;border-bottom:2px solid #0c66ff;background:none;height:1px;width:100%;margin:0px'></td> 
                         </tr> 
                       </table></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:0;Margin:0;padding-top:10px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:21px;color:#8492A6;font-size:14px;text-align:left'>Una vez que haya activado su cuenta, podrá ingresar utilizando los siguientes datos:</p></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:0;Margin:0;padding-top:10px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:21px;color:#8492A6;font-size:14px'><br></p></td> 
                     </tr> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:20px;Margin:0;font-size:0'> 
                       <table border='0' width='100%' height='100%' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                         <tr style='border-collapse:collapse'> 
                          <td style='padding:0;Margin:0;border-bottom:2px solid #0c66ff;background:none;height:1px;width:100%;margin:0px'></td> 
                         </tr> 
                       </table></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:0;Margin:0;padding-top:10px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:21px;color:#8492A6;font-size:14px'><b>Correo Electrónico:</b> &nbsp;" + correo + "&nbsp;<br><b>Contraseña:&nbsp;</b> " + contraseña + "<br></p></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' style='padding:20px;Margin:0;font-size:0'> 
                       <table border='0' width='100%' height='100%' cellpadding='0' cellspacing='0' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                         <tr style='border-collapse:collapse'> 
                          <td style='padding:0;Margin:0;border-bottom:2px solid #0c66ff;background:none;height:1px;width:100%;margin:0px'></td> 
                         </tr> 
                       </table></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td style='padding:0;Margin:0;padding-top:10px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:21px;color:#8492A6;font-size:14px;text-align:justify'><br><strong>Es importante que active su cuenta antes de {0} días, después de ese periodo el enlacé será inválido y tendrá que solicitar que le sea enviado un nuevo enlace.</strong></p></td> 
                     </tr> 
                   </table></td> 
                 </tr> 
               </table></td> 
             </tr> 
           </table></td> 
         </tr> 
       </table> 
       <table cellpadding='0' cellspacing='0' class='es-footer' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;background-color:#141B24;background-repeat:repeat;background-position:center top'> 
         <tr style='border-collapse:collapse'> 
          <td align='center' style='padding:0;Margin:0'> 
           <table class='es-footer-body' cellspacing='0' cellpadding='0' bgcolor='#ffffff' align='center' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#273444;width:600px'> 
             <tr style='border-collapse:collapse'> 
              <td align='left' style='Margin:0;padding-left:15px;padding-right:15px;padding-top:40px;padding-bottom:40px'> 
               <!--[if mso]><table style='width:570px' cellpadding='0' 
                        cellspacing='0'><tr><td style='width:180px' valign='top'><![endif]--> 
               <table class='es-left' cellspacing='0' cellpadding='0' align='left' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:left'> 
                 <tr style='border-collapse:collapse'> 
                  <td class='es-m-p20b' align='left' style='padding:0;Margin:0;width:180px'> 
                   <table width='100%' cellspacing='0' cellpadding='0' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' class='es-m-txt-c' style='padding:0;Margin:0;font-size:0px'><a target='_blank' href='https://www.muellesmaf.com.mx/' style='-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;text-decoration:none;color:#FFFFFF;font-size:12px'><img src='https://i0.wp.com/www.muellesmaf.com.mx/wp-content/uploads/2022/07/cropped-Logo-maf-solo.png?w=513&ssl=1' alt style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic' width='70'></a></td> 
                     </tr> 
                     <tr style='border-collapse:collapse'> 
                      <td align='center' class='es-m-txt-c' style='padding:0;Margin:0;padding-top:20px;font-size:0'> 
                       <table cellpadding='0' cellspacing='0' class='es-table-not-adapt es-social' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                         <tr style='border-collapse:collapse'> 
                          <td align='center' valign='top' style='padding:0;Margin:0;padding-right:10px'><img src='https://rvstos.stripocdn.email/content/assets/img/social-icons/logo-white/facebook-logo-white.png' alt='Fb' title='Facebook' width='32' style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic'></td> 
                          <td align='center' valign='top' style='padding:0;Margin:0;padding-right:10px'><img src='https://rvstos.stripocdn.email/content/assets/img/social-icons/logo-white/twitter-logo-white.png' alt='Tw' title='Twitter' width='32' style='display:block;border:0;outline:none;text-decoration:none;-ms-interpolation-mode:bicubic'></td> 
                         </tr> 
                       </table></td> 
                     </tr> 
                   </table></td> 
                 </tr> 
               </table> 
               <!--[if mso]></td><td style='width:20px'></td><td style='width:370px' valign='top'><![endif]--> 
               <table class='es-right' cellspacing='0' cellpadding='0' align='right' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;float:right'> 
                 <tr style='border-collapse:collapse'> 
                  <td align='left' style='padding:0;Margin:0;width:370px'> 
                   <table width='100%' cellspacing='0' cellpadding='0' role='presentation' style='mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px'> 
                     <tr style='border-collapse:collapse'> 
                      <td align='left' class='es-m-txt-c' style='padding:0;Margin:0;padding-top:20px;padding-bottom:20px'><p style='Margin:0;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:Open Sans, sans-serif;line-height:18px;color:#8492A6;font-size:12px'>Contacto - 5565-1580 & 3000-2999
                        <br>Antonio M. Rivera Num. 7 Centro industrial Tlalnepantla Estado de México C.P. 54030</p></td> 
                     </tr> 
                   </table></td> 
                 </tr> 
               </table> 
               <!--[if mso]></td></tr></table><![endif]--></td> 
             </tr> 
           </table></td> 
         </tr> 
       </table></td> 
     </tr> 
   </table> 
  </div>  
 </body>", parametros("DIAS_ACTIVACION_CUENTA")))

        Return cuerpo

#End Region

    End Function

End Class
