using System;
using System.IO;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class DeleteContainerHandler : BaseAsyncHandler
    {
        protected override void ServeContent(HttpContext context)
        {
            base.LogDebugInfo("DeleteContainerHandler, ServeContent");

            if (AuthorizationHandler.VerifyIfRequestIsAuthed(context, true))
            {
                var account = context.Request.QueryString["account"];

                base.LogDebugInfo(string.Format("DeleteContainerHandler, RequestIsAuthed, account: {0}", account));

                if (!AuthorizationHandler.CheckIfFolderNameIsInvalid(account))
                {
                    base.LogDebugInfo(string.Format("DeleteContainerHandler, {0} not valid account", account));

                    context.Response.Write("Not valid account");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.End();
                    return;
                }

                try
                {
                    var path = string.Format("{0}{1}", AppSettingsReader.RootFolder, account);

                    base.LogDebugInfo(string.Format(@"DeleteContainerHandler, Deleting container: {0}{1}", AppSettingsReader.RootFolder, account));

                    if (Directory.Exists(path))
                    {
                        clearFolder(path);

                        base.LogDebugInfo(string.Format(@"DeleteContainerHandler, Container {0} cleared", path));

                        Directory.Delete(path);

                        base.LogDebugInfo(string.Format(@"DeleteContainerHandler, Container {0} deleted ok", path));

                        context.Response.Write("Container deleted ok");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        base.LogDebugInfo(string.Format(@"DeleteContainerHandler, Container {0} not found", path));

                        context.Response.Write("Container not found");
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                    context.Response.End();
                }
                catch (ArgumentException e)
                {
                    base.LogError(context, e, "DeleteContainerHandler, ArgumentException, Not valid containername");

                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.End();
                }
                catch (System.IO.PathTooLongException e)
                {
                    base.LogError(context, e, "DeleteContainerHandler, PathTooLongException, Root path in config to long, must be less than 160 characters including length of the filenames that will be used");

                    context.Response.Write("Root path in config to long, must be less than 160 characters including length of the filenames that will be used");
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    context.Response.End();
                }
                catch (IOException e)
                {
                    base.LogError(context, e, "DeleteContainerHandler, IOException");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.End();
                }
                catch (Exception e)
                {
                    base.LogError(context, e, "DeleteContainerHandler, Exception");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.End();
                }
            }
        }

        private void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }
    }
}