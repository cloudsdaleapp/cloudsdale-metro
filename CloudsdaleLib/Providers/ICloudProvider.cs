using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudsdaleLib.Models;

namespace CloudsdaleLib.Providers {
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
