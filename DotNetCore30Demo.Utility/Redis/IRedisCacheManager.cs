using System.Threading.Tasks;

namespace DotNetCore30Demo.Utility.Redis
{
    /// <summary>
    /// Redis 缓存接口
    /// </summary>
    public interface IRedisCacheManager
    {
        /// <summary>获取指定 key 的值</summary>
        /// <typeparam name = "T" > byte[] 或其他类型</typeparam>
        /// <param name = "key" > 不含prefix前辍 </ param >
        /// < returns ></returns >
        T Get<T>(string key);

        /// <summary>获取指定 key 的值</summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);


        /// <summary>设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象</summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">值</param>
        /// <param name="expireSeconds">过期(秒单位)</param>
        /// <returns></returns>
        bool Set<T>(string key, T value, int expireSeconds = -1);

        /// <summary>设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象</summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">值</param>
        /// <param name="expireSeconds">过期(秒单位)</param>
        /// <returns></returns>
        Task<bool> SetAsync<T>(string key, T value, int expireSeconds = -1);

        /// <summary>用于在 key 存在时删除 key</summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        bool Del(params string[] key);

        /// <summary>用于在 key 存在时删除 key</summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        Task<bool> DelAsync(params string[] key);
    }
}