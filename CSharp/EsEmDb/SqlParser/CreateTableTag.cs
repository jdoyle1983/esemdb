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

	[TagType(CreateTableTag.cTagName)]
	[MatchCreateTableTag]
	internal class CreateTableTag : SimpleTwoWordTag
	{
		#region Consts

		/// <summary>
		/// The name of the tag (its identifier).
		/// </summary>
		public const string cTagName = "CREATE_TABLE";

		/// <summary>
		/// The first part of tag.
		/// </summary>
		public const string cTagFirstPart = "CREATE";

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

	internal class MatchCreateTableTagAttribute : MatchSimpleTwoWordTagAttribute
	{
		#region Properties

		/// <summary>
		/// Gets the name of the tag (its identifier and sql text)
		/// </summary>
		protected override string Name
		{
			get
			{
				return CreateTableTag.cTagName;
			}
		}

		/// <summary>
		/// Gets the first word of the tag.
		/// </summary>
		protected override string FirstWord 
		{
			get
			{
				return CreateTableTag.cTagFirstPart;
			}
		}

		/// <summary>
		/// Gets the second word of the tag.
		/// </summary>
		protected override string SecondWord
		{
			get
			{
				return CreateTableTag.cTagSecondPart;
			}
		}

		#endregion
	}

	#endregion
}
