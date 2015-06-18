using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class CreateContainerHandler : BaseAsyncHandler
    {
      
        protected  override void ServeContent(HttpContext context)
        {
            if (AuthorizationHandler.VerifyIfRequestIsAuthed(context,true))
            {
                var account = context.Request.QueryString["containername"];

                if (!AuthorizationHandler.CheckIfFolderNameIsInvalid(account))
                {
                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.ApplicationInstance.CompleteRequest();
                    return;
                }

                string key = RandomStringGenerator.GetRandomString(100);
                try
                {
                    System.IO.Directory.CreateDirectory(string.Format(@"{0}{1}\{2}", AppSettingsReader.RootFolder, account, key));
                    SaveMetaData(context, account);
                    
                    context.Response.AddHeader("Content-type", "text/plain; charset=utf-8");
                    context.Response.Write(key);
                    context.Response.Headers.Add("test","test");
                    context.Response.StatusCode = (int) HttpStatusCode.Created;
                    context.ApplicationInstance.CompleteRequest();
                }
                catch (ArgumentException)
                {
                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    context.ApplicationInstance.CompleteRequest();
                }
                catch (System.IO.PathTooLongException)
                {
                    context.Response.Write("Root path in config to long, must me less than 160 characters including length of the filenames that will be used");
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    context.ApplicationInstance.CompleteRequest();
                }
                catch (Exception e)
                {
                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    context.ApplicationInstance.CompleteRequest();
                }
            }
        }
        private static void SaveMetaData(HttpContext context,string  account)
        {
            var metadata = context.Request.Headers;
            NameValueCollection filteredMetaData = new NameValueCollection();

            foreach (string metaKey in metadata.AllKeys.Where(x => x.Contains("x-metadata-")))
            {
                filteredMetaData.Add(metaKey.Replace("x-metadata-", string.Empty), metadata[metaKey]);
            }
            if (filteredMetaData.Count > 1)
                Extensions.Serialize(filteredMetaData,string.Format(@"{0}{1}\container.metadata", AppSettingsReader.RootFolder, account));
        }
     
    }
}