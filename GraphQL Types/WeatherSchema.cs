using GraphQL.Types;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Weather_Acknowledgement.Models;

namespace Weather_Acknowledgement.GraphQL_Types
{
    public class WeatherSchema : GraphQL.Types.Schema
    {
        public WeatherSchema(IServiceProvider serviceProvider, WeatherQuery query, WeatherMutation mutation)
            : base(serviceProvider)
        {
            Query = query;
            Mutation = mutation;
        }
    }
}
