using Topshelf;

namespace FoldersWatcher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // CREATING A NEW WINDOWS SERVICE WITH TOPSHELF
            var rc = HostFactory.Run(x =>
            {
                x.Service<FoldersWatcher>(s =>
                {
                    s.ConstructUsing(name => new FoldersWatcher());
                    s.WhenStarted(fw => fw.Start());
                    s.WhenStopped(fw => fw.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Create, update and delete files in a folder");
                x.SetDisplayName("FolderWatcher");
                x.SetServiceName("FolderWatcher");
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }
    }
}