using System;
using System.ComponentModel;
using System.Reflection;

public static class EnumDescription
{
	/// <summary>
	/// Retrieve the description on the enum, e.g.
	/// [Description("Bright Pink")]
	/// BrightPink = 2,
	/// Then when you pass in the enum, it will retrieve the description
	/// </summary>
	/// <param name="en">The Enumeration</param>
	/// <returns>A string representing the friendly name</returns>
	public static string GetDescription(Enum en)
	{
		Type type = en.GetType();

		MemberInfo[] memInfo = type.GetMember(en.ToString());

		if (memInfo != null && memInfo.Length > 0)
		{
			object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

			if (attrs != null && attrs.Length > 0)
			{
				return ((DescriptionAttribute)attrs[0]).Description;
			}
		}

		return en.ToString();
	}

	/// <summary>
	/// Retrieve the long description on the enum, e.g.
	/// [LongDescription("Bright Pink")]
	/// BrightPink = 2,
	/// Then when you pass in the enum, it will retrieve the description
	/// </summary>
	/// <param name="en">The Enumeration</param>
	/// <returns>A string representing the long description</returns>
	public static string GetLongDescription( Enum en )
	{
		Type type = en.GetType();

		MemberInfo[] memInfo = type.GetMember( en.ToString() );

		if( memInfo != null && memInfo.Length > 0 )
		{
			object[] attrs = memInfo[ 0 ].GetCustomAttributes( typeof( LongDescriptionAttribute ), false );

			if( attrs != null && attrs.Length > 0 )
			{
				return ( (LongDescriptionAttribute)attrs[ 0 ] ).LongDescription;
			}
		}

		return en.ToString();
	}

	/// <summary>
	/// Retrieve the add-on icon on the enum, e.g.
	/// [AddOnIcon("Bright Pink")]
	/// BrightPink = 2,
	/// Then when you pass in the enum, it will retrieve the icon
	/// </summary>
	/// <param name="en">The Enumeration</param>
	/// <returns>A string representing the icon name</returns>
	public static string GetAddOnIcon( Enum en )
	{
		Type type = en.GetType();

		MemberInfo[] memInfo = type.GetMember( en.ToString() );

		if( memInfo != null && memInfo.Length > 0 )
		{
			object[] attrs = memInfo[ 0 ].GetCustomAttributes( typeof( AddOnIconAttribute ), false );

			if( attrs != null && attrs.Length > 0 )
			{
				return ( (AddOnIconAttribute)attrs[ 0 ] ).Icon;
			}
		}

		return en.ToString();
	}

	/// <summary>
	/// Retrieve the explanation
	/// [Explanation("Bright Pink")]
	/// BrightPink = 2,
	/// Then when you pass in the enum, it will retrieve the explanation
	/// </summary>
	/// <param name="en">The Enumeration</param>
	/// <returns>A string representing the explanation</returns>
	public static string GetExplanation( Enum en )
	{
		Type type = en.GetType();

		MemberInfo[] memInfo = type.GetMember( en.ToString() );

		if( memInfo != null && memInfo.Length > 0 )
		{
			object[] attrs = memInfo[ 0 ].GetCustomAttributes( typeof( ExplanationAttribute ), false );

			if( attrs != null && attrs.Length > 0 )
			{
				return ( (ExplanationAttribute)attrs[ 0 ] ).Explanation;
			}
		}

		return en.ToString();
	}

	/// <summary>
	/// Retrieve the example
	/// [Example("Bright Pink")]
	/// BrightPink = 2,
	/// Then when you pass in the enum, it will retrieve the example
	/// </summary>
	/// <param name="en">The Enumeration</param>
	/// <returns>A string representing the example</returns>
	public static string GetExample( Enum en )
	{
		Type type = en.GetType();

		MemberInfo[] memInfo = type.GetMember( en.ToString() );

		if( memInfo != null && memInfo.Length > 0 )
		{
			object[] attrs = memInfo[ 0 ].GetCustomAttributes( typeof( ExampleAttribute ), false );

			if( attrs != null && attrs.Length > 0 )
			{
				return ( (ExampleAttribute)attrs[ 0 ] ).Example;
			}
		}

		return en.ToString();
	}
}