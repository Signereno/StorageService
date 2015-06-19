using System;
using System.IO;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class DoesContainerExistHandler : BaseAsyncHandler
    {
        protected override void ServeContent(HttpContext context)
        {
            if (AuthorizationHandler.VerifyIfRequestIsAuthed(context, true))
            {
                var account = context.Request.QueryString["containername"];
                var key = context.Request.QueryString["key"];

                base.LogDebugInfo(string.Format("DoesContainerExistHandler, account: {0}, key: {1}", account, key));

                try
                {
                    string path = string.Format(@"{0}{1}\{2}", AppSettingsReader.RootFolder, account, key);

                    if (System.IO.Directory.Exists(path))
                    {
                        base.LogDebugInfo(string.Format("DoesContainerExistHandler, Directory {0} exists", path));

                        context.Response.Headers.Add("DoesContainerExistHandler", "true");
                        context.Response.StatusCode = (int) HttpStatusCode.OK;
                    }
                    else
                    {
                        base.LogDebugInfo(string.Format("DoesContainerExistHandler, Directory {0} not found", path));

                        if (System.IO.Directory.Exists(string.Format(@"{0}{1}", AppSettingsReader.RootFolder, account)))
                        {
                            base.LogDebugInfo(string.Format(
                                @"DoesContainerExistHandler, Non authorized request {0}{1}",
                                AppSettingsReader.RootFolder, account));

                            context.Response.Write("Non authorized request");
                            context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            base.LogDebugInfo("DoesContainerExistHandler, Container not found");

                            context.Response.Write("Container not found");
                            context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    base.LogError(context, e, "DoesContainerExistHandler, ArgumentException");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                }
                catch (System.IO.PathTooLongException e)
                {
                    base.LogError(context, e,
                        "DoesContainerExistHandler, PathTooLongException, Root path in config to long, must be less than 160 characters including length of the filenames that will be used");

                    context.Response.Write(
                        "Root path in config to long, must be less than 160 characters including length of the filenames that will be used");
                    context.Response.StatusCode = (int) HttpStatusCode.PreconditionFailed;
                }
                catch (IOException e)
                {
                    base.LogError(context, e, "DoesContainerExistHandler, IOException");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                }
                catch (Exception e)
                {
                    base.LogError(context, e, "DoesContainerExistHandler, Exception");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                }
                finally
                {
                    context.ApplicationInstance.CompleteRequest();
                }
            }
        }
    }
}
