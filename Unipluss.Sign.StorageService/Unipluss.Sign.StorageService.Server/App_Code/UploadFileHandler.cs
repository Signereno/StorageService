using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Unipluss.Sign.StorageService.Server.Code;

namespace Unipluss.Sign.StorageService.Server
{
    public class UploadFileHandler : BaseAsyncHandler
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
                    string path = string.Format(@"{0}{1}\{2}", AppSettingsReader.RootFolder, account,
                        key);
                    if (System.IO.Directory.Exists(path))
                    {
                        using (var ms = new MemoryStream())
                        {
                            context.Request.InputStream.CopyTo(ms);
                            File.WriteAllBytes(string.Format(@"{0}\{1}",path,filename),ms.ToArray());
                            var metadata = context.Request.Headers;
                            NameValueCollection filteredMetaData=new NameValueCollection();

                            foreach (string metaKey in metadata.AllKeys.Where(x=>x.Contains("x-metadata-")))
                            {
                                filteredMetaData.Add(metaKey.Replace("x-metadata-",string.Empty),metadata[metaKey]);   
                            }

                            Extensions.Serialize(filteredMetaData, string.Format(@"{0}\{1}.metadata", path, Path.GetFileNameWithoutExtension(filename)));
                          
                        }
                        context.Response.StatusCode = (int)HttpStatusCode.Created;

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
    }
}
