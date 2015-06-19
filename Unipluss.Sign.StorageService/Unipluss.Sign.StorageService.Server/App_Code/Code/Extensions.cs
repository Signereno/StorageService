using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;

namespace Unipluss.Sign.StorageService.Server.Code
{
    public static class Extensions
    {

        public static void Serialize(this NameValueCollection metaData, string filepath)
        {
            File.WriteAllText(filepath, Newtonsoft.Json.JsonConvert.SerializeObject(metaData.AllKeys.ToDictionary(x => x, x => metaData[x])));
        }

        public static NameValueCollection DeSerialize(this string filepath)
        {
            var dict = Newtonsoft.Json.JsonConvert.DeserializeObject<IDictionary<string, string>>(File.ReadAllText(filepath));

            var collection = new NameValueCollection();
            foreach (var pair in dict)
                collection.Add(pair.Key, pair.Value.ToString());
            return collection;
        }

        public static void Retry<T>(this Action action, int retryLimit = 4, int sleepMillicsecond = 0) where T : Exception
        {
            int retry = 0;
            bool success = false;
            while (!success && retry <= retryLimit)
            {
                try
                {
                    action();
                    success = true;

                }
                catch (T t)
                {
                    if (!success && retry >= retryLimit)
                        throw t;

                    if (sleepMillicsecond > 0)
                        Thread.Sleep(sleepMillicsecond);
                    retry++;
                }
            }
        }

        public static R Retry<T, R>(this Func<R> action, int retryLimit = 4, int sleepMillicsecond = 0) where T : Exception
        {
            int retry = 0;
            bool success = false;
            R result = default(R);
            while (!success && retry <= retryLimit)
            {
                try
                {
                    result = action();
                    success = true;
                }
                catch (T t)
                {
                    if (!success && retry >= retryLimit)
                        throw t;

                    if (sleepMillicsecond > 0)
                        Thread.Sleep(sleepMillicsecond);
                    retry++;
                }
            }

            return result;
        }

        public static R Retry<T, T2, R>(this Func<R> action, int retryLimit = 4, int sleepMillicsecond = 0)
            where T : Exception
            where T2 : Exception
        {
            int retry = 0;
            bool success = false;
            R result = default(R);
            while (!success && retry <= retryLimit)
            {
                try
                {
                    result = action();
                    success = true;
                }
                catch (T t)
                {
                    if (!success && retry >= retryLimit)
                        throw t;

                    if (sleepMillicsecond > 0)
                        Thread.Sleep(sleepMillicsecond);
                    retry++;
                }
                catch (T2 t)
                {
                    if (!success && retry >= retryLimit)
                        throw t;

                    if (sleepMillicsecond > 0)
                        Thread.Sleep(sleepMillicsecond);
                    retry++;
                }
            }

            return result;
        }

        public static R Retry<T, T2, T3, R>(this Func<R> action, int retryLimit = 4, int sleepMillicsecond = 0)
            where T : Exception
            where T2 : Exception
            where T3 : Exception
        {
            int retry = 0;
            bool success = false;
            R result = default(R);
            while (!success && retry <= retryLimit)
            {
                try
                {
                    result = action();
                    success = true;
                }
                catch (T t)
                {
                    if (!success && retry >= retryLimit)
                        throw t;

                    if (sleepMillicsecond > 0)
                        Thread.Sleep(sleepMillicsecond);
                    retry++;
                }
                catch (T2 t)
                {
                    if (!success && retry >= retryLimit)
                        throw t;

                    if (sleepMillicsecond > 0)
                        Thread.Sleep(sleepMillicsecond);
                    retry++;
                }
                catch (T3 t)
                {
                    if (!success && retry >= retryLimit)
                        throw t;

                    if (sleepMillicsecond > 0)
                        Thread.Sleep(sleepMillicsecond);
                    retry++;
                }
            }

            return result;
        }
    }
}