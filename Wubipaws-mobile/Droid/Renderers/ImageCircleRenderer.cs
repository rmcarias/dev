﻿using System;
using Wubipaws.Mobile;
using Wubipaws.Mobile.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;


[assembly: ExportRenderer (typeof(CircleImage), typeof(ImageCircleRenderer))]
namespace Wubipaws.Mobile.Droid
{
	/// <summary>
	/// ImageCircle Implementation
	/// </summary>
	public class ImageCircleRenderer : ImageRenderer
	{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnElementChanged (ElementChangedEventArgs<Image> e)
		{
			base.OnElementChanged (e);

			if (e.OldElement == null)
			{
				//Only enable hardware accelleration on lollipop
				if ((int)Android.OS.Build.VERSION.SdkInt < 21)
				{
					SetLayerType (Android.Views.LayerType.Software, null);
				}

			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="canvas"></param>
		/// <param name="child"></param>
		/// <param name="drawingTime"></param>
		/// <returns></returns>
		protected override bool DrawChild (Canvas canvas, global::Android.Views.View child, long drawingTime)
		{
			try
			{
				var radius = Math.Min (Width, Height) / 2;
				var strokeWidth = 10;
				radius -= strokeWidth / 2;


				Path path = new Path ();
				path.AddCircle (Width / 2, Height / 2, radius, Path.Direction.Ccw);
				canvas.Save ();
				canvas.ClipPath (path);

				var result = base.DrawChild (canvas, child, drawingTime);

				canvas.Restore ();

				path = new Path ();
				path.AddCircle (Width / 2, Height / 2, radius, Path.Direction.Ccw);

				var paint = new Paint ();
				paint.AntiAlias = true;
				paint.StrokeWidth = ((CircleImage)Element).BorderThickness;
				paint.SetStyle (Paint.Style.Stroke);
				paint.Color = ((CircleImage)Element).BorderColor.ToAndroid ();

				canvas.DrawPath (path, paint);

				paint.Dispose ();
				path.Dispose ();
				return result;
			} catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("Unable to create circle image: " + ex);
			}

			return base.DrawChild (canvas, child, drawingTime);
		}
	}
}

