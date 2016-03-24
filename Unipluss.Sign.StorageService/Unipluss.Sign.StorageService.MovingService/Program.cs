using Serilog;
using Topshelf;

namespace Unipluss.Sign.StorageService.MovingService
{
    public class Program
    {
        public static void Main()
        {

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();


            HostFactory.Run(x =>                                 //1
            {
                x.Service<MoveFilesService>(s =>                        //2
                {
                    s.ConstructUsing(name => new MoveFilesService());     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Dispose());               //5
                });
                x.RunAsLocalSystem();                            //6
                x.StartAutomatically();
                x.UseSerilog(Log.Logger);
                x.SetDescription("Signere.no Storage file moving service");        //7
                x.SetDisplayName("Signere.no Storage file moving service");                       //8
                x.SetServiceName("Signere.StorageServer.FileMover");                       //9
            });                                                  //10
        }
    }
}
