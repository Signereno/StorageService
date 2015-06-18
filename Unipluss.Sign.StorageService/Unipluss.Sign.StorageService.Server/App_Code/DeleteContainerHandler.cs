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
            if (AuthorizationHandler.VerifyIfRequestIsAuthed(context, true))
            {
                var containerName = context.Request.QueryString["containername"];

                if (!AuthorizationHandler.CheckIfFolderNameIsInvalid(containerName))
                {
                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    context.ApplicationInstance.CompleteRequest();
                    return;
                }

                try
                {
                    var path = string.Format(string.Format("{0}{1}", AppSettingsReader.RootFolder, containerName));
                    if (Directory.Exists(path))
                    {
                        clearFolder(path);
                        Directory.Delete(path);
                        context.Response.Write("Container deleted ok");
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        context.Response.Write("Container not found");
                        context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    }
                    context.ApplicationInstance.CompleteRequest();


                }
                catch (ArgumentException)
                {
                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
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
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.ApplicationInstance.CompleteRequest();
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