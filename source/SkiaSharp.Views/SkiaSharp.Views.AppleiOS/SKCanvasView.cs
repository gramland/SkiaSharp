﻿using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

#if __IOS__
namespace SkiaSharp.Views.iOS
#elif __TVOS__
namespace SkiaSharp.Views.tvOS
#endif
{
	[Register(nameof(SKCanvasView))]
	[DesignTimeVisible(true)]
	public class SKCanvasView : UIView, IComponent
	{
		// for IComponent
		private event EventHandler DisposedInternal;
		ISite IComponent.Site { get; set; }
		event EventHandler IComponent.Disposed
		{
			add { DisposedInternal += value; }
			remove { DisposedInternal -= value; }
		}
		private bool designMode;

		private SKDrawable drawable;

		// created in code
		public SKCanvasView()
		{
			Initialize();
		}

		// created in code
		public SKCanvasView(CGRect frame)
			: base(frame)
		{
			Initialize();
		}

		// created via designer
		public SKCanvasView(IntPtr p)
			: base(p)
		{
		}

		// created via designer
		public override void AwakeFromNib()
		{
			designMode = ((IComponent)this).Site?.DesignMode == true;

			Initialize();
		}

		private void Initialize()
		{
			drawable = new SKDrawable();
		}

		public SKSize CanvasSize => drawable.Info.Size;

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);

			if (designMode)
				return;

			var ctx = UIGraphics.GetCurrentContext();

			// create the skia context
			SKImageInfo info;
			var surface = drawable.CreateSurface(Bounds, ContentScaleFactor, out info);

			// draw on the image using SKiaSharp
			DrawInSurface(surface, info);

			// draw the surface to the context
			drawable.DrawSurface(ctx, Bounds, info, surface);
		}

		public event EventHandler<SKPaintSurfaceEventArgs> PaintSurface;

		public virtual void DrawInSurface(SKSurface surface, SKImageInfo info)
		{
			PaintSurface?.Invoke(this, new SKPaintSurfaceEventArgs(surface, info));
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			Layer.SetNeedsDisplay();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			drawable.Dispose();
		}
	}
}
