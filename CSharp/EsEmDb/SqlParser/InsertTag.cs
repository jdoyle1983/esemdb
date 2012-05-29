using System;
using System.Collections.Generic;
using System.Text;

namespace Parser
{
	#region OrderByTag

	[TagType(InsertTag.cTagName)]
	[MatchInsertTag]
	internal class InsertTag : SimpleTwoWordTag
	{
		#region Consts

		/// <summary>
		/// The name of the tag (its identifier).
		/// </summary>
		public const string cTagName = "INSERT";

		/// <summary>
		/// The first part of tag.
		/// </summary>
		public const string cTagFirstPart = "INSERT";

		/// <summary>
		/// The second part of tag.
		/// </summary>
		public const string cTagSecondPart = "INTO";

		#endregion

		#region Properties

		/// <summary>
		/// Gets the name of the tag.
		/// </summary>
		protected override string Name 
		{
			get
			{
				return cTagName;
			}
		}

		/// <summary>
		/// Gets the first word of the tag.
		/// </summary>
		protected override string FirstWord
		{
			get
			{
				return cTagFirstPart;
			}
		}

		/// <summary>
		/// Gets the second word of the tag.
		/// </summary>
		protected override string SecondWord
		{
			get
			{
				return cTagSecondPart;
			}
		}

		#endregion
	}

	#endregion

	#region MatchOrderByTagAttribute

	internal class MatchInsertTagAttribute : MatchSimpleTwoWordTagAttribute
	{
		#region Properties

		/// <summary>
		/// Gets the name of the tag (its identifier and sql text)
		/// </summary>
		protected override string Name
		{
			get
			{
				return InsertTag.cTagName;
			}
		}

		/// <summary>
		/// Gets the first word of the tag.
		/// </summary>
		protected override string FirstWord 
		{
			get
			{
				return InsertTag.cTagFirstPart;
			}
		}

		/// <summary>
		/// Gets the second word of the tag.
		/// </summary>
		protected override string SecondWord
		{
			get
			{
				return InsertTag.cTagSecondPart;
			}
		}

		#endregion
	}

	#endregion
}
