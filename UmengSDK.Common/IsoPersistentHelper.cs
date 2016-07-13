using System;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace UmengSDK.Common
{
	internal static class IsoPersistentHelper
	{
		public static void Save<T>(this T obj, string file)
		{
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
			IsolatedStorageFileStream isolatedStorageFileStream = null;
			try
			{
				isolatedStorageFileStream = userStoreForApplication.CreateFile(file);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				xmlSerializer.Serialize(isolatedStorageFileStream, obj);
			}
			catch (Exception)
			{
			}
			finally
			{
				if (isolatedStorageFileStream != null)
				{
					isolatedStorageFileStream.Close();
					isolatedStorageFileStream.Dispose();
				}
			}
		}

		public static T Load<T>(string file)
		{
			T result = default(T);
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
			if (userStoreForApplication.FileExists(file))
			{
				IsolatedStorageFileStream isolatedStorageFileStream = null;
				try
				{
					isolatedStorageFileStream = userStoreForApplication.OpenFile(file, 3);
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
					result = (T)((object)xmlSerializer.Deserialize(isolatedStorageFileStream));
				}
				catch (Exception)
				{
				}
				finally
				{
					if (isolatedStorageFileStream != null)
					{
						isolatedStorageFileStream.Close();
						isolatedStorageFileStream.Dispose();
					}
				}
			}
			return result;
		}

		public static void Delete(string file)
		{
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
			try
			{
				if (userStoreForApplication.FileExists(file))
				{
					userStoreForApplication.DeleteFile(file);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
