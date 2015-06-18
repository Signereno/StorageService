using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;
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
                    string path = string.Format(@"{0}{1}\{2}\{3}", AppSettingsReader.RootFolder, account, key,filename);
                    if (System.IO.File.Exists(path))
                    {
                        AddMetaData(context, account, key, filename);
                        context.Response.Headers.Add("x-response-filename",filename);

                        context.Response.Headers.Add("Content-Type", MimeAssistant.GetMIMEType(filename));
                        context.Response.Headers.Add("Content-Disposition", string.Format("attachment; filename=\"{0}\"", filename));
                        context.Response.Headers.Add("Content-Length", new System.IO.FileInfo(path).Length.ToString());


                        TimeSpan freshness = new TimeSpan(1, 0, 0, 60);
                        context.Response.Cache.SetExpires(DateTime.Now.Add(freshness));
                        context.Response.Cache.SetMaxAge(freshness);
                        context.Response.Cache.SetCacheability(HttpCacheability.Public);
                        context.Response.Cache.SetValidUntilExpires(true);
                        context.Response.Cache.VaryByParams["*"] = true;


                        int chunkSize = 64;
                        byte[] buffer = new byte[chunkSize];
                        int offset = 0;
                        int read = 0;
                        using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            while ((read = fs.Read(buffer, offset, chunkSize)) > 0)
                            {
                                context.Response.OutputStream.Write(buffer, 0, read);
                                context.Response.Flush();
                            }
                        }

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

                    // Prevents any other content from being sent to the browser
                    context.Response.SuppressContent = true;
                    
                    // Directs the thread to finish, bypassing additional processing
                    context.ApplicationInstance.CompleteRequest();
                }
                catch (ArgumentException)
                {
                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
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

        private static void AddMetaData(HttpContext context, string account, string key, string filename)
        {
            var metapath = string.Format(@"{0}{1}\{2}\{3}.metadata", AppSettingsReader.RootFolder, account,
                key, Path.GetFileName(filename).Replace(".", "_"));

            if (File.Exists(metapath))
            {
                var Metadata = Extensions.DeSerialize(metapath);
                foreach (string headerKey in Metadata)
                {
                    context.Response.Headers.Add(string.Format("x-metadata-{0}", headerKey), Metadata[headerKey]);
                }
            }
        }
    }
}
