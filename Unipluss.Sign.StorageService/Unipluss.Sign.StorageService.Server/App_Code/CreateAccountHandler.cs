using System;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class CreateAccountHandler : IHttpAsyncHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            ServeContent(context);
        }

        public bool IsReusable { get { return false; }  }
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

        /// <summary>
        /// You only have to modify this method.
        /// </summary>
        private void ServeContent(HttpContext context)
        {
            if (AuthorizationHandler.VerifyIfRequestIsAuthed(context))
            {
                var account = context.Request.QueryString["containername"];

                if (!AuthorizationHandler.CheckIfFolderNameIsInvalid(account))
                {
                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                string key = RandomStringGenerator.GetRandomString(220);
                try
                {
                    System.IO.Directory.CreateDirectory(string.Format(@"{0}{1}\{2}", AppSettingsReader.RootFolder,
                        account, key));

                    context.Response.Write(key);
                    context.Response.AddHeader("Content-type", "text/plain; charset=utf-8");
                    context.Response.StatusCode = (int) HttpStatusCode.Created;
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
