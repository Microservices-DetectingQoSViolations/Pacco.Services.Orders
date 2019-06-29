using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Convey;
using Convey.CQRS.Queries;
using Convey.Discovery.Consul;
using Convey.HTTP;
using Convey.LoadBalancing.Fabio;
using Convey.MessageBrokers.CQRS;
using Convey.MessageBrokers.RabbitMQ;
using Convey.Persistence.MongoDB;
using Convey.WebApi;
using Convey.WebApi.CQRS;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pacco.Services.Orders.Application;
using Pacco.Services.Orders.Application.Commands;
using Pacco.Services.Orders.Application.Events.External;
using Pacco.Services.Orders.Application.Services;
using Pacco.Services.Orders.Application.Services.Clients;
using Pacco.Services.Orders.Core.Repositories;
using Pacco.Services.Orders.Infrastructure.Mongo.Documents;
using Pacco.Services.Orders.Infrastructure.Mongo.Repositories;
using Pacco.Services.Orders.Infrastructure.Services;
using Pacco.Services.Orders.Infrastructure.Services.Clients;

namespace Pacco.Services.Orders.Infrastructure
{
    public static class Extensions
    {
        public static IConveyBuilder AddInfrastructure(this IConveyBuilder builder)
        {
            builder.Services.AddSingleton<IEventMapper, EventMapper>();
            builder.Services.AddTransient<IMessageBroker, MessageBroker>();
            builder.Services.AddTransient<IOrderRepository, OrderMongoRepository>();
            builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            builder.Services.AddTransient<IParcelsServiceClient, ParcelsServiceClient>();
            builder.Services.AddTransient<IPricingServiceClient, PricingServiceClient>();
            
            return builder
                .AddQueryHandlers()
                .AddInMemoryQueryDispatcher()
                .AddHttpClient()
                .AddConsul()
                .AddFabio()
                .AddRabbitMq()
                .AddMongo()
                .AddMongoRepository<OrderDocument, Guid>("Orders");
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseErrorHandler()
                .UsePublicContracts<ContractAttribute>()
                .UseInitializers()
                .UseConsul()
                .UseRabbitMq()
                .SubscribeCommand<ApproveOrder>()
                .SubscribeCommand<CreateOrder>()
                .SubscribeCommand<CancelOrder>()
                .SubscribeCommand<DeleteOrder>()
                .SubscribeCommand<AddParcelToOrder>()
                .SubscribeCommand<DeleteParcelFromOrder>()
                .SubscribeEvent<DeliveryCompleted>()
                .SubscribeEvent<DeliveryFailed>()
                .SubscribeEvent<DeliveryStarted>()
                .SubscribeEvent<ParcelDeleted>();

            return app;
        }

        internal sealed class MyMessageHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Console.WriteLine("AAAAA");
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
}