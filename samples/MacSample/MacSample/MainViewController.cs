using System;
using AppKit;
using SkiaSharp;
using SkiaSharp.Views.Mac;

namespace SkiaSharpSample.MacSample
{
	public partial class MainViewController : NSViewController
	{
		public static AppDelegate App => (AppDelegate)NSApplication.SharedApplication.Delegate;

		private SampleBase sample;

		public MainViewController(IntPtr handle)
			: base(handle)
		{
		}

		public void SetSample(SampleBase newSample)
		{
			sample = newSample;

			// set the title
			var title = sample?.Title ?? "SkiaSharp for MacOS";
			Title = title;
			var window = View?.Window;
			if (window != null)
			{
				window.Title = title;
			}

			// prepare the sample
			sample?.Init(() =>
			{
				// refresh the view
				canvas.SetNeedsDisplayInRect(canvas.Bounds);
				glview.SetNeedsDisplayInRect(glview.Bounds);
			});

			// refresh the view
			canvas.SetNeedsDisplayInRect(canvas.Bounds);
			glview.SetNeedsDisplayInRect(glview.Bounds);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			View.AddGestureRecognizer(new NSClickGestureRecognizer(OnSampleClicked));

			SetSample(sample);
		}

		public override void ViewWillAppear()
		{
			base.ViewWillAppear();

			canvas.PaintSurface += OnPaintCanvas;
			glview.PaintSurface += OnPaintGL;

			App.Controller = this;
		}

		public override void ViewWillDisappear()
		{
			base.ViewWillDisappear();

			canvas.PaintSurface -= OnPaintCanvas;
			glview.PaintSurface -= OnPaintGL;

			App.Controller = null;
		}

		private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
		{
			OnPaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
		}

		private void OnPaintGL(object sender, SKPaintGLSurfaceEventArgs e)
		{
			OnPaintSurface(e.Surface.Canvas, e.RenderTarget.Width, e.RenderTarget.Height);
		}

		public void OnSampleClicked()
		{
			sample?.Tap();
		}

		public void OnPaintSurface(SKCanvas canvas, int width, int height)
		{
			sample?.DrawSample(canvas, width, height);
		}

		public void ChangeBackend(SampleBackends backend)
		{
			switch (backend)
			{
				case SampleBackends.Memory:
					glview.Hidden = true;
					canvas.Hidden = false;
					break;
				case SampleBackends.OpenGL:
					glview.Hidden = false;
					canvas.Hidden = true;
					break;
				default:
					var alert = new NSAlert();
					alert.MessageText = "Configure Backend";
					alert.AddButton("OK");
					alert.InformativeText = "This functionality is not yet implemented.";
					alert.RunSheetModal(View.Window);
					break;
			}
		}
	}
}
