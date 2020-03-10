using System.Configuration;

namespace CityDBTest
{
    public static class Helper
    {
        public static string ConnectionVal(string name)
        {

            var output = ConfigurationManager.ConnectionStrings[name].ConnectionString;
            return output;
        }




    }
}