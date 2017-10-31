using System.Configuration;
using System.Web;
using System.Web.Configuration;

namespace Krista.FM.Server.Dashboards.Core
{
    public class RegionSettings : ConfigurationSection
    {
        [ConfigurationProperty("id")]
        public string Id
        {
            get
            {
                return base["id"].ToString();
            }
            set
            {
                base["id"] = value;
            }
        }

        [ConfigurationProperty("name")]
        public string Name
        {
            get
            {
                return base["name"].ToString();
            }
            set
            {
                base["name"] = value;
            }
        }

        [ConfigurationProperty("mdx")]
        public string mdx
        {
            get
            {
                return base["mdx"].ToString();
            }
            set
            {
                base["mdx"] = value;
            }
        }

        private static RegionSettings instance;

        public static RegionSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    Configuration config =
                        WebConfigurationManager.OpenWebConfiguration(
                            HttpContext.Current.Request.ApplicationPath);
                    instance = (RegionSettings) config.GetSection("regionSettings");
                }
                return instance;
            }
        }
    }
}
