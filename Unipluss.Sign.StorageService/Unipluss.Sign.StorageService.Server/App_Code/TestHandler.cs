using System;
using System.IO;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class TestHandler : BaseAsyncHandler
    {
        protected override void ServeContent(HttpContext context)
        {
            try
            {
                    
                if ( System.IO.Directory.Exists(AppSettingsReader.RootFolder))
                {
                    context.Response.Write("Rootfolder found");
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    context.Response.Write("Rootfolder not found");
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }

                context.ApplicationInstance.CompleteRequest();
            }
            catch (ArgumentException e)
            {
                base.WriteExceptionIfDebug(context, e); context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ;
                context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception)
            {
                context.Response.Write("Something went wrong");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                ;
                context.ApplicationInstance.CompleteRequest();
            }
            
        }
    }
}
