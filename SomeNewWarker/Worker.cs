using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmailService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SomeNewWarker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEmailSender _sender;
        private FileSystemWatcher watcher;
        private readonly string path = @"C:\Users\Insan\Desktop\MyFolder";

        public Worker(ILogger<Worker> logger, IEmailSender sender)
        {
            _logger = logger;
            _sender = sender;
            
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.Created += OnChanged;
            return base.StartAsync(cancellationToken);
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("New message is about to send at: {time}", DateTimeOffset.Now);
            var message = new Message(new string[] { "newsomeone229@gmail.com" }, "MY Test Subject","Hello From Insan", e.FullPath);
            _sender.SendEmail(message);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path); 

                }
                watcher.EnableRaisingEvents = true;
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
