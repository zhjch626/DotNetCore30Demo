using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace DotNetCore30Demo.Utility.Helper
{
    public class THashAlgorithmInstances<THashAlgorithm> where THashAlgorithm : HashAlgorithm
    {
        /// <summary>
        /// 线程静态变量。
        /// 即：这个变量在每个线程中都是唯一的。
        /// 再结合泛型类实现了该变量在不同泛型或不同的线程先的变量都是唯一的。
        /// 这样做的目的是为了避开多线程问题。
        /// </summary>
        [ThreadStatic]
        static THashAlgorithm instance;

        public static THashAlgorithm Instance => instance ?? Create();

        /// <summary>
        /// 寻找 THashAlgorithm 类型下的 Create 静态方法，并执行它。
        /// 如果没找到，则执行 Activator.CreateInstance 调用构造方法创建实例。
        /// 如果 Activator.CreateInstance 方法执行失败，它会抛出异常。
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        static THashAlgorithm Create()
        {
            var createMethod = typeof(THashAlgorithm).GetMethod(
                nameof(HashAlgorithm.Create),
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly,
                Type.DefaultBinder,
                Type.EmptyTypes,
                null);

            if (createMethod != null)
            {
                instance = (THashAlgorithm)createMethod.Invoke(null, new object[] { });
            }
            else
            {
                instance = Activator.CreateInstance<THashAlgorithm>();
            }

            return instance;
        }
    }
}
