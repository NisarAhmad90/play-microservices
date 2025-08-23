using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Settings;

namespace Play.Common.MassTransit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumers(Assembly.GetEntryAssembly());

                x.UsingRabbitMq((context, cfg) =>
                {
                    // Retrieve RabbitMQ settings from configuration
                    var configuration = context.GetService<IConfiguration>();
                    var serviceSettings = configuration
                        .GetSection(nameof(ServiceSettings))
                        .Get<ServiceSettings>();

                    var rabbitMQSettings = configuration
                        .GetSection(nameof(RabbitMQSettings))
                        .Get<RabbitMQSettings>();

                    // Configure RabbitMQ host
                    cfg.Host(rabbitMQSettings.Host);

                    // Configure endpoints automatically based on consumers
                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }
    }
}
