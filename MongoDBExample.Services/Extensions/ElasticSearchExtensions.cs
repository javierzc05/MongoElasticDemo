using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDBExample.Entities;
using Nest;

namespace MongoDBExample.Services.Extensions;

public static class ElasticSearchExtensions
{
    public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
    {
        var url = configuration["ELKConfiguration:Uri"];

        if (string.IsNullOrEmpty(url)) 
        { 
             throw new ApplicationException("Uri is not defined in appsettings.json");
        }

        var concertsIndex = configuration["ELKConfiguration:ConcertsIndex"];
        var settings = new ConnectionSettings(new Uri(url)).PrettyJson().DefaultIndex(concertsIndex);
        var client = new ElasticClient(settings);

        services.AddSingleton<IElasticClient>(client);
        client.Indices.Create(concertsIndex, c => c.Map<Concert>(m => m.AutoMap()));
    }

    private static void AddDefaultMapping(this ConnectionSettings settings)
    {
        settings.DefaultMappingFor<Concert>(c => 
                c.Ignore(x => x.Id)
                    .Ignore(x => x.TicketsQuantity));
    }
}