using GraphQL;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using Weather_Acknowledgement.Models;

namespace Weather_Acknowledgement.GraphQL_Types
{
    public class WeatherQuery : ObjectGraphType
    {
        public WeatherQuery(WeatherDbContext dbContext)
        {
            Field<ListGraphType<WeatherStationType>>(
                "weatherStations",
                resolve: context => dbContext.WeatherStations.Include(w => w.Readings).ToListAsync()
            );

            Field<WeatherStationType>(
                "weatherStation",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "id" }
                ),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");
                    return dbContext.WeatherStations
                        .Include(w => w.Readings)
                        .FirstOrDefaultAsync(w => w.Id == id);
                }
            );

            Field<ListGraphType<WeatherReadingType>>(
                "weatherReadings",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> { Name = "stationId" },
                    new QueryArgument<DateGraphType> { Name = "fromDate" },
                    new QueryArgument<DateGraphType> { Name = "toDate" }
                ),
                resolve: context =>
                {
                    var query = dbContext.WeatherReadings.AsQueryable();

                    var stationId = context.GetArgument<int?>("stationId");
                    if (stationId.HasValue)
                        query = query.Where(r => r.WeatherStationId == stationId.Value);

                    var fromDate = context.GetArgument<DateTime?>("fromDate");
                    if (fromDate.HasValue)
                        query = query.Where(r => r.Timestamp >= fromDate.Value);

                    var toDate = context.GetArgument<DateTime?>("toDate");
                    if (toDate.HasValue)
                        query = query.Where(r => r.Timestamp <= toDate.Value);

                    return query.Include(r => r.Temperature).ToListAsync();
                }
            );
        }
    }
}
