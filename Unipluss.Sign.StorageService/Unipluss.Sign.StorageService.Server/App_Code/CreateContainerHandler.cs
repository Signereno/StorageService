using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class CreateContainerHandler : BaseAsyncHandler
    {
        protected override void ServeContent(HttpContext context)
        {
            base.LogDebugInfo("CreateContainerHandler, ServeContent");

            if (AuthorizationHandler.VerifyIfRequestIsAuthed(context, true))
            {
                var account = context.Request.QueryString["containername"];

                base.LogDebugInfo(string.Format("CreateContainerHandler, RequestIsAuthed, account: {0}", account));

                if (!AuthorizationHandler.CheckIfFolderNameIsInvalid(account))
                {
                    base.LogDebugInfo(string.Format("CreateContainerHandler, {0} not valid account", account));

                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.End();
                    return;
                }

                string key = RandomStringGenerator.GetRandomString(100);

                try
                {
                    var path = string.Format(@"{0}{1}\{2}", AppSettingsReader.RootFolder, account, key);

                    base.LogDebugInfo(string.Format("CreateContainerHandler, Creating container: {0}", path));

                    System.IO.Directory.CreateDirectory(path);

                    base.LogDebugInfo(string.Format("CreateContainerHandler, Container {0} created", path));

                    var metadata = SaveMetaData(context, account);

                    base.LogDebugInfo(string.Format(@"CreateContainerHandler, Container metadata {0} saved", string.Join(",", metadata.Cast<string>().Select(e => string.Format("{0}={1}", e, metadata[e])))));

                    context.Response.AddHeader("Content-type", "text/plain; charset=utf-8");
                    context.Response.Write(key);
                    context.Response.Headers.Add("test", "test");
                    context.Response.StatusCode = (int)HttpStatusCode.Created;
                    context.Response.End();
                }
                catch (ArgumentException e)
                {
                    base.LogError(context, e, "CreateContainerHandler, ArgumentException, Not valid containername");

                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.End();
                }
                catch (System.IO.PathTooLongException e)
                {
                    base.LogError(context, e, "CreateContainerHandler, PathTooLongException, Root path in config to long, must be less than 160 characters including length of the filenames that will be used");

                    context.Response.Write("Root path in config to long, must be less than 160 characters including length of the filenames that will be used");
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    context.Response.End();
                }
                catch (IOException e)
                {
                    base.LogError(context, e, "CreateContainerHandler, IOException");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.End();
                }
                catch (Exception e)
                {
                    base.LogError(context, e, "CreateContainerHandler, Exception");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.End();
                }
            }
        }
        private static NameValueCollection SaveMetaData(HttpContext context, string account)
        {
            var metadata = context.Request.Headers;
            var filteredMetaData = new NameValueCollection();

            foreach (string metaKey in metadata.AllKeys.Where(x => x.Contains("x-metadata-")))
            {
                filteredMetaData.Add(metaKey.Replace("x-metadata-", string.Empty), metadata[metaKey]);
            }
            if (filteredMetaData.Count > 1)
                Extensions.Serialize(filteredMetaData, string.Format(@"{0}{1}\container.metadata", AppSettingsReader.RootFolder, account));

            return filteredMetaData;
        }
    }
}