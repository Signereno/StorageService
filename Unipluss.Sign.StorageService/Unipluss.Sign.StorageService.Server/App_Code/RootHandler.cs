using System;
using System.IO;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class RootHandler : BaseAsyncHandler
    {
        protected override void ServeContent(HttpContext context)
        {
      
                context.Response.Write("<h2>Signere.no storageservice</h2>");
                context.Response.StatusCode = (int)HttpStatusCode.OK;
               
                context.Response.End();
            }
            
        }
    
}
