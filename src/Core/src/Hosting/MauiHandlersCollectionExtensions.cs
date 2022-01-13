using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Maui.Hosting
{
	public static partial class MauiHandlersCollectionExtensions
	{
		public static IMauiHandlersCollection AddHandler(this IMauiHandlersCollection handlersCollection, Type viewType, Type handlerType)
		{
			handlersCollection.AddTransient(viewType, handlerType);
			return handlersCollection;
		}

		public static IMauiHandlersCollection AddHandler<TType, TTypeRender>(this IMauiHandlersCollection handlersCollection)
			where TType : IElement
			where TTypeRender : IElementHandler
		{
			handlersCollection.AddTransient(typeof(TType), typeof(TTypeRender));
			return handlersCollection;
		}

		public static IMauiHandlersCollection TryAddHandler(this IMauiHandlersCollection handlersCollection, Type viewType, Type handlerType)
		{
			handlersCollection.TryAddTransient(viewType, handlerType);
			return handlersCollection;
		}

		public static IMauiHandlersCollection TryAddHandler<TType, TTypeRender>(this IMauiHandlersCollection handlersCollection)
			where TType : IView
			where TTypeRender : IViewHandler
		{
			handlersCollection.TryAddTransient(typeof(TType), typeof(TTypeRender));
			return handlersCollection;
		}
	}
}