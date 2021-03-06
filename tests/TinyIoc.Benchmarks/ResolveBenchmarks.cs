﻿using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace TinyIoc.Benchmarks
{

#if NET48 //These work on either platform but no point running them twice
	[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net48)]
	[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.Net461)]
#endif
#if NETCOREAPP3_1_OR_GREATER // These don't seem to work in a FX app
	[SimpleJob(BenchmarkDotNet.Jobs.RuntimeMoniker.NetCoreApp31)]
	[RyuJitX86Job]
#endif
#if NET48 //These don't seem to work in a .net core app
	[RyuJitX64Job]
	[MonoJob]
#endif
	[MemoryDiagnoser]
	[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByCategory)]
	//[CategoriesColumn]
	public class ResolveBenchmarks
	{
		private TinyIoC.TinyIoCContainer _NewContainer;
		private TinyIoC.Original.TinyIoCContainer _OriginalContainer;

		[BenchmarkDotNet.Attributes.GlobalSetup]
		public void InitContainer()
		{
			_NewContainer = new TinyIoC.TinyIoCContainer();
			_NewContainer.Register<ISingleton, Singleton>().AsSingleton();
			_NewContainer.Register<IParameterlessInstance, ParameterlessInstance>().AsMultiInstance();
			_NewContainer.Register<ISingleParameterInstance, SingleParameterInstance>().AsMultiInstance();
			_NewContainer.Register(typeof(IOpenGeneric<>), typeof(OpenGenericInstance<>)).AsMultiInstance();

			_OriginalContainer = new TinyIoC.Original.TinyIoCContainer();
			_OriginalContainer.Register<ISingleton, Singleton>().AsSingleton();
			_OriginalContainer.Register<IParameterlessInstance, ParameterlessInstance>().AsMultiInstance();
			_OriginalContainer.Register<ISingleParameterInstance, SingleParameterInstance>().AsMultiInstance();
			_OriginalContainer.Register(typeof(IOpenGeneric<>), typeof(OpenGenericInstance<>)).AsMultiInstance();
		}

		[BenchmarkCategory("ResolveSingleton"), Benchmark(Baseline = true)]
		public ISingleton Original_Resolve_Singleton()
		{
			return _OriginalContainer.Resolve<ISingleton>();
		}

		[BenchmarkCategory("ResolveSingleton"), Benchmark]
		public ISingleton New_Resolve_Singleton()
		{
			return _NewContainer.Resolve<ISingleton>();
		}


		[BenchmarkCategory("ResolveInstance"), Benchmark(Baseline = true)]
		public IParameterlessInstance Original_Resolve_Instance_Without_Dependencies()
		{
			return _OriginalContainer.Resolve<IParameterlessInstance>();
		}

		[BenchmarkCategory("ResolveInstance"), Benchmark]
		public IParameterlessInstance New_Resolve_Instance_Without_Dependencies()
		{
			return _NewContainer.Resolve<IParameterlessInstance>();
		}


		[BenchmarkCategory("ResolveInstanceWithSingletonDependency"), Benchmark(Baseline = true)]
		public ISingleParameterInstance Original_Resolve_Instance_With_Singleton_Dependency()
		{
			return _OriginalContainer.Resolve<ISingleParameterInstance>();
		}

		[BenchmarkCategory("ResolveInstanceWithSingletonDependency"), Benchmark]
		public ISingleParameterInstance New_Resolve_Instance_With_Singleton_Dependency()
		{
			return _NewContainer.Resolve<ISingleParameterInstance>();
		}


		[BenchmarkCategory("ResolveOpenGeneric"), Benchmark(Baseline = true)]
		public IOpenGeneric<string> Original_Resolve_OpenGeneric()
		{
			return _OriginalContainer.Resolve<IOpenGeneric<string>>();
		}

		[BenchmarkCategory("ResolveOpenGeneric"), Benchmark]
		public IOpenGeneric<string> New_Resolve_OpenGeneric()
		{
			return _NewContainer.Resolve<IOpenGeneric<string>>();
		}


		[BenchmarkCategory("ResolveUnregistered"), Benchmark(Baseline = true)]
		public UnregisteredInstance Original_Resolve_Unregistered()
		{
			return _OriginalContainer.Resolve<UnregisteredInstance>();
		}

		[BenchmarkCategory("ResolveUnregistered"), Benchmark]
		public UnregisteredInstance New_Resolve_Unregistered()
		{
			return _NewContainer.Resolve<UnregisteredInstance>();
		}


		[BenchmarkCategory("AutomaticFuncFactory"), Benchmark(Baseline = true)]
		public Func<ParameterlessInstance> Original_Resolve_AutomaticFuncFactory()
		{
			return _OriginalContainer.Resolve<Func<ParameterlessInstance>>();
		}

		[BenchmarkCategory("AutomaticFuncFactory"), Benchmark]
		public Func<ParameterlessInstance> New_Resolve_AutomaticFuncFactory()
		{
			return _NewContainer.Resolve<Func<ParameterlessInstance>>();
		}
	}

	public interface ISingleton
	{

	}

	public class Singleton : ISingleton
	{
		public Singleton()
		{

		}
	}

	public interface IParameterlessInstance { }

	public class ParameterlessInstance : IParameterlessInstance { }

	public class UnregisteredInstance { public UnregisteredInstance(ISingleton singleton) {} }

	public interface ISingleParameterInstance { }

	public class SingleParameterInstance : ISingleParameterInstance
	{
		private readonly ISingleton _Singleton;
		public SingleParameterInstance(ISingleton singleton)
		{
			_Singleton = singleton;
		}
	}

	public interface IOpenGeneric<T> { T Value { get; set; } }
	public class OpenGenericInstance<T> : IOpenGeneric<T>
	{
		public T Value { get; set; }
	}
}
