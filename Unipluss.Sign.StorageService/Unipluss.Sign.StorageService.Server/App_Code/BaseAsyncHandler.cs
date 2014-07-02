using System;
using System.Web;

namespace Unipluss.Sign.StorageService.Server
{
    public  abstract class BaseAsyncHandler : IHttpHandler
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
    }
}