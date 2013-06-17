﻿using System;
using System.Threading.Tasks;
using CloudsdaleLib.Helpers;

namespace CloudsdaleLib.Providers {
    public interface IModelErrorProvider {
        Task OnError<T>(WebResponse<T> response);
    }

    internal class DefaultModelErrorProvider : IModelErrorProvider {
        public Task OnError<T>(WebResponse<T> response) {
            throw new NotImplementedException("Model error handler not implemented");
        }
    }
}
