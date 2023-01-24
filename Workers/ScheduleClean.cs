using System.Net.NetworkInformation;

namespace DataManagement.Workers
{
    public class ScheduleClean : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ScheduleClean> _logger;
        private string _destinationPath1 = null!;
        private string _destinationPath2 = null!;
        private int _periodDays = 0;
        private int _deleteAtHour = 0;
        private int _deleteAtMinute = 0;

        private Boolean _isDelete = false;

        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public ScheduleClean(IHostApplicationLifetime hostApplicationLifetime,
            ILogger<ScheduleClean> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _hostApplicationLifetime = hostApplicationLifetime;
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


        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int setMinute = _deleteAtHour * 60 + _deleteAtMinute;
            int lastDay = DateTime.Now.Day - 1;
            while (!stoppingToken.IsCancellationRequested && _isDelete == true)
            {
                try
                {
                    int nowDay = DateTime.Now.Day;

                    int nowMintue = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
                    if (lastDay != nowDay && nowMintue > setMinute)
                    {
                        lastDay = nowDay;
                        Proceesing();
                    }


                }
                catch (Exception ex)
                {
                    _logger.LogError($"The Website is down.{ex.Message}");
                }


                await Task.Delay(60_000, stoppingToken);
            }
        }

        private async void Proceesing()
        {
            var deldate = DateTime.Now.AddDays(-_periodDays);
            var datepoint = new DateTime(deldate.Year, deldate.Month, deldate.Day, 0, 0, 0);

            // Path 1

            string[] rbLists = Directory.GetDirectories(_destinationPath1);  //D:\picture_Backup\RB01
            foreach (var item in rbLists)
            {
                var rbOnly = item.Split('\\');
                if (rbOnly.Length == 3)
                {
                    if (rbOnly.Length > 2 && rbOnly[2].Trim().StartsWith("RB"))
                    {
                        var jobLists = Directory.GetDirectories(item); //D:\picture_Backup\RB01\010
                        foreach (var job in jobLists)
                        {
                            string[] backupDateLists = Directory.GetDirectories(job); //D:\picture_Backup\RB01\010\20-01-2022
                            foreach (var backupFile in backupDateLists)
                            {
                                var dateOnly = backupFile.Split('\\');  // 20-01-2022
                                if (dateOnly.Length == 5)
                                {
                                    var dateStr = dateOnly[4].Split('-');
                                    if (dateStr.Length == 3)
                                    {
                                        var listdate = new DateTime(Convert.ToInt32(dateStr[2].Trim()), Convert.ToInt32(dateStr[1].Trim()), Convert.ToInt32(dateStr[0].Trim()));
                                        if (listdate < datepoint)
                                        {
                                            Directory.Delete(backupFile, true);
                                            await Task.Delay(200);
                                        }


                                    }
                                }
                            }

                        }
                    }
                }
            }




            // Path 2

            string[] wrongJudement = Directory.GetDirectories(_destinationPath2); // D:\picture_Backup\wrong_judgment\21-01-2022
            foreach (string dir in wrongJudement)
            {
                var date = dir.Split('\\');
                if (date.Length == 4)
                {
                    var dd = date[3].Split('-');  //21-01-2022
                    if (dd.Length == 3)
                    {
                        var listdate = new DateTime(Convert.ToInt32(dd[2].Trim()), Convert.ToInt32(dd[1].Trim()), Convert.ToInt32(dd[0].Trim()));
                        if (listdate < datepoint)
                        {
                            Directory.Delete(dir, true);
                            await Task.Delay(200);
                        }


                    }
                }

            }

        }

        private async void Initialize()
        {
            _destinationPath1 = _configuration.GetValue<string>("DestinationPath:Path1");
            _destinationPath2 = _configuration.GetValue<string>("DestinationPath:Path2");
            _periodDays = _configuration.GetValue<int>("PeriodDays");
            _deleteAtHour = _configuration.GetValue<int>("DeleteAt:Hour");
            _deleteAtMinute = _configuration.GetValue<int>("DeleteAt:Minute");

            _isDelete = _configuration.GetValue<bool>("isDeleted");


            if (!Directory.Exists(_destinationPath1))
            {
                _logger.LogError($"No exiting DestinationPath1 => {_destinationPath1}");
                _logger.LogCritical("Exiting application...");
                _hostApplicationLifetime.StopApplication();
                await Task.Delay(5000);
            }
            if (!Directory.Exists(_destinationPath2))
            {
                _logger.LogError($"No exiting DestinationPath2 => {_destinationPath2}");
                _logger.LogCritical("Exiting application...");
                _hostApplicationLifetime.StopApplication();
                await Task.Delay(5000);
            }

            if ((_deleteAtHour >= 0 && _deleteAtHour < 24) && (_deleteAtMinute >= 0 && _deleteAtMinute < 60))
            {

            }
            else
            {
                _logger.LogError($" Hour : 0 - 23 , Minute : 0 - 59 ");
                _logger.LogCritical("Exiting application...");
                _hostApplicationLifetime.StopApplication();
                await Task.Delay(5000);
            }


      
        }
    }
}
