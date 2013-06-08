﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleLib.Annotations;
using CloudsdaleLib.Models;
using CloudsdaleLib.Providers;
using Windows.UI.Xaml;

namespace Cloudsdale_Metro.Models {
    public class BooleanMetadataProvider : IMetadataProvider {
        public IMetadataObject CreateNew(CloudsdaleModel model) {
            return new BooleanMetadata(model);
        }

        public class BooleanMetadata : IMetadataObject, INotifyPropertyChanged {
            private bool _value;
            public event PropertyChangedEventHandler PropertyChanged;

            public BooleanMetadata(CloudsdaleModel model) {
                Model = model;
            }

            public object Value {
                get { return _value; }
                set {
                    if (Equals(value, _value)) return;
                    _value = (bool)value;
                    OnPropertyChanged();
                }
            }

            public CloudsdaleModel Model { get; private set; }

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}