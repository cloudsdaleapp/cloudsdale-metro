using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CloudsdaleLib.Annotations;
using CloudsdaleLib.Models;

namespace CloudsdaleLib.Providers {
    public interface IMetadataProvider {
        IMetadataObject CreateNew(CloudsdaleModel model);
    }

    public interface IMetadataObject {
        object Value { get; set; }
        CloudsdaleModel Model { get; }
    }

    public interface IMetadataProviderStore {
        IMetadataProvider this[string key] { get; set; }
    }

    internal class MetadataProviderStore : IMetadataProviderStore {
        private readonly Dictionary<string, IMetadataProvider> providers = new Dictionary<string, IMetadataProvider>();

        public IMetadataProvider this[string key] {
            get {
                return providers.ContainsKey(key) ? providers[key] : providers[key] = new DefaultMetadataProvider();
            }
            set { providers[key] = value; }
        }
    }

    internal class DefaultMetadataProvider : IMetadataProvider {
        public IMetadataObject CreateNew(CloudsdaleModel model) {
            return new DefaultMetadataObject(model);
        }
    }

    internal class DefaultMetadataObject : IMetadataObject, INotifyPropertyChanged {
        public DefaultMetadataObject(CloudsdaleModel model) {
            Model = model;
        }

        private object _value;
        public object Value {
            get { return _value; }
            set {
                if (Equals(value, _value)) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        public CloudsdaleModel Model { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
