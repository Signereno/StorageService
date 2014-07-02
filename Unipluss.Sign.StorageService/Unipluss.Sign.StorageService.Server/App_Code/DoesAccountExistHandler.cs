using System;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class DoesAccountExistHandler : BaseAsyncHandler
    {
        protected override void ServeContent(HttpContext context)
        {
            if (AuthorizationHandler.VerifyIfRequestIsAuthed(context))
            {
                var account = context.Request.QueryString["containername"];
                var key = context.Request.QueryString["key"];

                try
                {
                    if (
                        System.IO.Directory.Exists(string.Format(@"{0}{1}\{2}", AppSettingsReader.RootFolder, account,
                            key)))
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.Found;
                    }
                    else
                    {
                        if (System.IO.Directory.Exists(string.Format(@"{0}{1}", AppSettingsReader.RootFolder, account)))
                        {
                            context.Response.Write("Non authorized request");
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                              context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                        }
                      
                    }

                    context.Response.End();
                }
                catch (ArgumentException)
                {
                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    ;
                    context.Response.End();
                }
                catch (Exception)
                {
                    context.Response.Write("Something went wrong");
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    ;
                    context.Response.End();
                }
            }


        }
    }
}
