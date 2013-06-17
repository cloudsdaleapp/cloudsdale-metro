using CloudsdaleLib.Models;

namespace CloudsdaleLib.Providers {
    /// <summary>
    /// Provides the models access to the backing cache of clouds
    /// </summary>
    public interface ICloudProvider {
        Cloud GetCloud(string cloudId);
        Cloud UpdateCloud(Cloud cloud);
    }

    class DefaultCloudProvider : ICloudProvider {
        public Cloud GetCloud(string cloudId) {
            return null;
        }

        public Cloud UpdateCloud(Cloud cloud) {
            return cloud;
        }
    }
}
