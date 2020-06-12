using System.Text.Json;

namespace EZNEW.Web.Mvc
{
    /// <summary>
    /// Default json naming policy
    /// </summary>
    public class DefaultJsonNamingPolicy : JsonNamingPolicy
    {
        /// <summary>
        /// Convert name
        /// </summary>
        /// <param name="name">Original name</param>
        /// <returns>Return new name</returns>
        public override string ConvertName(string name)
        {
            return name;
        }
    }
}
