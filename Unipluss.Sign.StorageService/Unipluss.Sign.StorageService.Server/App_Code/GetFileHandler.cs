﻿using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using dotnet.common;
using Unipluss.Sign.StorageService.Server.Code;
using dotnet.common.files;
using dotnet.common.misc;

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

                base.LogDebugInfo(string.Format("GetFileHandler, account: {0}, key: {1}, filename: {2}", account, key, filename));

                if (!AuthorizationHandler.CheckIfFilenameIsValid(filename))
                {
                    base.LogDebugInfo(string.Format("GetFileHandler, {0} not valid filename", filename));

                    context.Response.Write("Not valid filename");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.ApplicationInstance.CompleteRequest();
                    return;
                }

                try
                {
                    string path = string.Format(@"{0}{1}\{2}\{3}", AppSettingsReader.RootFolder, account, key, filename);

                    if (System.IO.File.Exists(path))
                    {
                        base.LogDebugInfo(string.Format("GetFileHandler, File {0} exists", path));

                        var metadata = AddMetaData(context, account, key, filename);

                        base.LogDebugInfo(string.Format(@"GetFileHandler, File metadata: {0}", string.Join(",", metadata.Cast<string>().Select(e => string.Format("{0}={1}", e, metadata[e])))));

                        context.Response.Headers.Add("x-response-filename", filename);

                        TimeSpan freshness = new TimeSpan(1, 0, 0, 60);
                        context.Response.Cache.SetExpires(DateTime.Now.Add(freshness));
                        context.Response.Cache.SetMaxAge(freshness);
                        context.Response.Cache.SetCacheability(HttpCacheability.Public);
                        context.Response.Cache.SetValidUntilExpires(true);
                        context.Response.Cache.VaryByParams["*"] = true;

                       
                        CommonHelper.Retry<IOException>(() =>
                        {
                            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                if (!string.IsNullOrWhiteSpace(AppSettingsReader.CertificateThumbprint))
                                {
                                    using (var ms = new MemoryStream())
                                    {
                                        fs.CopyTo(ms);
                                        context.Response.BinaryWrite(encryption.DecryptFile(ms.ToArray()));
                                    }
                                }
                                else
                                {
                                    fs.CopyTo(context.Response.OutputStream);
                                }
                            }
                           
                        }, 5, 1500);

                        base.LogDebugInfo(string.Format(@"GetFileHandler, File {0} written to response", path));

                        context.Response.Headers.Add("Content-Type",filename.GetMIMEType());
                        context.Response.Headers.Add("Content-Disposition", string.Format("attachment; filename=\"{0}\"", filename));
                        context.Response.Headers.Add("Content-Length", new System.IO.FileInfo(path).Length.ToString());
                    }
                    else
                    {
                        base.LogDebugInfo(string.Format("GetFileHandler, File {0} not found", path));

                        if (System.IO.Directory.Exists(string.Format(@"{0}{1}\{2}\", AppSettingsReader.RootFolder, account, key)))
                        {
                            base.LogDebugInfo(string.Format(@"GetFileHandler, File in directory {0}{1}\{2}\ not found", AppSettingsReader.RootFolder, account, key));

                            context.Response.Write("File not found");
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                        else if (System.IO.Directory.Exists(string.Format(@"{0}{1}", AppSettingsReader.RootFolder, account)))
                        {
                            base.LogDebugInfo(string.Format(@"GetFileHandler, Non authorized request {0}{1}", AppSettingsReader.RootFolder, account));

                            context.Response.Write("Non authorized request");
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            base.LogDebugInfo("GetFileHandler, Container not found");

                            context.Response.Write("Container not found");
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    base.LogError(context, e, "GetFileHandler, ArgumentException");

                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                catch (System.IO.PathTooLongException e)
                {
                    base.LogError(context, e, "GetFileHandler, PathTooLongException, Root path in config to long, must be less than 160 characters including length of the filenames that will be used");

                    context.Response.Write("Root path in config to long, must be less than 160 characters including length of the filenames that will be used");
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                }
                catch (IOException e)
                {
                    base.LogError(context, e, "GetFileHandler, IOException");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                catch (Exception e)
                {
                    base.LogError(context, e, "GetFileHandler, Exception");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
                finally
                {
                    context.ApplicationInstance.CompleteRequest();
                }
            }
        }

        private static NameValueCollection AddMetaData(HttpContext context, string account, string key, string filename)
        {
            var metapath = string.Format(@"{0}{1}\{2}\{3}.metadata", AppSettingsReader.RootFolder, account, key, Path.GetFileName(filename).Replace(".", "_"));

            if (!File.Exists(metapath)) 
                return new NameValueCollection();

            var metadata = SerializeExtensions.DeSerialize(metapath);
            foreach (string headerKey in metadata)
            {
                context.Response.Headers.Add(string.Format("x-metadata-{0}", headerKey), metadata[headerKey]);
            }
            return metadata;
        }
    }
}
