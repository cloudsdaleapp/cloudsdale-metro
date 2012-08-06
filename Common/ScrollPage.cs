using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cloudsdale.Common {
    public class ScrollPage {
        DispatcherTimer Timer { get; set; }
        double NewVerticalOffset { get; set; }
        double VerticalOffsetIncrement { get; set; }
        double CurrentVerticalOffset { get; set; }
        ScrollViewer ScrollViewer { get; set; }
        double ViewportHeight { get; set; }
        double VerticalOffset { get; set; }
        double Intervals { get; set; }

        public ScrollPage(ScrollViewer scrollViewer, double durationInSeconds) {
            ScrollViewer = scrollViewer;
            Intervals = durationInSeconds * 120; // number of timer Intervals
            Timer = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(1.0/120.0)
            };
            Timer.Tick += TimerTick;
        }

        // This member function scrolls the ScrollViewer up ViewportHeight - 75
        // device-independent units so that there is overlap between one view and the next.
        public void Up() {
            // The user can change this between expansions/collapses.
            ViewportHeight = ScrollViewer.ViewportHeight;

            // The user can change this by moving the thumb control.
            // Equivalent to the data type Animation.From property.
            VerticalOffset = ScrollViewer.VerticalOffset;

            // Equivalent to the data type Animation.To property.
            NewVerticalOffset = VerticalOffset - ViewportHeight + 75;

            // We don't want to try to scroll out of the ScrollViewer.
            if (NewVerticalOffset < 0) {
                NewVerticalOffset = 0;
            }
            VerticalOffsetIncrement = (NewVerticalOffset - VerticalOffset) / Intervals;
            if (Math.Abs(VerticalOffsetIncrement - 0.0) < .1) {
                return;
            }
            CurrentVerticalOffset = VerticalOffset;
            Timer.Start();
        }

        // This member function scrolls the ScrollViewer down ViewportHeight - 75
        // device-independent units so that there is overlap between one view and the next.
        public void Down() {
            // The user can change this between expansions/collapses.
            ViewportHeight = ScrollViewer.ViewportHeight;

            // The user can change this by moving the thumb control.
            // Equivalent to the data type Animation.From property.
            VerticalOffset = ScrollViewer.VerticalOffset;

            // Equivalent to the data type Animation.To property.
            NewVerticalOffset = VerticalOffset + ViewportHeight - 75;

            // We don't want to try to scroll out of the ScrollViewer.
            if (NewVerticalOffset > ScrollViewer.ExtentHeight) {
                NewVerticalOffset = ScrollViewer.ExtentHeight;
            }
            VerticalOffsetIncrement = (NewVerticalOffset - VerticalOffset) / Intervals;
            if (Math.Abs(VerticalOffsetIncrement - 0.0) < .1) {
                return;
            }
            CurrentVerticalOffset = VerticalOffset;
            Timer.Start();
        }

        public void Bottom() {
            // The user can change this between expansions/collapses.
            ViewportHeight = ScrollViewer.ViewportHeight;

            // The user can change this by moving the thumb control.
            // Equivalent to the data type Animation.From property.
            VerticalOffset = ScrollViewer.VerticalOffset;

            // Equivalent to the data type Animation.To property.
            NewVerticalOffset = ScrollViewer.ExtentHeight;

            // We don't want to try to scroll out of the ScrollViewer.
            if (NewVerticalOffset > ScrollViewer.ExtentHeight) {
                NewVerticalOffset = ScrollViewer.ExtentHeight;
            }
            VerticalOffsetIncrement = (NewVerticalOffset - VerticalOffset) / Intervals;
            if (Math.Abs(VerticalOffsetIncrement - 0.0) < .1) {
                return;
            }
            CurrentVerticalOffset = VerticalOffset;
            Timer.Start();
        }

        void TimerTick(object sender, object o) {
            CurrentVerticalOffset += VerticalOffsetIncrement;
            if (VerticalOffsetIncrement > 0 && CurrentVerticalOffset > NewVerticalOffset ||
                VerticalOffsetIncrement < 0 && NewVerticalOffset > CurrentVerticalOffset) {
                Timer.Stop();
            } else {
                ScrollViewer.ScrollToVerticalOffset(CurrentVerticalOffset);
            }
        }
    }
}