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
            base.LogDebugInfo("DeleteFileHandler ServeContent");

            if (AuthorizationHandler.VerifyIfRequestIsAuthed(context))
            {
                var account = context.Request.QueryString["containername"];
                var key = context.Request.QueryString["key"];
                var filename = context.Request.QueryString["filename"];

                base.LogDebugInfo(string.Format("DeleteFileHandler, RequestIsAuthed, account: {0}, key: {1}, filename: {2}", account, key, filename));

                if (!AuthorizationHandler.CheckIfFilenameIsValid(filename))
                {
                    base.LogDebugInfo(string.Format("DeleteFileHandler, {0} not valid filename", filename));

                    context.Response.Write("Not valid filename");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                try
                {
                    string path = string.Format(@"{0}{1}\{2}\{3}", AppSettingsReader.RootFolder, account, key, filename);

                    base.LogDebugInfo(string.Format(@"DeleteFileHandler, Deleting file: {0}", path));

                    if (System.IO.File.Exists(path))
                    {
                        File.Delete(path);

                        base.LogDebugInfo(string.Format("DeleteFileHandler, File {0} deleted", path));

                        var metapath = string.Format(@"{0}{1}\{2}\{3}.metadata", AppSettingsReader.RootFolder, account, key, Path.GetFileName(filename).Replace(".", "_"));

                        base.LogDebugInfo(string.Format(@"DeleteFileHandler, Deleting metadata file: {0}", metapath));

                        if (System.IO.File.Exists(metapath))
                        {
                            File.Delete(metapath);

                            base.LogDebugInfo(string.Format("DeleteFileHandler, Metadata file {0} deleted", metapath));
                        }
                        else
                            base.LogDebugInfo(string.Format("DeleteFileHandler, Metadata file {0} not found", metapath));

                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        base.LogDebugInfo(string.Format("DeleteFileHandler, File {0} not found", path));

                        if (System.IO.Directory.Exists(string.Format(@"{0}{1}\{2}\", AppSettingsReader.RootFolder, account, key)))
                        {
                            base.LogDebugInfo(string.Format(@"DeleteFileHandler, File {3} in directory {0}{1}\{2}\ not found", AppSettingsReader.RootFolder, account, key, filename));

                            context.Response.Write("File not found");
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                        else if (System.IO.Directory.Exists(string.Format(@"{0}{1}", AppSettingsReader.RootFolder, account)))
                        {
                            base.LogDebugInfo(string.Format(@"DeleteFileHandler, Non authorized request {0}{1}", AppSettingsReader.RootFolder, account));

                            //context.Response.Headers.Add("path", string.Format(@"{0}{1}", AppSettingsReader.RootFolder, account));
                            context.Response.Write("Non authorized request");
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            base.LogDebugInfo("DeleteFileHandler, Container not found");

                            context.Response.Write("Container not found");
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                    }

                    context.Response.End();
                }
                catch (ArgumentException e)
                {
                    base.LogError(context, e, "DeleteFileHandler, ArgumentException");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.End();
                }
                catch (System.IO.PathTooLongException e)
                {
                    base.LogError(context, e, "DeleteFileHandler, PathTooLongException, Root path in config to long, must be less than 160 characters including length of the filenames that will be used");

                    context.Response.Write("Root path in config to long, must be less than 160 characters including length of the filenames that will be used");
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    context.Response.End();
                }
                catch (IOException e)
                {
                    base.LogError(context, e, "DeleteFileHandler, IOException");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.End();
                }
                catch (Exception e)
                {
                    base.LogError(context, e, "DeleteFileHandler, Exception");

                    context.Response.Write("Something went wrong");
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.End();
                }
            }
        }
    }
}