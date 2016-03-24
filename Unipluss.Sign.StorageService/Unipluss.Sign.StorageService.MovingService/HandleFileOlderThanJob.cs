using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Quartz;
using Serilog;

namespace Unipluss.Sign.StorageService.MovingService
{
    public class HandleFileOlderThanJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Directory.GetFiles(AppSettingsReader.MoveFromFolder, "*.*", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .Where(f => f.CreationTime < DateTime.Now.AddMinutes(-1*AppSettingsReader.MinutesBeforeMovingFiles)
                            && !f.Name.Contains(MoveFilesService.SignerePades))
                .ToList()
                .ForEach(f =>
                {
                    var toPath = Path.Combine(AppSettingsReader.MoveToFolder, f.Name);
                    if (File.Exists(toPath))
                    {
                        using (var sha1 = new SHA1CryptoServiceProvider())
                        {
                            var sameFile = Convert.ToBase64String(sha1.ComputeHash(File.ReadAllBytes(toPath)))
                                .Equals(Convert.ToBase64String(sha1.ComputeHash(File.ReadAllBytes(f.FullName))));

                            if (!sameFile)
                            {
                                var newName = Path.Combine(AppSettingsReader.MoveToFolder,
                                    string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(f.Name),
                                        Guid.NewGuid().ToString(), Path.GetExtension(f.Name)));

                                f.MoveTo(newName);
                                Log.Logger.Debug(
                                    "Copying file {0} with new name {1} because there already exists a file with the same name but the content is different",
                                    f.Name, newName);
                            }
                            else
                            {
                                f.Delete();
                                Log.Logger.Debug("Delete file because same already exists at the to folder: {0}", f.Name);
                            }
                        }
                    }
                    else
                    {
                        f.MoveTo(toPath);
                        Log.Logger.Debug("Moving file: {0} to {1}", f.Name, toPath);
                    }
                });
        }
    }
}