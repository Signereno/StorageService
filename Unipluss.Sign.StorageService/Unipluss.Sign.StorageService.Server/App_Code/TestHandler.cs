using System;
using System.IO;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class TestHandler : BaseAsyncHandler
    {
        protected override void ServeContent(HttpContext context)
        {
            try
            {
                if (System.IO.Directory.Exists(AppSettingsReader.RootFolder))
                {
                    base.LogDebugInfo(string.Format("TestHandler, Rootfolder {0} found", AppSettingsReader.RootFolder));

                    context.Response.Write("Rootfolder found");
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                else
                {
                    base.LogDebugInfo(string.Format("TestHandler, Rootfolder {0} not found", AppSettingsReader.RootFolder));

                    context.Response.Write("Rootfolder not found");
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            catch (ArgumentException e)
            {
                base.LogError(context, e, "TestHandler, ArgumentException");

                base.WriteExceptionIfDebug(context, e);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            catch (DirectoryNotFoundException e)
            {
                base.LogError(context, e, "TestHandler, DirectoryNotFoundException, Container not found");

                context.Response.Write("Container not found");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            catch (System.IO.PathTooLongException e)
            {
                base.LogError(context, e, "TestHandler, PathTooLongException, Root path in config to long, must be less than 160 characters including length of the filenames that will be used");

                context.Response.Write("Root path in config to long, must be less than 160 characters including length of the filenames that will be used");
                context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
            }
            catch (IOException e)
            {
                base.LogError(context, e, "TestHandler, IOException");

                base.WriteExceptionIfDebug(context, e);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            catch (Exception e)
            {
                base.LogError(context, e, "TestHandler, Exception");

                context.Response.Write("Something went wrong");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            finally
            {
                context.ApplicationInstance.CompleteRequest();
            }
        }
    }
}
