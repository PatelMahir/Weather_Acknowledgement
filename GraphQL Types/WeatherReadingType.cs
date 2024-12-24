using GraphQL.Types;
using Weather_Acknowledgement.Models;

namespace Weather_Acknowledgement.GraphQL_Types
{
    public class WeatherReadingType : ObjectGraphType<WeatherReading>
    {
        public WeatherReadingType()
        {
            Field(x => x.Id);
            Field(x => x.WeatherStationId);
            Field(x => x.Timestamp);
            Field(x => x.Temperature);
            Field(x => x.Humidity);
            Field(x => x.WindSpeed);
            Field(x => x.WindDirection);
            Field(x => x.Precipitation);
            Field<WeatherStationType>("weatherStation");
        }
    }
}
