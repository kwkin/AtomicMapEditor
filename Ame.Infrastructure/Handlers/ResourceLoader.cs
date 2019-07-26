using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Models.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Handlers
{
    // TODO change to DI
    public sealed class ResourceLoader
    {
        private static readonly Lazy<ResourceLoader> lazy = new Lazy<ResourceLoader>(() => new ResourceLoader());

        public static ResourceLoader Instance
        {
            get
            {
                return lazy.Value;
            }
        }


        #region constructor

        private ResourceLoader()
        {
            loaded = new Dictionary<string, object>();
        }

        #endregion constructor


        #region fields

        private IDictionary<string, object> loaded;

        #endregion fields


        #region properties

        #endregion properties


        #region methods

        public T Load<T>(string path, IResourceReader<T> reader)
        {
            object item;
            if (IsLoaded(path))
            {
                item = this.loaded[path];
            }
            else
            {
                item = reader.Read(path);
                this.loaded.Add(path, item);
            }
            return (T)item;
        }

        public T Load<T>(string path, T item)
        {
            if (IsLoaded(path))
            {
                item = (T)this.loaded[path];
            }
            else
            {
                this.loaded.Add(path, item);
            }
            return item;
        }

        public bool ForceLoad<T>(string path, IResourceReader<T> reader)
        {
            bool isReplaced = this.loaded.Remove(path);
            object item = reader.Read(path);
            this.loaded.Add(path, item);
            return isReplaced;
        }

        public bool ForceLoad<T>(string path, T item)
        {
            bool isReplaced = this.loaded.Remove(path);
            this.loaded.Add(path, item);
            return isReplaced;
        }

        public bool Unload(string path)
        {
            return this.loaded.Remove(path);
        }

        public bool IsLoaded(string path)
        {
            return this.loaded.ContainsKey(path);
        }

        public DateTime GetLastModified(string path)
        {
            return File.GetLastWriteTime(path);
        }

        #endregion methods
    }
}
