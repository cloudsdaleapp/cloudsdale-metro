﻿using System;
using Callisto.Controls;
using Cloudsdale_Metro.Helpers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Cloudsdale_Metro.Views.Controls {
    public class Hyperlink : Span {
        private InlineUIContainer marker;

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Hyperlink),
            new PropertyMetadata(default(string), TextChanged));

        private static void TextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args) {
            var hyperlink = (Hyperlink)dependencyObject;

            hyperlink.marker = new InlineUIContainer { Child = new Grid() };

            hyperlink.Inlines.Clear();
            hyperlink.Inlines.Add(hyperlink.marker);
            hyperlink.Inlines.Add(new Underline {
                Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x63, 0xA0, 0xD0)),
                Inlines = { new Run { Text = (string) args.NewValue } }
            });
        }

        public void TriggerLink(RichTextBlock rtb) {
            Uri uri;
            if (Uri.IsWellFormedUriString(Target, UriKind.Absolute)) {
                uri = new Uri(Target);
            } else if (Uri.IsWellFormedUriString("http://" + Target, UriKind.Absolute)) {
                uri = new Uri("http://" + Target);
            } else {
                return;
            }

// ReSharper disable ObjectCreationAsStatement
            new Flyout {
                PlacementTarget = rtb,
                Placement = PlacementMode.Top,
                HostMargin = new Thickness(0),
                Content = new Menu {
                    Items = {
                            new MenuItem {
                                Text = "Open Link",
                                Command = new OpenLinkCommand(),
                                CommandParameter = uri
                            },
                            new MenuItem {
                                Text = "Copy Link",
                                Command = new CopyLinkCommand(),
                                CommandParameter = uri
                            }
                        }
                },
                IsOpen = true
            };
// ReSharper restore ObjectCreationAsStatement
        }

        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(string), typeof(Hyperlink),
            new PropertyMetadata(default(string), TextChanged));

        public string Target {
            get { return (string)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }
    }
}
