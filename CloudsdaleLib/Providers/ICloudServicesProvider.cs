using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudsdaleLib.Providers {
    public interface ICloudServicesProvider {
        IStatusProvider StatusProvider { get; }
    }

    internal class DefaultCloudServicesProvider : ICloudServicesProvider {
        private static readonly DefaultStatusProvider DefaultStatusProvider = new DefaultStatusProvider();

        public IStatusProvider StatusProvider { get { return DefaultStatusProvider; } }
    }
}
