using System;
using System.Collections.Generic;
using System.Diagnostics;
using StackExchange.Redis;
using Toolkits.Log;

namespace Toolkits.Redis {

    /// <summary> 
    /// </summary>
    public class RedisHelper {
        private static RedisHelper _Instance = new RedisHelper();

        /// <summary>
        /// 连接池
        /// </summary>
        private Dictionary<string, ConnectionMultiplexer> Manager = null;

        public RedisHelper() {
            if (null == Manager) {
                try {
                    Manager = new Dictionary<string, ConnectionMultiplexer>();
                    if (null != RedisConfig.RedisGroups) {

                        Console.WriteLine(RedisConfig.RedisGroups.Length);
                        foreach (var curGroup in RedisConfig.RedisGroups) {
                            try {
                                var connStr = RedisConfig.GetConfigInfo<string>(curGroup, "");
                                Console.WriteLine($"{curGroup}:{connStr}");

                                if (!string.IsNullOrEmpty(connStr)) {
                                    var cur = ConnectionMultiplexer.ConnectAsync(connStr);
                                    Manager[curGroup] = cur.Result;
                                }
                            } catch (Exception ex) {
                                LogHelper.Instance.Error("[RedisHelper][RedisHelper][" + curGroup + "]", ex);
                            }
                        }
                    }
                } catch (Exception ex) {
                    LogHelper.Instance.Error("[RedisHelper][RedisHelper]", ex);
                }
            }
        }

        /// <summary>
        /// 全局单例
        /// </summary>
        public static RedisHelper Instance {
            get {
                if (_Instance == null) {
                    _Instance = new RedisHelper();
                }
                return _Instance;
            }
        }

        /// <summary>
        ///  重新构造
        /// </summary>
        public void ReConstruct() {
            _Instance = null;
        }

        /// <summary>
        /// 获取一个可用的 IDatabase
        /// </summary>
        public IDatabase GetDb(string redisGroup = "", int dbIndex = 0) {
            IDatabase db = null;
            try {
                ConnectionMultiplexer cur = null;
                if (null != Manager && Manager.TryGetValue(redisGroup, out cur))
                    db = cur.GetDatabase(dbIndex);

            } catch (Exception ex) {
                LogHelper.Instance.Error("[RedisHelper][GetDb]", ex);

                ReConstruct();

                LogHelper.Instance.Warn("[RedisHelper][GetDb]: 重建到Redis的连接");
            }

            return db;
        }

        /// <summary>
        /// 获取一个可用的 ISubscriber
        /// </summary>
        public ISubscriber GetSubscribe(string redisGroup = "") {
            ISubscriber sub = null;
            try {
                ConnectionMultiplexer cur = null;
                if (null != Manager && Manager.TryGetValue(redisGroup, out cur))
                    sub = cur.GetSubscriber();

            } catch (Exception ex) {
                LogHelper.Instance.Error("[RedisHelper][GetSubscribe]", ex);

                ReConstruct();

                LogHelper.Instance.Warn("[RedisHelper][GetSubscribe]: 重建到Redis的连接");
            }

            return sub;
        }

        public object GetDb(string v, object eM_LIVE_DbIndex) {
            throw new NotImplementedException();
        }
    }
}