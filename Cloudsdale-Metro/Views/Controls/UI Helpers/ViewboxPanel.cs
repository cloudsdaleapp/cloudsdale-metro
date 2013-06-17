using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Cloudsdale_Metro.Views.Controls {
    public class ViewboxPanel : Panel {
        private double scale;

        protected override Size MeasureOverride(Size availableSize) {
            if (availableSize.Width < 5) {
                scale = 1;
                return availableSize;
            }

            double width = 0;
            double height = 0;
            var unlimitedSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (var child in Children) {
                child.Measure(unlimitedSize);
                width += child.DesiredSize.Width;
                height += child.DesiredSize.Height;
            }
            scale = Math.Min(availableSize.Width / width, availableSize.Height / height);

            foreach (var child in Children) {
                unlimitedSize.Width = availableSize.Width / scale;
                unlimitedSize.Height = availableSize.Height / scale;
                child.Measure(unlimitedSize);
            }

            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize) {
            var scaleTransform = new ScaleTransform { ScaleX = scale, ScaleY = scale };
            double width = 0;
            foreach (var child in Children) {
                child.RenderTransform = scaleTransform;
                child.Arrange(new Rect(new Point(scale * width, 0),
                              new Size(finalSize.Width / scale, finalSize.Height / scale)));
                width += child.DesiredSize.Width;
            }

            return finalSize;
        }
    }
}
