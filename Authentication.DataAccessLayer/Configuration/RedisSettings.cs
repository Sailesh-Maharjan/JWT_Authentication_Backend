using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.DataAccessLayer.Configuration
{
    public class RedisSettings
    {
        public string ConnectionString {  get; set; } =string.Empty;
        public string InstanceName {  get; set; } =string.Empty;

    }
}
