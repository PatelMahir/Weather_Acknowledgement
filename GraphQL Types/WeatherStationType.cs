using GraphQL.Types;
using Weather_Acknowledgement.Models;

namespace Weather_Acknowledgement.GraphQL_Types
{
    public class WeatherStationType : ObjectGraphType<WeatherStation>
    {
        public WeatherStationType()
        {
            Field(x => x.Id);
            Field(x => x.Name);
            Field(x => x.Location);
            Field(x => x.Latitude);
            Field(x => x.Longitude);
            Field(x => x.InstallationDate);
            Field<ListGraphType<WeatherReadingType>>("readings");
        }
    }
}
