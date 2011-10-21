﻿namespace Releaser.Core
{
	/// <summary>
	/// Base class for commands.
	/// </summary>
	public abstract class BaseCommand
	{
		/// <summary>
		/// Gets command name.
		/// </summary>
		public string Name
		{
			get { return GetType().FullName; }
		}
	}
}
