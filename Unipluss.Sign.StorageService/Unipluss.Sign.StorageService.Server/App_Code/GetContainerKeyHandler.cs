using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class GetContainerKeyHandler : BaseAsyncHandler
    {
        protected override void ServeContent(HttpContext context)
        {
            if (AuthorizationHandler.VerifyIfRequestIsAuthed(context, true))
            {
                var account = context.Request.QueryString["containername"];

                base.LogDebugInfo(string.Format("GetContainerKeyHandler, account: {0}", account));

                if (!AuthorizationHandler.CheckIfFolderNameIsInvalid(account))
                {
                    base.LogDebugInfo(string.Format("GetContainerKeyHandler, {0} not valid account", account));

                    context.Response.Write("Not valid account");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.ApplicationInstance.CompleteRequest();
                    return;
                }

                try
                {
                    var directories =
                        Directory.GetDirectories(string.Format(@"{0}{1}", AppSettingsReader.RootFolder, account));

                    if (directories == null || !directories.Any())
                        throw new DirectoryNotFoundException();

                    context.Response.AddHeader("Content-type", "text/plain; charset=utf-8");

                    var directory = new DirectoryInfo(directories.FirstOrDefault()).Name;

                    context.Response.Write(directory);

                    base.LogDebugInfo(string.Format(@"GetContainerKeyHandler, Container key: {0}", directory));

                    context.Response.Headers.Add("GetContainerKeyHandler", "true");
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                catch (ArgumentException e)
                {
                    base.LogError(context, e, "GetContainerKeyHandler, ArgumentException, Not valid containername");

                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                catch (DirectoryNotFoundException e)
                {
                    base.LogError(context, e, "GetContainerKeyHandler, DirectoryNotFoundException, Container not found");

                    context.Response.Write("Container not found");
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                catch (System.IO.PathTooLongException e)
                {
                    base.LogError(context, e,
                        "GetContainerKeyHandler, PathTooLongException, Root path in config to long, must be less than 160 characters including length of the filenames that will be used");

                    context.Response.Write(
                        "Root path in config to long, must be less than 160 characters including length of the filenames that will be used");
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                }
                catch (IOException e)
                {
                    base.LogError(context, e, "GetContainerKeyHandler, IOException");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                catch (Exception e)
                {
                    base.LogError(context, e, "GetContainerKeyHandler, Exception");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
                finally
                {
                    context.ApplicationInstance.CompleteRequest();
                }
            }
        }
    }
}
