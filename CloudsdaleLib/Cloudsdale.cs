﻿using CloudsdaleLib.Providers;

namespace CloudsdaleLib {
    public class Cloudsdale {
        private static readonly ISessionProvider DefaultSessionProvider = new DefaultSessionProvider();
        private static readonly ICloudServicesProvider DefaultCloudServicesProvider = new DefaultCloudServicesProvider();
        private static readonly IModelErrorProvider DefaultModelErrorProvider = new DefaultModelErrorProvider();

        private static ISessionProvider _sessionProvider;
        private static ICloudServicesProvider _cloudServicesProvider;
        private static IModelErrorProvider _modelErrorProvider;

        public static ISessionProvider SessionProvider {
            get { return _sessionProvider ?? DefaultSessionProvider; }
            set { _sessionProvider = value; }
        }

        public static ICloudServicesProvider CloudServicesProvider {
            get { return _cloudServicesProvider ?? DefaultCloudServicesProvider; }
            set { _cloudServicesProvider = value; }
        }

        public static IModelErrorProvider ModelErrorProvider {
            get { return _modelErrorProvider ?? DefaultModelErrorProvider; }
            set { _modelErrorProvider = value; }
        }
    }
}
