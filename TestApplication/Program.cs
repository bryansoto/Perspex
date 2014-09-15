﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Perspex;
using Perspex.Controls;
using Perspex.Layout;
using Perspex.Media;
using Perspex.Media.Imaging;
using Perspex.Threading;
using Perspex.Windows;
using Splat;

namespace TestApplication
{
    class TestLogger : ILogger
    {
        public LogLevel Level
        {
            get;
            set;
        }

        public void Write(string message, LogLevel logLevel)
        {
            if ((int)logLevel < (int)Level) return;
            System.Diagnostics.Debug.WriteLine(message);
        }
    }

    class Item
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    class Node
    {
        public Node()
        {
            this.Children = new PerspexList<Node>();
        }

        public string Name { get; set; }
        public PerspexList<Node> Children { get; set; }
    }

    class Program
    {
        private static PerspexList<Node> treeData = new PerspexList<Node>
        {
            new Node
            {
                Name = "Root 1",
                Children = new PerspexList<Node>
                {
                    new Node
                    {
                        Name = "Child 1",
                    },
                    new Node
                    {
                        Name = "Child 2",
                        Children = new PerspexList<Node>
                        {
                            new Node
                            {
                                Name = "Grandchild 1",
                            },
                            new Node
                            {
                                Name = "Grandmaster Flash",
                            },
                        }
                    },
                    new Node
                    {
                        Name = "Child 3",
                    },
                }
            },
            new Node
            {
                Name = "Root 2",
            },
        };

        static void Main(string[] args)
        {
            //Locator.CurrentMutable.Register(() => new TestLogger { Level = LogLevel.Debug } , typeof(ILogger));

            App application = new App
            {
                DataTemplates = new DataTemplates
                {
                    new TreeDataTemplate<Node>(
                        x => new TextBlock { Text = x.Name },
                        x => x.Children,
                        x => true),
                },
            };

            Window window = new Window
            {
                //Content = new TabControl
                //{
                //    Items = new[]
                //    {
                //        new TabItem
                //        {
                //            Header = "Buttons",
                //            Content = new StackPanel
                //            {
                //                Orientation = Orientation.Vertical,
                //                HorizontalAlignment = HorizontalAlignment.Center,
                //                VerticalAlignment = VerticalAlignment.Center,
                //                Gap = 8,
                //                MinWidth = 120,
                //                Children = new Controls
                //                {
                //                    new Button
                //                    {
                //                        Content = "Button",
                //                    },
                //                    new Button
                //                    {
                //                        Content = "Button",
                //                        Background = new SolidColorBrush(0xcc119eda),
                //                    },
                //                    new CheckBox
                //                    {
                //                        Content = "Checkbox",
                //                    },
                //                }
                //            },
                //        },
                //        new TabItem
                //        {
                //            Header = "Text",
                //            Content = new StackPanel
                //            {
                //                Orientation = Orientation.Vertical,
                //                HorizontalAlignment = HorizontalAlignment.Center,
                //                VerticalAlignment = VerticalAlignment.Center,
                //                Gap = 8,
                //                Width = 120,
                //                Children = new Controls
                //                {
                //                    new TextBlock
                //                    {
                //                        Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin venenatis dui quis libero suscipit tincidunt.",
                //                    },
                //                    new TextBox
                //                    {
                //                        Text = "Text Box",
                //                    },
                //                }
                //            },
                //        },
                //        new TabItem
                //        {
                //            Header = "Images",
                //            Content = new StackPanel
                //            {
                //                Orientation = Orientation.Vertical,
                //                HorizontalAlignment = HorizontalAlignment.Center,
                //                VerticalAlignment = VerticalAlignment.Center,
                //                Gap = 8,
                //                Width = 120,
                //                Children = new Controls
                //                {
                //                    new Image
                //                    {
                //                        Source = new Bitmap("github_icon.png"),
                //                        Width = 200,
                //                    },
                //                }
                //            },
                //        },
                //        new TabItem
                //        {
                //            Header = "Lists",
                //            IsSelected = true,
                //            Content = new StackPanel
                //            {
                //                Orientation = Orientation.Horizontal,
                //                HorizontalAlignment = HorizontalAlignment.Center,
                //                VerticalAlignment = VerticalAlignment.Center,
                //                Gap = 8,
                //                Children = new Controls
                //                {
                //                    new TreeView
                //                    {
                //                        Id = "treeView",
                //                        Items = treeData,
                //                    },
                //                    new StackPanel
                //                    {
                //                        Orientation = Orientation.Vertical,
                //                        Gap = 2.0,
                //                        Children = new Controls
                //                        {
                //                            new TextBox
                //                            {
                //                                Id = "newTreeViewItemText",
                //                                Text = "New Item"
                //                            },
                //                            new Button
                //                            {
                //                                Id = "addTreeViewItem",
                //                                Content = "Add",
                //                            },
                //                        }
                //                    },
                //                }
                //            },
                //        },
                //    }
                //}
                Content = new Border
                {
                    Background = Brushes.Chartreuse,
                    Width = 100,
                    Height = 100,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    RenderTransform = new RotateTransform(0),
                    Content = new Border
                    {
                        Background = Brushes.Chocolate,
                        Width = 50,
                        Height = 50,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                    }
                }
            };

            var border = (Border)window.Content;
            var rotate = (RotateTransform)border.RenderTransform;
            var timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(10);
            timer.Tick += (s, e) =>
            {
                rotate.Angle += 2;
                window.InvalidateVisual();
            };
            timer.Start();

            //var treeView = window.FindControl<TreeView>("treeView");
            //var newTreeViewItemText = window.FindControl<TextBox>("newTreeViewItemText");
            //var addTreeViewItem = window.FindControl<Button>("addTreeViewItem");

            //addTreeViewItem.Click += (s, e) =>
            //{
            //    if (treeView.SelectedItem != null)
            //    {
            //        ((Node)treeView.SelectedItem).Children.Add(new Node
            //        {
            //            Name = newTreeViewItemText.Text,
            //        });
            //    }
            //};

            window.Show();
            Application.Current.Run(window);
        }
    }
}
