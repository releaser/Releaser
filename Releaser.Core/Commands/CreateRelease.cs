using System;

namespace Releaser.Core.Commands
{
	/// <summary>
	/// Command for creating release.
	/// </summary>
	public class CreateRelease : BaseCommand
	{
		/// <summary>
		/// Gets or sets release version code.
		/// </summary>
		public string VersionCode { get; set; }

		/// <summary>
		/// Gets or sets project ID.
		/// </summary>
		public Guid ProjectId { get; set; }

		/// <summary>
		/// Gets or sets user ID.
		/// </summary>
		public Guid UserId { get; set; }

		/// <summary>
		/// Gets or sets release comment.
		/// </summary>
		public string Comment { get; set; }
	}
}