using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Documents;

namespace Cloudsdale_Metro.Helpers {
    public static class UIHelpers {
        public static T ScaleFor<T>(this TextPointer pointer) where T : Inline {
            var element = pointer.Parent as TextElement;
            while (element != null) {
                if (element is T) break;
                if (element.ContentStart == null || element == element.ElementStart.Parent) {
                    element = null;
                    continue;
                }

                element = element.ElementStart.Parent as TextElement;
            }
            return element as T;
        }
    }
}
