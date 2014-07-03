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

                if (!AuthorizationHandler.CheckIfFolderNameIsInvalid(account))
                {
                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.End();
                    return;
                }

                try
                {
                    var directories =
                        Directory.GetDirectories(string.Format(@"{0}{1}", AppSettingsReader.RootFolder, account));

                    if(directories==null || !directories.Any())
                        throw new DirectoryNotFoundException();

                    context.Response.AddHeader("Content-type", "text/plain; charset=utf-8");
                    context.Response.Write(new DirectoryInfo(directories.FirstOrDefault()).Name);
                    context.Response.Headers.Add("GetContainerKeyHandler", "true");
                    context.Response.StatusCode = (int) HttpStatusCode.OK;
                    context.Response.End();
                }
                catch (ArgumentException)
                {
                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    context.Response.End();
                }
                catch (DirectoryNotFoundException)
                {
                    context.Response.Write("Container not found");
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.End();
                }
                catch (System.IO.PathTooLongException)
                {
                    context.Response.Write("Root path in config to long, must me less than 160 characters including length of the filenames that will be used");
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                    context.Response.End();
                }
                catch (Exception e)
                {
                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.End();
                }
            }
        }


    }
}
