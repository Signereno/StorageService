using System;
using System.IO;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class GetFileHandler : BaseAsyncHandler
    {
     

        /// <summary>
        /// You only have to modify this method.
        /// </summary>
        protected override void ServeContent(HttpContext context)
        {
            if (AuthorizationHandler.VerifyIfRequestIsAuthed(context))
            {
                var account = context.Request.QueryString["containername"];
                var key = context.Request.QueryString["key"];
                var filename = context.Request.QueryString["filename"];

                if (!AuthorizationHandler.CheckIfFilenameIsValid(filename))
                {
                    context.Response.Write("Not valid filename");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                try
                {
                    string path = string.Format(@"{0}{1}\{2}\{3}", AppSettingsReader.RootFolder, account,
                        key,filename);
                    if (System.IO.File.Exists(path))
                    {
                        AddMetaData(context, account, key, filename);

                        context.Response.TransmitFile(path);
                    }
                    else
                    {
                        if (System.IO.Directory.Exists(string.Format(@"{0}{1}", AppSettingsReader.RootFolder, account)))
                        {
                            context.Response.Write("Non authorized request");
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }

                    }

                    context.Response.End();
                }
                catch (ArgumentException)
                {
                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ;
                    context.Response.End();
                }
                catch (Exception)
                {
                    context.Response.Write("Something went wrong");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    ;
                    context.Response.End();
                }
            }
        }

        private static void AddMetaData(HttpContext context, string account, string key, string filename)
        {
            var metapath = string.Format(@"{0}{1}\{2}\{3}.metadata", AppSettingsReader.RootFolder, account,
                key, Path.GetFileNameWithoutExtension(filename));
            var Metadata = Extensions.DeSerialize(metapath);
            foreach (string headerKey in Metadata)
            {
                context.Response.Headers.Add(string.Format("x-metadata-{0}", headerKey), Metadata[headerKey]);
            }
        }
    }
}
