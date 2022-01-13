using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using UIKit;

namespace Microsoft.Maui.Handlers
{
	public partial class ApplicationHandler : ElementHandler<IApplication, UIApplicationDelegate>
	{
		public static void MapTerminate(ApplicationHandler handler, IApplication application, object? args)
		{
#if __MACCATALYST__
			NSApplication.SharedApplication.Terminate();
#else
			handler.Logger?.LogWarning("iOS does not support programmatically terminating the app.");
#endif
		}

		public static void MapOpenWindow(ApplicationHandler handler, IApplication application, object? args)
		{
			handler.NativeView?.RequestNewWindow(application, args as OpenWindowRequest);
		}

		public static void MapCloseWindow(ApplicationHandler handler, IApplication application, object? args)
		{
			if (args is IWindow window)
			{
				// See if the window's handler has an associated UIWindowScene and UISceneSession
				var sceneSession = (window.Handler?.NativeView as UIWindow)?.WindowScene?.Session;

				if (sceneSession != null)
				{
					// Request that the scene be destroyed
					// TODO: Error handler?
					UIApplication.SharedApplication.RequestSceneSessionDestruction(sceneSession, null, null);
				}
			}
		}

#if __MACCATALYST__
		class NSApplication
		{
			static NativeHandle ClassHandle => ObjCRuntime.Class.GetHandle("NSApplication");
			static NativeHandle SharedApplicationSelector => ObjCRuntime.Selector.GetHandle("sharedApplication");
			static NativeHandle TerminateSelector => ObjCRuntime.Selector.GetHandle("terminate:");

			readonly NativeHandle _handle;

			NSApplication(NativeHandle handle)
			{
				_handle = handle;
			}

			public static NSApplication SharedApplication =>
				new(NativeHandle_objc_msgSend(ClassHandle, SharedApplicationSelector));

			public void Terminate() =>
				void_objc_msgSend_NativeHandle(_handle, TerminateSelector, NativeHandle.Zero);

			[DllImport(ObjCRuntime.Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
			static extern NativeHandle NativeHandle_objc_msgSend(NativeHandle receiver, NativeHandle selector);

			[DllImport(ObjCRuntime.Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
			static extern void void_objc_msgSend_NativeHandle(NativeHandle receiver, NativeHandle selector, NativeHandle arg1);
		}
#endif
	}
}