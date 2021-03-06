﻿// -----------------------------------------------------------------------
// <copyright file="ScrollViewer.cs" company="Steven Kirk">
// Copyright 2014 MIT Licence. See licence.md for more information.
// </copyright>
// -----------------------------------------------------------------------

namespace Perspex.Controls
{
    using System;
    using System.Reactive.Linq;
    using Perspex.Controls.Presenters;
    using Perspex.Controls.Primitives;

    public class ScrollViewer : ContentControl
    {
        public static readonly PerspexProperty<Size> ExtentProperty =
            PerspexProperty.Register<ScrollViewer, Size>("Extent");

        public static readonly PerspexProperty<Vector> OffsetProperty =
            PerspexProperty.Register<ScrollViewer, Vector>("Offset", coerce: CoerceOffset);

        public static readonly PerspexProperty<Size> ViewportProperty =
            PerspexProperty.Register<ScrollViewer, Size>("Viewport");

        private ScrollContentPresenter presenter;

        private ScrollBar horizontalScrollBar;

        private ScrollBar verticalScrollBar;

        static ScrollViewer()
        {
            AffectsCoercion(ExtentProperty, OffsetProperty);
            AffectsCoercion(ViewportProperty, OffsetProperty);
        }

        public Size Extent
        {
            get { return this.GetValue(ExtentProperty); }
            private set { this.SetValue(ExtentProperty, value); }
        }

        public Vector Offset
        {
            get { return this.GetValue(OffsetProperty); }
            set { this.SetValue(OffsetProperty, value); }
        }

        public Size Viewport
        {
            get { return this.GetValue(ViewportProperty); }
            private set { this.SetValue(ViewportProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override void OnTemplateApplied()
        {
            this.presenter = this.GetTemplateChild<ScrollContentPresenter>("presenter");
            this.horizontalScrollBar = this.GetTemplateChild<ScrollBar>("horizontalScrollBar");
            this.verticalScrollBar = this.GetTemplateChild<ScrollBar>("verticalScrollBar");

            this[!ExtentProperty] = this.presenter[!ExtentProperty];
            this[!ViewportProperty] = this.presenter[!ViewportProperty];
            this.presenter[!OffsetProperty] = this[!OffsetProperty];

            var extentAndViewport = Observable.CombineLatest(
                this.GetObservable(ExtentProperty).StartWith(this.Extent),
                this.GetObservable(ViewportProperty).StartWith(this.Viewport))
                .Select(x => new { Extent = x[0], Viewport = x[1] });

            this.horizontalScrollBar.Bind(
                Visual.IsVisibleProperty,
                extentAndViewport.Select(x => x.Extent.Width > x.Viewport.Width));

            this.horizontalScrollBar.Bind(
                ScrollBar.MaximumProperty,
                extentAndViewport.Select(x => x.Extent.Width - x.Viewport.Width));

            this.horizontalScrollBar.Bind(
                ScrollBar.ViewportSizeProperty,
                extentAndViewport.Select(x => (x.Viewport.Width / x.Extent.Width) * (x.Extent.Width - x.Viewport.Width)));

            this.verticalScrollBar.Bind(
                Visual.IsVisibleProperty,
                extentAndViewport.Select(x => x.Extent.Height > x.Viewport.Height));

            this.verticalScrollBar.Bind(
                ScrollBar.MaximumProperty,
                extentAndViewport.Select(x => x.Extent.Height - x.Viewport.Height));

            this.verticalScrollBar.Bind(
                ScrollBar.ViewportSizeProperty,
                extentAndViewport.Select(x => (x.Viewport.Height / x.Extent.Height) * (x.Extent.Height - x.Viewport.Height)));

            var offset = Observable.CombineLatest(
                this.horizontalScrollBar.GetObservable(ScrollBar.ValueProperty),
                this.verticalScrollBar.GetObservable(ScrollBar.ValueProperty))
                .Select(x => new Vector(x[0], x[1]));

            this.presenter.GetObservable(ScrollContentPresenter.OffsetProperty).Subscribe(x =>
            {
                this.horizontalScrollBar.Value = x.X;
                this.verticalScrollBar.Value = x.Y;
            });

            this.Bind(OffsetProperty, offset);
        }

        private static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private static Vector CoerceOffset(PerspexObject o, Vector value)
        {
            ScrollViewer scrollViewer = o as ScrollViewer;

            if (scrollViewer != null)
            {
                var extent = scrollViewer.Extent;
                var viewport = scrollViewer.Viewport;
                var maxX = Math.Max(extent.Width - viewport.Width, 0);
                var maxY = Math.Max(extent.Height - viewport.Height, 0);
                return new Vector(Clamp(value.X, 0, maxX), Clamp(value.Y, 0, maxY));
            }
            else
            {
                return value;
            }
        }
    }
}
