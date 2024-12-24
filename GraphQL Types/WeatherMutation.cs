using GraphQL;
using GraphQL.Types;
using Weather_Acknowledgement.Models;

namespace Weather_Acknowledgement.GraphQL_Types
{
    public class WeatherMutation : ObjectGraphType
    {
        public WeatherMutation(WeatherDbContext dbContext)
        {
            Field<WeatherStationType>(
                "createWeatherStation",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "name" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "location" },
                    new QueryArgument<NonNullGraphType<FloatGraphType>> { Name = "latitude" },
                    new QueryArgument<NonNullGraphType<FloatGraphType>> { Name = "longitude" }
                ),
                resolve: context =>
                {
                    var station = new WeatherStation
                    {
                        Name = context.GetArgument<string>("name"),
                        Location = context.GetArgument<string>("location"),
                        Latitude = context.GetArgument<double>("latitude"),
                        Longitude = context.GetArgument<double>("longitude"),
                        InstallationDate = DateTime.UtcNow
                    };

                    dbContext.WeatherStations.Add(station);
                    dbContext.SaveChanges();
                    return station;
                }
            );

            Field<WeatherReadingType>(
                "createWeatherReading",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IntGraphType>> { Name = "stationId" },
                    new QueryArgument<NonNullGraphType<FloatGraphType>> { Name = "temperature" },
                    new QueryArgument<NonNullGraphType<FloatGraphType>> { Name = "humidity" },
                    new QueryArgument<NonNullGraphType<FloatGraphType>> { Name = "windSpeed" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "windDirection" },
                    new QueryArgument<NonNullGraphType<FloatGraphType>> { Name = "precipitation" }
                ),
                resolve: context =>
                {
                    var reading = new WeatherReading
                    {
                        WeatherStationId = context.GetArgument<int>("stationId"),
                        Temperature = context.GetArgument<double>("temperature"),
                        Humidity = context.GetArgument<double>("humidity"),
                        WindSpeed = context.GetArgument<double>("windSpeed"),
                        WindDirection = context.GetArgument<string>("windDirection"),
                        Precipitation = context.GetArgument<double>("precipitation"),
                        Timestamp = DateTime.UtcNow
                    };

                    dbContext.WeatherReadings.Add(reading);
                    dbContext.SaveChanges();
                    return reading;
                }
            );
        }
    }
}