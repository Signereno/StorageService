using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Unipluss.Sign.StorageService.Server
{

    public class StorageServer : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Storage is the only thing required for basic configuration.
            // Just discover what configuration options do you have.
           Trace.WriteLine("Storage service starting");
            //.UseActivator(...)
            //.UseLogProvider(...)
        }

        protected void Application_End()
        {
            if (BaseAsyncHandler.encryption!=null)
             BaseAsyncHandler.encryption.Dispose();
        }
    }
}
