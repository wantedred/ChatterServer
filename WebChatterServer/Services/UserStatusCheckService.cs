using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebChatterServer.Models.Database.BindingModels;
using WebChatterServer.Models.Database.Contexts;

namespace WebChatterServer.Services
{
    public class UserStatusCheckTask : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        
        public UserStatusCheckTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private void RunCleanup(object state)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            MainContext context = scope.ServiceProvider.GetRequiredService<MainContext>();
            IQueryable<UserStatus> offlineUsers = context.UserStatuses.Where(s => s.Status == false);
            foreach (var user in offlineUsers)
            {
                context.UserStatuses.Remove(user);
            }
            context.SaveChanges();
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RunCleanup, null, TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}