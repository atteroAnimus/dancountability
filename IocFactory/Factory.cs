using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace IocFactory
{
	public class Factory 
	{

		private static volatile Factory _instance;
		private static readonly object _syncRoot = new object();
		private readonly IServiceProvider _provider;
		private Factory()
		{

			var collection = new List<ServiceDescriptor>();
			var container = new Container();
			container.Configure(config =>
			{
				config.Scan(x =>
				{
					x.AssembliesFromApplicationBaseDirectory();
					x.WithDefaultConventions();
				});
				config.Populate(collection);
				
			});
			
			_provider = container.GetInstance<IServiceProvider>();
		}
		

		public static Factory Instance
		{
			get
			{
				if (_instance != null) return _instance;
				lock (_syncRoot)
				{
					if (_instance != null) return _instance;
					_instance = new Factory();
				}

				return _instance;
			}
		}

		public T Resolve<T>()
		{
			try
			{
				return _provider.GetService<T>();
			}
			catch (NullReferenceException nulm)
			{
				throw new NullReferenceException($"unable to resolve type {typeof(T)}");
			}
		}
	}
}