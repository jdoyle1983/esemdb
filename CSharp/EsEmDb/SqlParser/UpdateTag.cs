using System;
using System.Collections.Generic;
using System.Text;

namespace Parser
{
	#region UpdateTag

	[TagType(UpdateTag.cTagName)]
	[MatchUpdateTag]
	internal class UpdateTag : SimpleOneWordTag
	{
		#region Consts

		/// <summary>
		/// The name of the tag (its identifier).
		/// </summary>
		public const string cTagName = "UPDATE";

		#endregion

		#region Properties

		/// <summary>
		/// Gets the name of the tag (its identifier and sql text)
		/// </summary>
		protected override string Name
		{
			get 
			{
				return cTagName;
			}
		}

		#endregion
	}

	#endregion

	#region MatchUpdateTagAttribute

	internal class MatchUpdateTagAttribute : MatchSimpleOneWordTagAttribute
	{
		#region Properties

		/// <summary>
		/// Gets the name of the tag (its identifier and sql text)
		/// </summary>
		protected override string Name
		{
			get 
			{
				return UpdateTag.cTagName;
			}
		}

		#endregion
	}

	#endregion
}
