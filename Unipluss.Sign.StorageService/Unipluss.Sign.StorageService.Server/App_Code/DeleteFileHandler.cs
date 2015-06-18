using System;
using System.IO;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class DeleteFileHandler : BaseAsyncHandler
    {
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
                        key, filename);
                    if (
                        System.IO.File.Exists(path))
                    {
                        File.Delete(path);

                        var metapath = string.Format(@"{0}{1}\{2}\{3}.metadata", AppSettingsReader.RootFolder, account,
                            key, Path.GetFileName(filename).Replace(".", "_"));

                        if(System.IO.File.Exists(metapath))
                            File.Delete(metapath);

                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        if (System.IO.Directory.Exists(string.Format(@"{0}{1}\{2}\", AppSettingsReader.RootFolder, account, key)))
                        {
                            context.Response.Write("File not found");
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                        else if (System.IO.Directory.Exists(string.Format(@"{0}{1}", AppSettingsReader.RootFolder, account)))
                        {
                            //context.Response.Headers.Add("path", string.Format(@"{0}{1}", AppSettingsReader.RootFolder, account));
                            context.Response.Write("Non authorized request");
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            context.Response.Write("Container not found");
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }

                    }

                    context.ApplicationInstance.CompleteRequest();
                }
                catch (ArgumentException e)
                {
                    base.WriteExceptionIfDebug(context, e); context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    ;
                    context.ApplicationInstance.CompleteRequest();
                }
                catch (Exception)
                {
                    context.Response.Write("Something went wrong");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    ;
                    context.ApplicationInstance.CompleteRequest();
                }
            }
        }
    }
}