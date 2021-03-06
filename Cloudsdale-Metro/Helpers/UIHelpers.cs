﻿using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Cloudsdale_Metro.Helpers {
    public static class UIHelpers {
        public static T ScaleFor<T>(this RichTextBlock rtb, PointerRoutedEventArgs args) where T : Inline {
            var point = args.GetCurrentPoint(rtb);
            return rtb.GetPositionFromPoint(point.Position).ScaleFor<T>();
        }

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

        public static IEnumerable<T> Children<T>(this DependencyObject parent) where T : DependencyObject {
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childCount; ++i) {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T) {
                    yield return child as T;
                }
                foreach (var subChild in child.Children<T>()) {
                    yield return subChild;
                }
            }
        } 
    }
}
