using System;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public  abstract class BaseAsyncHandler : IHttpAsyncHandler
    {
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

        protected  void WriteExceptionIfDebug(HttpContext context, Exception e)
        {
            string response = "Something went wrong";
            if (AppSettingsReader.Debug)
                response += Environment.NewLine+ e.ToString();
            context.Response.Write(response);
        }
    }
}