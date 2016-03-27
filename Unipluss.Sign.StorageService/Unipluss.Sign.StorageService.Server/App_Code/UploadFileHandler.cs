using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using dotnet.common;
using dotnet.common.hash;
using dotnet.common.misc;
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

                base.LogDebugInfo(string.Format("UploadFileHandler, account: {0}, key: {1}, filename: {2}", account, key, filename));

                if (!AuthorizationHandler.CheckIfFilenameIsValid(filename))
                {
                    base.LogDebugInfo(string.Format("UploadFileHandler, {0} not valid filename", filename));

                    context.Response.Write("Not valid filename");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.ApplicationInstance.CompleteRequest();
                    return;
                }

                try
                {
                    string path = string.Format(@"{0}{1}\{2}", AppSettingsReader.RootFolder, account, key);

                    if (System.IO.Directory.Exists(path))
                    {
                        base.LogDebugInfo(string.Format("UploadFileHandler, Directory {0} exists", path));
                        bool fileExists = File.Exists(string.Format(@"{0}\{1}", path, filename));
                        if (fileExists && File.ReadAllBytes(string.Format(@"{0}\{1}", path, filename)).ToSha1(HashFormat.HEX).Equals(context.Request.Headers["filesha1"]))                            
                        {
                            base.LogDebugInfo(string.Format(@"UploadFileHandler, File {0}\{1} already exists, use unique filenames.", path, filename));

                            context.Response.Write(string.Format("Filename: {0} already exists, use unique filenames.", filename));
                            context.Response.StatusCode = (int)HttpStatusCode.Ambiguous;
                        }
                        else
                        {
                            if (fileExists)
                            {
                                base.LogDebugInfo(string.Format(@"UploadFileHandler, Deleting existing file {0}\{1} beacause not SHA do not match.", path, filename));
                                File.Delete(path);
                            }

                            if (SaveFile(context, path, filename))
                            {
                                base.LogDebugInfo(string.Format(@"UploadFileHandler, File {0}\{1} saved", path, filename));

                                var metadata = SaveMetaData(context, path, filename);

                                base.LogDebugInfo(string.Format(@"UploadFileHandler, File {0}\{1} metadata {2} saved",
                                    path, filename,
                                    string.Join(",",
                                        metadata.Cast<string>().Select(e => string.Format("{0}={1}", e, metadata[e])))));

                                context.Response.StatusCode = (int)HttpStatusCode.Created;
                            }
                            else
                            {
                                base.LogDebugInfo(string.Format(@"UploadFileHandler, File {0}\{1} NOT saved", path, filename));
                                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            }
                        }
                    }
                    else
                    {
                        base.LogDebugInfo(string.Format(@"UploadFileHandler, Directory {0} not found", path));

                        if (System.IO.Directory.Exists(string.Format(@"{0}{1}", AppSettingsReader.RootFolder, account)))
                        {
                            base.LogDebugInfo(string.Format(@"UploadFileHandler, Non authorized request {0}{1}", AppSettingsReader.RootFolder, account));

                            context.Response.Write("Non authorized request");
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        else
                        {
                            base.LogDebugInfo("UploadFileHandler, Container not found");

                            context.Response.Write("Container not found");
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                    }
                }
                catch (ArgumentException e)
                {
                    base.LogError(context, e, "UploadFileHandler, ArgumentException");

                    context.Response.Write("Not valid containername");
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                catch (System.IO.PathTooLongException e)
                {
                    base.LogError(context, e, "UploadFileHandler, PathTooLongException, Root path in config to long, must be less than 160 characters including length of the filenames that will be used");

                    context.Response.Write("Root path in config to long, must be less than 160 characters including length of the filenames that will be used");
                    context.Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                }
                catch (IOException e)
                {
                    base.LogError(context, e, "UploadFileHandler, IOException");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                catch (Exception e)
                {
                    base.LogError(context, e, "UploadFileHandler, Exception");

                    base.WriteExceptionIfDebug(context, e);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
                finally
                {
                    context.ApplicationInstance.CompleteRequest();
                }
            }
        }

        private static bool SaveFile(HttpContext context, string path, string filename)
        {
            using (var ms = new MemoryStream())
            {
                context.Request.InputStream.CopyTo(ms);
                var bytes = ms.ToArray();

                if (bytes.ToSha1(HashFormat.HEX).Equals(context.Request.Headers["filesha1"]))
                {
                    if (!string.IsNullOrWhiteSpace(AppSettingsReader.CertificateThumbprint))
                        bytes = encryption.EncryptFile(bytes);
                    CommonHelper.Retry<IOException>(() => File.WriteAllBytes(string.Format(@"{0}\{1}", path, filename), bytes), 5, 1000);
                    return true;
                }
            }

            return false;
        }

        private static NameValueCollection SaveMetaData(HttpContext context, string path, string filename)
        {
            var metadata = context.Request.Headers;
            var filteredMetaData = new NameValueCollection();

            foreach (string metaKey in metadata.AllKeys.Where(x => x.Contains("x-metadata-")))
            {
                filteredMetaData.Add(metaKey.Replace("x-metadata-", string.Empty), metadata[metaKey]);
            }
            if (filteredMetaData.Count > 1)
                CommonHelper.Retry<IOException>(() => SerializeExtensions.Serialize(filteredMetaData, string.Format(@"{0}\{1}.metadata", path, Path.GetFileName(filename).Replace(".", "_"))), 5, 1000);

            return filteredMetaData;
        }
    }
}
