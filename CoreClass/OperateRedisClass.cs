using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CoreClass
{
    public class OperateRedisClass
    {
        string m_ip = "127.0.0.1";
        int m_port = 6379;
        string m_host = "";


        /// <summary>
        /// 初始化OperateRedisClass类型的新实例
        /// </summary>
        public OperateRedisClass()
        {
            
        }
        /// <summary>
        /// 初始化OperateRedisClass类型的新实例
        /// </summary>
        /// <param name="ip">连接地址</param>
        /// <param name="port">连接端口</param>
        public OperateRedisClass(string ip, int port)
        {
            m_ip = ip;
            m_port = port;
            m_host = m_ip + ":" + port;
        }
        /// <summary>
        /// 获取或设置redis的连接地址
        /// </summary>
        public string IP
        {
            set
            {
                m_ip = value;
                m_host = m_ip + ":" + m_port;
            }
            get
            {
                return m_ip;
            }
        }
        /// <summary>
        /// 获取或设置redis的连接端口
        /// </summary>
        public int Port
        {
            set
            {
                m_port = value;
                m_host = m_ip + ":" + m_port;
            }
            get
            {
                return m_port;
            }
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public string Get(string key)
        {
            string str = null;
            using (RedisClient redisClient = new RedisClient(m_host))
            {
                str = redisClient.Get<string>(key);
            }
            return str;
        }
        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            T str;
            using (RedisClient redisClient = new RedisClient(m_host))
            {
                str = redisClient.Get<T>(key);
            }
            return str;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Set(string key, string value)
        {
            bool b = false;
            using (RedisClient redisClient = new RedisClient(m_host))
            {
                b = redisClient.Set(key, value);
            }
            return b;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiresAt">过期时间到</param>
        /// <returns></returns>
        public bool Set(string key, string value, DateTime expiresAt)
        {
            bool b = false;
            using (RedisClient redisClient = new RedisClient(m_host))
            {
                b = redisClient.Set(key, value, expiresAt);
            }
            return b;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiresIn">过期时间</param>
        /// <returns></returns>
        public bool Set(string key, string value, TimeSpan expiresIn)
        {
            bool b = false;
            using (RedisClient redisClient = new RedisClient(m_host))
            {
                b = redisClient.Set(key, value, expiresIn);
            }
            return b;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value)
        {
            bool b = false;
            using (RedisClient redisClient = new RedisClient(m_host))
            {
                b = redisClient.Set<T>(key, value);
            }
            return b;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiresAt">过期时间到</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            bool b = false;
            using (RedisClient redisClient = new RedisClient(m_host))
            {
                b = redisClient.Set<T>(key, value, expiresAt);
            }
            return b;
        }
        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiresAt">过期时间</param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan expiresIn)
        {
            bool b = false;
            using (RedisClient redisClient = new RedisClient(m_host))
            {
                b = redisClient.Set<T>(key, value, expiresIn);
            }
            return b;
        }

    }
}
