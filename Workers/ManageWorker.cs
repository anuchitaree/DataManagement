using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManagement.Workers
{
    public class ManageWorker : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CleanWorker> _logger;

        private string _destinationPath1 = null!;
        private string _destinationPath2 = null!;

        private int _deleteAtDayOfWeek = 0;
        private int _deleteAtHour = 0;
        private int _deleteAtMinute = 0;

        private int _periodDays = 0;
        private Boolean _isDelete = false;

        private readonly IHostApplicationLifetime _hostApplicationLifetime;


        public ManageWorker(IHostApplicationLifetime hostApplicationLifetime,
           ILogger<CleanWorker> logger, IConfiguration configuration)
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
                    var dayOfWeek = (int)DateTime.Now.DayOfWeek;

                    bool resultday = dayOfWeek == _deleteAtDayOfWeek || _deleteAtDayOfWeek == 8;


                    int nowDay = DateTime.Now.Day;

                    int nowMintue = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

                    if (lastDay != nowDay && nowMintue > setMinute && resultday)
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

        private  void Proceesing()
        {
            var deldate = DateTime.Now.AddDays(-_periodDays);
            //var datepoint = new DateTime(deldate.Year, deldate.Month, deldate.Day, 0, 0, 0);
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

                            //D:\picture_Backup\RB01\010\OK-RB01-508-xx xxx xxx x_20210502_124817.jpg
                            string[] filerecords = Directory.GetFiles(job); 

                            foreach (var backupFile in filerecords)//
                            {
                                var fileName = backupFile.Split('\\');  // OK-RB01-508-xx xxx xxx x_20210502_124817.jpg
                                if (fileName.Length == 5)
                                {
                                    int length = fileName[4].Length;
                                    string yyyyMMdd_HHmmss = fileName[4].Substring(length-15, 15);

                                    int yyyy = Convert.ToInt32(yyyyMMdd_HHmmss.Substring(0,2));
                                    int MM = Convert.ToInt32(yyyyMMdd_HHmmss.Substring(2, 2));
                                    int dd = Convert.ToInt32(yyyyMMdd_HHmmss.Substring(4, 2));
                                    int HH= Convert.ToInt32(yyyyMMdd_HHmmss.Substring(7, 2));
                                    int mm = Convert.ToInt32(yyyyMMdd_HHmmss.Substring(9, 2));
                                    int ss = Convert.ToInt32(yyyyMMdd_HHmmss.Substring(11, 2));

                                    DateTime fileDate = new DateTime(yyyy, MM, dd, HH, mm, ss);


                                  
                                }
                            }

                        }
                    }
                }
            }



        }
        private async void Initialize()
        {
            _destinationPath1 = _configuration.GetValue<string>("DestinationPath:Path1");
            _destinationPath2 = _configuration.GetValue<string>("DestinationPath:Path2");

            _deleteAtDayOfWeek = _configuration.GetValue<int>("DeleteAt:DayOfWeek");
            _deleteAtHour = _configuration.GetValue<int>("DeleteAt:Hour");
            _deleteAtMinute = _configuration.GetValue<int>("DeleteAt:Minute");

            _periodDays = _configuration.GetValue<int>("PeriodDays");
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

            bool isDayOfWeek = _deleteAtDayOfWeek >= 0 && _deleteAtDayOfWeek <= 8;
            if (!isDayOfWeek)
            {
                _logger.LogError($" Day of week to delete file : 0 to 8");
                _logger.LogCritical("Exiting application...");
                _hostApplicationLifetime.StopApplication();
                await Task.Delay(5000);
            }
            bool isHour = _deleteAtHour >= 0 && _deleteAtHour < 24;
            if (!isHour)
            {
                _logger.LogError($" Hour : 0 to 23");
                _logger.LogCritical("Exiting application...");
                _hostApplicationLifetime.StopApplication();
                await Task.Delay(5000);
            }
            bool isMinute = _deleteAtMinute >= 0 && _deleteAtMinute < 60;
            if (!isMinute)
            {
                _logger.LogError($" Minute : 0 to 59");
                _logger.LogCritical("Exiting application...");
                _hostApplicationLifetime.StopApplication();
                await Task.Delay(5000);
            }

            //=============== license protect =============//
            DateTime setDate = new DateTime(2023, 3, 1, 0, 0, 0);
            if (DateTime.Now > setDate)
            {
                _logger.LogCritical("Exiting application...");
                _hostApplicationLifetime.StopApplication();
            }

        }

    }
}
