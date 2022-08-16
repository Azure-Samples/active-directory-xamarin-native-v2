using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace MauiB2C;

class Program : MauiApplication
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	static void Main(string[] args)
	{
		throw new PlatformNotSupportedException("Tizen platform is not supported!");
	}
}
