using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using dotnet.common.encryption;
using log4net;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public abstract class BaseAsyncHandler : IHttpAsyncHandler
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static IEncryption encryption;


        public BaseAsyncHandler()
        {
            if (encryption == null && !string.IsNullOrWhiteSpace(AppSettingsReader.CertificateThumbprint))
            {
                encryption=new CertificateEncryptionService(AppSettingsReader.CertificateThumbprint,StoreName.My,StoreLocation.CurrentUser);
            }

        }
        protected abstract void ServeContent(HttpContext context);
        public void ProcessRequest(HttpContext context)
        {
            ServeContent(context);
        }

        public bool IsReusable { get { return false; } }

        #region IHttpAsyncHandler Members

        private AsyncProcessorDelegate _Delegate;
        protected delegate void AsyncProcessorDelegate(HttpContext context);

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            _Delegate = new AsyncProcessorDelegate(ProcessRequest);
            return _Delegate.BeginInvoke(context, cb, extraData);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            _Delegate.EndInvoke(result);
        }

        #endregion

        protected void WriteExceptionIfDebug(HttpContext context, Exception exception)
        {
            string response = "Something went wrong";
            if (AppSettingsReader.Debug)
                response += Environment.NewLine + exception.ToString();
            context.Response.Write(response);
        }
        
        protected void LogError(HttpContext context, Exception exception, string message)
        {
            log.Error(string.Format("Headers = {0}, Url = {1}, Message = {2}", context.Request.Headers, context.Request.Url, message), exception);
        }

        protected void LogDebugInfo(string message)
        {
            log.Debug(message);
        }
    }
}