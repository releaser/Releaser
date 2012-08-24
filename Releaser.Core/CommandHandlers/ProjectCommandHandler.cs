﻿using Releaser.Core.Commands;
using Releaser.Core.Handlers;

namespace Releaser.Core.CommandHandlers
{
	/// <summary>
	/// Handles all commands with projects.
	/// </summary>
	public class CreateProjectCommandHandler : BaseCommandHandler<CreateProjectCommand>
	{
		protected override void ExecuteInternal(CreateProjectCommand command)
		{
		}
	}
}