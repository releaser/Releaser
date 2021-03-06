﻿using System;
using System.Collections.Generic;
using ServiceStack.Text;

namespace Releaser.Engine
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Context.Initialize();

			if (Environment.UserInteractive)
			{
				var listeningOn = args.Length == 0 ? "http://localhost:5557/" : args[0];

				var appHost = new ReleaserEngine();
				appHost.Init();
				appHost.Start(listeningOn);

				Console.WriteLine(
					"ReleaserServer started at {0}, listening on {1}.",
					DateTime.Now,
					listeningOn);

				Console.WriteLine("Please enter any key to exit...");
				Console.ReadKey();
			}
			else
			{
				// TODO: run as service
			}
		}
	}
}
