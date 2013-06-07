using System.Collections.Generic;
using CloudsdaleLib.Helpers;
using CloudsdaleLib.Providers;

namespace CloudsdaleLib.Models {
    public class UIMetadata {
        private readonly Dictionary<string, IMetadataObject> objects = new Dictionary<string, IMetadataObject>();  

        public IMetadataObject this[string key] {
            get {
                return objects.ContainsKey(key)
                           ? objects[key]
                           : objects[key] = Cloudsdale.MetadataProviders[key].CreateNew();
            }
        }
    }
}
