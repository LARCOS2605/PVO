using MyApplication;
using NetsuiteLibrary.Clases;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace NetsuiteLibrary.SuiteServlet_Connection
{
    public class ConnectionServer
    {
        public RespuestaServicios ConnectionServiceServlet(EntregaMercancia Model)
        {
            try
            {
                RespuestaServicios Respuesta = new RespuestaServicios();
                String urlString = ConfigurationManager.AppSettings["URL_Servlet"];
                String ckey = ConfigurationManager.AppSettings["ConsumerKey"];                  
                String csecret = ConfigurationManager.AppSettings["ConsumerSecret"];
                String tkey = ConfigurationManager.AppSettings["TokenId"];
                String tsecret = ConfigurationManager.AppSettings["TokenSecret"];
                String netsuiteAccount = ConfigurationManager.AppSettings["AccountID"];
                String Algorithm = ConfigurationManager.AppSettings["Algorithm"];
                long TimeOut = 100000000000;

                Uri url = new Uri(urlString);
                OAuthBase req = new OAuthBase();
                String timestamp = req.GenerateTimeStamp();
                String nonce = req.GenerateNonce();
                String norm = "";
                String norm1 = "";
                String signature = req.GenerateSignature(url, ckey, csecret, tkey, tsecret, "POST", timestamp, nonce, out norm, out norm1);

                if (signature.Contains("+"))
                {
                    signature = signature.Replace("+", "%2B");
                }

                String header = "Authorization: OAuth ";
                header += "oauth_signature=\"" + signature + "\",";
                header += "oauth_version=\"1.0\",";
                header += "oauth_nonce=\"" + nonce + "\",";
                header += "oauth_signature_method=\"" + Algorithm + "\",";
                header += "oauth_consumer_key=\"" + ckey + "\",";
                header += "oauth_token=\"" + tkey + "\",";
                header += "oauth_timestamp=\"" + timestamp + "\",";
                header += "realm=\"" + netsuiteAccount + "\"";
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(urlString);
                request.Timeout = (int) TimeOut;

                request.ContentType = "application/json";
                request.Method = "POST";
                request.Headers.Add(header);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    //Datos Cabecera
                    string json = "{\"operation\":\"" + "insertItemFulfillment" + "\",";
                    json += "\"records\":[{\"referenceRecId\":\"" + Model.NS_InternalID_Doc + "\",";
                    json += "\"memo\":\""+ Model.Memo + "\",";
                    json += "\"shipstatus\":\"" + "C" + "\",\"sublists\":[{\"itemList\":[";

                    //Validamos el total de registros
                    DetalleEntregaMercancia Itemlast = Model.l_Detalle.Last();
                    foreach (DetalleEntregaMercancia RegMerc in Model.l_Detalle)
                    {

                        if (RegMerc.TipoMaterial == "Assembly")
                        {
                            if (RegMerc.Equals(Itemlast))
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + ",\"inventorydetail\":[{\"issueinventorynumber\":\"" + RegMerc.IssuesInventoryID + "\",\"quantity\":" + RegMerc.Cantidad + "}]}";
                            }
                            else
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + ",\"inventorydetail\":[{\"issueinventorynumber\":\"" + RegMerc.IssuesInventoryID + "\",\"quantity\":" + RegMerc.Cantidad + "}]},";
                            }
                        } else if (RegMerc.TipoMaterial == "Inventory")
                        {
                            if (RegMerc.Equals(Itemlast))
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + "}";
                            }
                            else
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + "},";
                            }
                        }
                        else if (RegMerc.TipoMaterial == "Assembly_nL")
                        {
                            if (RegMerc.Equals(Itemlast))
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + "}";
                            }
                            else
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + "},";
                            }
                        }
                        else if (RegMerc.TipoMaterial == "Generic")
                        {
                            if (RegMerc.Equals(Itemlast))
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + "}";
                            }
                            else
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + "},";
                            }
                        }


                    }

                    json += "]}]}]}";

                    streamWriter.Write(json);
                }


                WebResponse response = request.GetResponse();
                HttpWebResponse httpResponse = (HttpWebResponse)response;
                Stream resStream = httpResponse.GetResponseStream();
                StreamReader sr = new StreamReader(resStream);
                var result = sr.ReadToEnd();
                var ResultObject = Newtonsoft.Json.JsonConvert.DeserializeObject((string)result);
                string GetResultado = ResultObject.ToString();       
                var ConvertResultado = JsonConvert.DeserializeObject<JsonRespuestaGeneral>(GetResultado);

                if (ConvertResultado.code == "200")
                {
                    Respuesta.Estatus = "Exito";
                    Respuesta.Mensaje = "Se ha creado la ejecución de pedido con exito";
                } else
                {
                    Respuesta.Estatus = "Fracaso";
                    Respuesta.Mensaje = string.Format("Hubo un problema al crear la ejecución de pedido. Detalles: {0}", ConvertResultado.message);
                }

                
                return Respuesta;
            } catch (Exception ex){
                RespuestaServicios Respuesta = new RespuestaServicios();
                Respuesta.Estatus = "Freacaso";
                Respuesta.Mensaje = ex.Message;
                return Respuesta;
            }  
        }

        public RespuestaServicios ConnectionServiceServletPDF(string NS_ID)
        {
            try
            {
                RespuestaServicios Respuesta = new RespuestaServicios();
                String urlString = ConfigurationManager.AppSettings["URL_Servlet_pdf"];
                String ckey = ConfigurationManager.AppSettings["ConsumerKey"];
                String csecret = ConfigurationManager.AppSettings["ConsumerSecret"];
                String tkey = ConfigurationManager.AppSettings["TokenId"];
                String tsecret = ConfigurationManager.AppSettings["TokenSecret"];
                String netsuiteAccount = ConfigurationManager.AppSettings["AccountID"];
                String Algorithm = ConfigurationManager.AppSettings["Algorithm"];
                long TimeOut = 100000000000;

                Uri url = new Uri(urlString);
                OAuthBase req = new OAuthBase();
                String timestamp = req.GenerateTimeStamp();
                String nonce = req.GenerateNonce();
                String norm = "";
                String norm1 = "";
                String signature = req.GenerateSignature(url, ckey, csecret, tkey, tsecret, "POST", timestamp, nonce, out norm, out norm1);

                if (signature.Contains("+"))
                {
                    signature = signature.Replace("+", "%2B");
                }

                String header = "Authorization: OAuth ";
                header += "oauth_signature=\"" + signature + "\",";
                header += "oauth_version=\"1.0\",";
                header += "oauth_nonce=\"" + nonce + "\",";
                header += "oauth_signature_method=\"" + Algorithm + "\",";
                header += "oauth_consumer_key=\"" + ckey + "\",";
                header += "oauth_token=\"" + tkey + "\",";
                header += "oauth_timestamp=\"" + timestamp + "\",";
                header += "realm=\"" + netsuiteAccount + "\"";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlString);
                request.Timeout = (int)TimeOut;

                request.ContentType = "application/json";
                request.Method = "POST";
                request.Headers.Add(header);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    //Datos Cabecera
                    string json = "{\"type\":\"" + "number" + "\",\"entityId\":" + NS_ID + "}";

                    //string json = "";
                    streamWriter.Write(json);
                }


                WebResponse response = request.GetResponse();
                HttpWebResponse httpResponse = (HttpWebResponse)response;
                Stream resStream = httpResponse.GetResponseStream();
                StreamReader sr = new StreamReader(resStream);
                var result = sr.ReadToEnd();
                var ResultObject = Newtonsoft.Json.JsonConvert.DeserializeObject((string)result);
                string GetResultado = ResultObject.ToString();
                var ConvertResultado = JsonConvert.DeserializeObject<JsonRespuestaGeneral>(GetResultado);

                if (ConvertResultado.code == "200")
                {
                    Respuesta.Estatus = "Exito";
                    Respuesta.Mensaje = "Se ha generado el PDF con Exito.";
                    Respuesta.Valor = ConvertResultado.value;
                }
                else
                {
                    Respuesta.Estatus = "Fracaso";
                    Respuesta.Mensaje = string.Format("Hubo un problema al crear la ejecución de pedido. Detalles: {0}", ConvertResultado.message);
                }

                return Respuesta;
            }
            catch (Exception ex)
            {
                RespuestaServicios Respuesta = new RespuestaServicios();
                Respuesta.Estatus = "Freacaso";
                Respuesta.Mensaje = ex.Message;
                return Respuesta;
            }
        }

        public RespuestaServicios ConnectionServiceServletCustList(string NS_ID,string NS_ID_CONTACTO)
        {
            try
            {
                RespuestaServicios Respuesta = new RespuestaServicios();
                String urlString = ConfigurationManager.AppSettings["URL_Servlet_CTD"];
                String ckey = ConfigurationManager.AppSettings["ConsumerKey"];
                String csecret = ConfigurationManager.AppSettings["ConsumerSecret"];
                String tkey = ConfigurationManager.AppSettings["TokenId"];
                String tsecret = ConfigurationManager.AppSettings["TokenSecret"];
                String netsuiteAccount = ConfigurationManager.AppSettings["AccountID"];
                String Algorithm = ConfigurationManager.AppSettings["Algorithm"];
                long TimeOut = 100000000000;

                Uri url = new Uri(urlString);
                OAuthBase req = new OAuthBase();
                String timestamp = req.GenerateTimeStamp();
                String nonce = req.GenerateNonce();
                String norm = "";
                String norm1 = "";
                String signature = req.GenerateSignature(url, ckey, csecret, tkey, tsecret, "POST", timestamp, nonce, out norm, out norm1);

                if (signature.Contains("+"))
                {
                    signature = signature.Replace("+", "%2B");
                }

                String header = "Authorization: OAuth ";
                header += "oauth_signature=\"" + signature + "\",";
                header += "oauth_version=\"1.0\",";
                header += "oauth_nonce=\"" + nonce + "\",";
                header += "oauth_signature_method=\"" + Algorithm + "\",";
                header += "oauth_consumer_key=\"" + ckey + "\",";
                header += "oauth_token=\"" + tkey + "\",";
                header += "oauth_timestamp=\"" + timestamp + "\",";
                header += "realm=\"" + netsuiteAccount + "\"";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlString);
                request.Timeout = (int)TimeOut;

                request.ContentType = "application/json";
                request.Method = "POST";
                request.Headers.Add(header);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    //Datos Cabecera
                    string json = "{\"type\":\"" + "number" + "\",\"entityId\":\"" + NS_ID + "\",\"contactId\":" + NS_ID_CONTACTO + "}";

                    //string json = "";
                    streamWriter.Write(json);
                }


                WebResponse response = request.GetResponse();
                HttpWebResponse httpResponse = (HttpWebResponse)response;
                Stream resStream = httpResponse.GetResponseStream();
                StreamReader sr = new StreamReader(resStream);
                var result = sr.ReadToEnd();
                var ResultObject = Newtonsoft.Json.JsonConvert.DeserializeObject((string)result);
                string GetResultado = ResultObject.ToString();
                var ConvertResultado = JsonConvert.DeserializeObject<JsonRespuestaGeneral>(GetResultado);

                if (ConvertResultado.code == "200")
                {
                    Respuesta.Estatus = "Exito";
                    Respuesta.Mensaje = "Se ha generado el PDF con Exito.";
                    Respuesta.Valor = ConvertResultado.value;
                }
                else
                {
                    Respuesta.Estatus = "Fracaso";
                    Respuesta.Mensaje = string.Format("Hubo un problema al crear la ejecución de pedido. Detalles: {0}", ConvertResultado.message);
                }

                return Respuesta;
            }
            catch (Exception ex)
            {
                RespuestaServicios Respuesta = new RespuestaServicios();
                Respuesta.Estatus = "Freacaso";
                Respuesta.Mensaje = ex.Message;
                return Respuesta;
            }
        }

        public RespuestaServicios ConnectionServiceServletTO(EntregaMercancia Model)
        {
            try
            {
                RespuestaServicios Respuesta = new RespuestaServicios();
                String urlString = ConfigurationManager.AppSettings["URL_Servlet_TO"];
                String ckey = ConfigurationManager.AppSettings["ConsumerKey"];
                String csecret = ConfigurationManager.AppSettings["ConsumerSecret"];
                String tkey = ConfigurationManager.AppSettings["TokenId"];
                String tsecret = ConfigurationManager.AppSettings["TokenSecret"];
                String netsuiteAccount = ConfigurationManager.AppSettings["AccountID"];
                String Algorithm = ConfigurationManager.AppSettings["Algorithm"];
                long TimeOut = 100000000000;

                Uri url = new Uri(urlString);
                OAuthBase req = new OAuthBase();
                String timestamp = req.GenerateTimeStamp();
                String nonce = req.GenerateNonce();
                String norm = "";
                String norm1 = "";
                String signature = req.GenerateSignature(url, ckey, csecret, tkey, tsecret, "POST", timestamp, nonce, out norm, out norm1);

                if (signature.Contains("+"))
                {
                    signature = signature.Replace("+", "%2B");
                }

                String header = "Authorization: OAuth ";
                header += "oauth_signature=\"" + signature + "\",";
                header += "oauth_version=\"1.0\",";
                header += "oauth_nonce=\"" + nonce + "\",";
                header += "oauth_signature_method=\"" + Algorithm + "\",";
                header += "oauth_consumer_key=\"" + ckey + "\",";
                header += "oauth_token=\"" + tkey + "\",";
                header += "oauth_timestamp=\"" + timestamp + "\",";
                header += "realm=\"" + netsuiteAccount + "\"";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlString);
                request.Timeout = (int)TimeOut;

                request.ContentType = "application/json";
                request.Method = "POST";
                request.Headers.Add(header);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    //Datos Cabecera
                    string json = "{\"operation\":\"" + "insertItemFulfillment" + "\",";
                    json += "\"records\":[{\"referenceRecId\":\"" + Model.NS_InternalID_Doc + "\",";
                    json += "\"memo\":\"" + Model.Memo + "\",";
                    json += "\"shipstatus\":\"" + "C" + "\",\"sublists\":[{\"itemList\":[";

                    //Validamos el total de registros
                    DetalleEntregaMercancia Itemlast = Model.l_Detalle.Last();
                    foreach (DetalleEntregaMercancia RegMerc in Model.l_Detalle)
                    {

                        if (RegMerc.TipoMaterial == "Assembly")
                        {
                            if (RegMerc.Equals(Itemlast))
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + ",\"inventorydetail\":[{\"issueinventorynumber\":\"" + RegMerc.IssuesInventoryID + "\",\"quantity\":" + RegMerc.Cantidad + "}]}";
                            }
                            else
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + ",\"inventorydetail\":[{\"issueinventorynumber\":\"" + RegMerc.IssuesInventoryID + "\",\"quantity\":" + RegMerc.Cantidad + "}]},";
                            }
                        }
                        else if (RegMerc.TipoMaterial == "Inventory")
                        {
                            if (RegMerc.Equals(Itemlast))
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + "}";
                            }
                            else
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + "},";
                            }
                        }
                        else if (RegMerc.TipoMaterial == "Assembly_nL")
                        {
                            if (RegMerc.Equals(Itemlast))
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + "}";
                            }
                            else
                            {
                                json += "{\"item\":\"" + RegMerc.NS_InternalID_Mercancia + "\",\"quantity\":" + RegMerc.Cantidad + "},";
                            }
                        }


                    }

                    json += "]}]}]}";

                    streamWriter.Write(json);
                }


                WebResponse response = request.GetResponse();
                HttpWebResponse httpResponse = (HttpWebResponse)response;
                Stream resStream = httpResponse.GetResponseStream();
                StreamReader sr = new StreamReader(resStream);
                var result = sr.ReadToEnd();
                var ResultObject = Newtonsoft.Json.JsonConvert.DeserializeObject((string)result);
                string GetResultado = ResultObject.ToString();
                var ConvertResultado = JsonConvert.DeserializeObject<JsonRespuestaGeneral>(GetResultado);

                if (ConvertResultado.code == "200")
                {
                    Respuesta.Estatus = "Exito";
                    Respuesta.Mensaje = "Se ha creado la ejecución de pedido con exito";
                }
                else
                {
                    Respuesta.Estatus = "Fracaso";
                    Respuesta.Mensaje = string.Format("Hubo un problema al crear la ejecución de pedido. Detalles: {0}", ConvertResultado.message);
                }


                return Respuesta;
            }
            catch (Exception ex)
            {
                RespuestaServicios Respuesta = new RespuestaServicios();
                Respuesta.Estatus = "Freacaso";
                Respuesta.Mensaje = ex.Message;
                return Respuesta;
            }
        }

        public RespuestaServicios ConnectionServiceServletPruebas(double NS_ID)
        {
            try
            {
                RespuestaServicios Respuesta = new RespuestaServicios();
                String urlString = ConfigurationManager.AppSettings["URL_Servlet_Pruebas"];
                String ckey = ConfigurationManager.AppSettings["ConsumerKey"];
                String csecret = ConfigurationManager.AppSettings["ConsumerSecret"];
                String tkey = ConfigurationManager.AppSettings["TokenId"];
                String tsecret = ConfigurationManager.AppSettings["TokenSecret"];
                String netsuiteAccount = ConfigurationManager.AppSettings["AccountID"];
                String Algorithm = ConfigurationManager.AppSettings["Algorithm"];
                long TimeOut = 100000000000;

                Uri url = new Uri(urlString);
                OAuthBase req = new OAuthBase();
                String timestamp = req.GenerateTimeStamp();
                String nonce = req.GenerateNonce();
                String norm = "";
                String norm1 = "";
                String signature = req.GenerateSignature(url, ckey, csecret, tkey, tsecret, "POST", timestamp, nonce, out norm, out norm1);

                if (signature.Contains("+"))
                {
                    signature = signature.Replace("+", "%2B");
                }

                String header = "Authorization: OAuth ";
                header += "oauth_signature=\"" + signature + "\",";
                header += "oauth_version=\"1.0\",";
                header += "oauth_nonce=\"" + nonce + "\",";
                header += "oauth_signature_method=\"" + Algorithm + "\",";
                header += "oauth_consumer_key=\"" + ckey + "\",";
                header += "oauth_token=\"" + tkey + "\",";
                header += "oauth_timestamp=\"" + timestamp + "\",";
                header += "realm=\"" + netsuiteAccount + "\"";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlString);
                request.Timeout = (int)TimeOut;

                request.ContentType = "application/json";
                request.Method = "POST";
                request.Headers.Add(header);

                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    //Datos Cabecera
                    string json = "{\"type\":\"" + "number" + "\",\"entityId\":" + NS_ID + "}";

                    //string json = "";
                    streamWriter.Write(json);
                }


                WebResponse response = request.GetResponse();
                HttpWebResponse httpResponse = (HttpWebResponse)response;
                Stream resStream = httpResponse.GetResponseStream();
                StreamReader sr = new StreamReader(resStream);
                var result = sr.ReadToEnd();
                var ResultObject = Newtonsoft.Json.JsonConvert.DeserializeObject((string)result);
                string GetResultado = ResultObject.ToString();
                var ConvertResultado = JsonConvert.DeserializeObject<JsonRespuestaGeneral>(GetResultado);

                if (ConvertResultado.code == "200")
                {
                    Respuesta.Estatus = "Exito";
                    Respuesta.Mensaje = "Se ha generado el PDF con Exito.";
                    Respuesta.Valor = ConvertResultado.value;
                }
                else
                {
                    Respuesta.Estatus = "Fracaso";
                    Respuesta.Mensaje = string.Format("Hubo un problema al crear la ejecución de pedido. Detalles: {0}", ConvertResultado.message);
                }

                return Respuesta;
            }
            catch (Exception ex)
            {
                RespuestaServicios Respuesta = new RespuestaServicios();
                Respuesta.Estatus = "Freacaso";
                Respuesta.Mensaje = ex.Message;
                return Respuesta;
            }
        }
    }
}
