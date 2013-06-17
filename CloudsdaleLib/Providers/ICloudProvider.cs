﻿using CloudsdaleLib.Models;

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
