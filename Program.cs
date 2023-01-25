using DataManagement;
using DataManagement.Workers;

namespace Company.WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<CleanWorker>();
                })
                .Build();

            host.Run();
        }
    }
}