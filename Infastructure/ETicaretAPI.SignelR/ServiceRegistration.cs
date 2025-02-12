using ETicaretAPI.Application.Abstractions.Hubs;
using ETicaretAPI.SignelR.HubService;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.SignelR
{
    public static class ServiceRegistration
    {
        public static void AddSignelRServices(this IServiceCollection collection )
        {
            collection.AddTransient<IProductHubService, ProductHubService>();
            collection.AddSignalR();
        }
    }
}
