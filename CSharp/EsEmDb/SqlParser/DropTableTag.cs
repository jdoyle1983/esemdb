/*
    THIS FILE WAS FOUND SOMEWHERE ONLINE
    UNTIL I CAN FIND THE SOURCE, I WON'T APPLY LICENSE TERMS TO IT
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Parser
{
	#region OrderByTag

	[TagType(DropTableTag.cTagName)]
	[MatchDropTableTag]
	internal class DropTableTag : SimpleTwoWordTag
	{
		#region Consts

		/// <summary>
		/// The name of the tag (its identifier).
		/// </summary>
		public const string cTagName = "DROP_TABLE";

		/// <summary>
		/// The first part of tag.
		/// </summary>
		public const string cTagFirstPart = "DROP";

		/// <summary>
		/// The second part of tag.
		/// </summary>
		public const string cTagSecondPart = "TABLE";

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

	internal class MatchDropTableTagAttribute : MatchSimpleTwoWordTagAttribute
	{
		#region Properties

		/// <summary>
		/// Gets the name of the tag (its identifier and sql text)
		/// </summary>
		protected override string Name
		{
			get
			{
				return DropTableTag.cTagName;
			}
		}

		/// <summary>
		/// Gets the first word of the tag.
		/// </summary>
		protected override string FirstWord 
		{
			get
			{
				return DropTableTag.cTagFirstPart;
			}
		}

		/// <summary>
		/// Gets the second word of the tag.
		/// </summary>
		protected override string SecondWord
		{
			get
			{
				return DropTableTag.cTagSecondPart;
			}
		}

		#endregion
	}

	#endregion
}
