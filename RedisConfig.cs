using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkits.Redis {
    public abstract class RedisConfig : ConfigBase {
        public static string[] RedisGroups { get; set; }

        static RedisConfig() {
            RedisGroups = GetConfigInfo<string>("RedisGroups", new[] { "MINE" });
        }
    }
}
