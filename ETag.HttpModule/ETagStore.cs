using System.Collections.Concurrent;

namespace ETag.HttpModule
{
    public class ETagStore
    {
        /// <summary>
        /// Key is resource key, value is ETag
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> ETags;

        static ETagStore()
        {
            ETags = new ConcurrentDictionary<string, string>();
        }

        public bool Exists(string resourceKey)
        {
            return ETags.ContainsKey(resourceKey);
        }

        public bool TryGet(string resourceKey, out string etag)
        {
            return ETags.TryGetValue(resourceKey, out etag);
        }

        public string AddOrUpdate(string resourceKey, string etag)
        {
            return ETags.AddOrUpdate(resourceKey, etag, (key, value) => etag);
        }
    }
}
