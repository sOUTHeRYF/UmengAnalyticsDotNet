using System;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace UmengSDK.Common
{
	internal static class IsoPersistentHelper
	{
		public static void Save<T>(this T obj, string file)
		{
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null); 
			IsolatedStorageFileStream isolatedStorageFileStream = null;
			try
			{
				isolatedStorageFileStream = new IsolatedStorageFileStream(file,System.IO.FileMode.OpenOrCreate);
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
        public static bool FileExists(string name)
        {
            bool result = false;
            IsolatedStorageFileStream isolatedStorageFileStream = null;
            try
            {
                isolatedStorageFileStream = new IsolatedStorageFileStream(name, System.IO.FileMode.Open);
                result = true;
            }
            catch (Exception e)
            {
                result = false;
            }
            finally
            {
                if (null != isolatedStorageFileStream)
                {
                    isolatedStorageFileStream.Close();
                    isolatedStorageFileStream.Dispose();
                }
            }
            return result;
        }
		public static T Load<T>(string file)
		{
			T result = default(T);
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            if (FileExists(file))
			{
				IsolatedStorageFileStream isolatedStorageFileStream = null;
				try
				{
					isolatedStorageFileStream = new IsolatedStorageFileStream(file, System.IO.FileMode.Open);
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
			IsolatedStorageFile userStoreForApplication = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            try
			{
				if (FileExists(file))
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
