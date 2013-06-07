using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleLib.Annotations;

namespace CloudsdaleLib.Providers {
    public interface IMetadataProvider {
        IMetadataObject CreateNew();
    }

    public interface IMetadataObject {
        object Value { get; set; }
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
        public IMetadataObject CreateNew() {
            return new DefaultMetadataObject();
        }
    }

    internal class DefaultMetadataObject : IMetadataObject, INotifyPropertyChanged {
        private object _value;
        public object Value {
            get { return _value; }
            set {
                if (Equals(value, _value)) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
