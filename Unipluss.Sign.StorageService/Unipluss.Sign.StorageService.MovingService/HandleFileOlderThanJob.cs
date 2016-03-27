using System;
using System.IO;
using dotnet.common.files;
using Quartz;
using Serilog;

namespace Unipluss.Sign.StorageService.MovingService
{
    public class HandleFileOlderThanJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {

            var files = AppSettingsReader.MoveFromFolder.GetFilesInFolder(
                includeSubDirectories: true,
                onlyFilesOlderThan: TimeSpan.FromMinutes(AppSettingsReader.MinutesBeforeMovingFiles));

            foreach (FileInfo fileInfo in files)
            {
                var toPath = Path.Combine(AppSettingsReader.MoveToFolder, fileInfo.Name);
                if (File.Exists(toPath))
                {
                    if (!toPath.MatchFileContent(fileInfo.FullName))
                    {
                        var newName = Path.Combine(AppSettingsReader.MoveToFolder,
                            string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(fileInfo.Name),
                                Guid.NewGuid().ToString(), Path.GetExtension(fileInfo.Name)));

                        fileInfo.MoveTo(newName);
                        Log.Logger.Debug(
                            "Copying file {0} with new name {1} because there already exists a file with the same name but the content is different",
                            fileInfo.Name, newName);
                    }
                    else
                    {
                        fileInfo.Delete();
                        Log.Logger.Debug("Delete file because same already exists at the to folder: {0}", fileInfo.Name);
                    }
                }
                else
                {
                    fileInfo.MoveTo(toPath);
                    Log.Logger.Debug("Moving file: {0} to {1}", fileInfo.Name, toPath);
                }
            }

        }
    }
}