using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudsdaleLib.Models {
    public interface IMergable {
        void Merge(CloudsdaleModel other);
        bool CanMerge(CloudsdaleModel other);
    }
}
