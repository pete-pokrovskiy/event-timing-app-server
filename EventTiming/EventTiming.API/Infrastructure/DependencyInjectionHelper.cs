using EventTiming.Logic.Contract.Events;
using EventTiming.Logic.Contract.Infra;
using EventTiming.Logic.Events.Commands;
using EventTiming.Logic.Events.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace EventTiming.API.Infrastructure
{
    public class DependencyInjectionHelper
    {
        public static void RegisterCommandDependencies(IServiceCollection services)
        {
            services.AddTransient<ICommandHandler<CreateEventCommand>, CreateEventCommandHandler>();
            services.AddTransient<ICommandHandler<UpdateEventCommand>, UpdateEventCommandHandler>();

        }

        internal static void RegisterQueryDependencies(IServiceCollection services)
        {
            services.AddTransient<IQueryHandler<GetEventQuery, GetEventQueryResult>, GetEventQueryHandler>();
            services.AddTransient<IQueryHandler<GetAllEventsQuery, GetAllEventsQueryResult>, GetAllEventsQueryHandler>();
        }
    }
}
