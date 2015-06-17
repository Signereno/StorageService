using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class GetContainerMetaDataHander : BaseAsyncHandler
    {
        protected override void ServeContent(HttpContext context)
        {
            base.LogDebugInfo("GetContainerMetaDataHander ServeContent");

            if (AuthorizationHandler.VerifyIfRequestIsAuthed(context, true))
            {
                var account = context.Request.QueryString["containername"];

                base.LogDebugInfo(string.Format("GetContainerMetaDataHander, RequestIsAuthed, account: {0}", account));

                if (!AuthorizationHandler.CheckIfFolderNameIsInvalid(account))
                {
                    base.LogDebugInfo(string.Format("GetContainerMetaDataHander, {0} not valid account", account));

                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.End();
                    return;
                }

                try
                {
                    var metadata = AddMetaData(context, account);

                    base.LogDebugInfo(string.Format(@"GetContainerMetaDataHander, Container metadata: {0}", string.Join(",", metadata.Cast<string>().Select(e => string.Format("{0}={1}", e, metadata[e])))));

                    context.Response.Headers.Add("GetContainerMetaDataHander", "true");
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    context.Response.End();
                }
                catch (ArgumentException e)
                {
                    base.LogError(context, e, "GetContainerMetaDataHander, ArgumentException, Not valid containername");

                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.End();
                }
                catch (DirectoryNotFoundException e)
                {
                    base.LogError(context, e, "GetContainerMetaDataHander, DirectoryNotFoundException, Container not found");

                    context.Response.Write("Container not found");
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.End();
                }
                catch (System.IO.PathTooLongException e)
                {
                    base.LogError(context, e, "GetContainerMetaDataHander, PathTooLongException, Root path in config to long, must be less than 160 characters including length of the filenames that will be used");

                    context.Response.Write("Root path in config to long, must be less than 160 characters including length of the filenames that will be used");
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    context.Response.End();
                }
                catch (IOException e)
                {
                    base.LogError(context, e, "GetContainerMetaDataHander, IOException");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.End();
                }
                catch (Exception e)
                {
                    base.LogError(context, e, "GetContainerMetaDataHander, Exception");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.End();
                }
            }
        }
        private static NameValueCollection AddMetaData(HttpContext context, string account)
        {
            var metapath = string.Format(@"{0}{1}\container.metadata", AppSettingsReader.RootFolder, account);

            if (!File.Exists(metapath)) 
                return new NameValueCollection();

            var metadata = Extensions.DeSerialize(metapath);
            foreach (string headerKey in metadata)
            {
                context.Response.Headers.Add(string.Format("x-metadata-{0}", headerKey), metadata[headerKey]);
            }
            return metadata;
        }
    }
}
