using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Illisian.UnityUtil.Config
{
	/// <summary>
	/// 
	/// </summary>
	public class Setting
	{
		/// <summary>
		/// Gets the app setting.
		/// </summary>
		/// <param name="settingKey">The setting key.</param>
		/// <returns></returns>
		public static string GetAppSetting(string settingKey)
		{
			if (ConfigurationManager.AppSettings[settingKey] != null)
				return ConfigurationManager.AppSettings[settingKey];
			return "";
		}
		/// <summary>
		/// Gets the connection setting.
		/// </summary>
		/// <param name="settingKey">The setting key.</param>
		/// <returns></returns>
		public static string GetConnectionSetting(string settingKey)
		{
			if (ConfigurationManager.ConnectionStrings[settingKey] != null)
				return ConfigurationManager.ConnectionStrings[settingKey].ConnectionString;
			return "";
		}

		/// <summary>
		/// Gets the section.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sectionName">Name of the section.</param>
		/// <returns></returns>
		public static T GetSection<T>(string sectionName)
		{
			return (T)ConfigurationManager.GetSection(sectionName);
		}
	}
}
