using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Workers
{
    public class ScheduleClean : BackgroundService
    {
        private readonly IConfiguration _configuration;

        private string _destinationPath1=null!;
        private string _destinationPath2 = null!;
        private int _dayDelete = 0;
        private Boolean _isDelete = false;


        public ScheduleClean(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Initialize();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }


        private void Initialize()
        {
            _destinationPath1 =_configuration.GetValue<string>("DestinationPath:Path1");
            _destinationPath2 = _configuration.GetValue<string>("DestinationPath:Path2");
            _dayDelete = _configuration.GetValue<int>("DayDelete");
            _isDelete = _configuration.GetValue<bool>("isDeleted");

           

            
        }
    }
}
