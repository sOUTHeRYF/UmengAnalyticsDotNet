using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using UmengSDK.Common;
using UmengSDK.Model;

namespace UmengSDK.Business
{
	internal class BodyPersistentManager
	{
		private static BodyPersistentManager _current = null;

		private static readonly object lockObj = new object();

		private IsolatedStorageFile _isoFile = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);

		private static readonly object thislocker = new object();

		public static BodyPersistentManager Current
		{
			get
			{
				if (BodyPersistentManager._current == null)
				{
					lock (BodyPersistentManager.lockObj)
					{
						if (BodyPersistentManager._current == null)
						{
							BodyPersistentManager._current = new BodyPersistentManager();
						}
					}
				}
				return BodyPersistentManager._current;
			}
		}
        public static bool FileExists(string name)
        {
            bool result = false;
            IsolatedStorageFileStream isolatedStorageFileStream = null;
            try
            {
                isolatedStorageFileStream = new IsolatedStorageFileStream(name, System.IO.FileMode.Open);
                result =  isolatedStorageFileStream.Length > 5;
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
        public bool HasCache
		{
            get
            {
                return FileExists(this.FileName);
            }
		}

		public Body LocalBody
		{
			get
			{
				Body result;
				lock (this)
				{
					result = this.ReadBodyFromFile();
				}
				return result;
			}
		}

		public string FileName
		{
			get;
			set;
		}

		private BodyPersistentManager()
		{
			this.FileName = "unknown";
		}

		public bool Save(Body body)
		{
			bool result;
			try
			{
				lock (this)
				{
					if (body != null)
					{
						this.Load(body);
						bool flag2 = this.WriteBodyToFile(body);
						DebugUtil.Log("save local body successed: " + flag2.ToString(), "udebug----------->");
						result = flag2;
					}
					else
					{
						result = false;
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("error on save body message in BodyPersistent", e);
				result = false;
			}
			return result;
		}

		public bool Load(Body body)
		{
			try
			{
				lock (this)
				{
					if (body != null)
					{
						Body body2 = this.ReadBodyFromFile();
						if (body2 != null)
						{
							body.Merge(body2);
							DebugUtil.Log("Load local body successed!", "udebug----------->");
							return true;
						}
					}
					DebugUtil.Log("no body cached in local!", "udebug----------->");
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("error in load body cache message in BodyPersistent", e);
			}
			return false;
		}

		public void Delete()
		{
			try
			{
				lock (this)
				{
					if (this.HasCache)
					{
                        IsolatedStorageFileStream isolatedStorageFileStream = null;
                        try
                        {
                            isolatedStorageFileStream = new IsolatedStorageFileStream(this.FileName, System.IO.FileMode.Open);
                            using (StreamWriter streamWriter = new StreamWriter(isolatedStorageFileStream))
                            {
                                streamWriter.Write("y");
                            }
                        }
                        catch (Exception e)
                        {

                        }
                        finally
                        {
                            if (null != isolatedStorageFileStream)
                            {
                                isolatedStorageFileStream.Close();
                                isolatedStorageFileStream.Dispose();
                            }
                        }
						DebugUtil.Log("delete cached message successed!", "udebug----------->");
					}
					else
					{
						DebugUtil.Log("has no cached message", "udebug----------->");
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("delete cached message failed!", e);
			}
		}

		private Body ReadBodyFromFile()
		{
			Body result = null;
			try
			{
				if (this.HasCache)
				{
                //    	using (IsolatedStorageFileStream isolatedStorageFileStream = this._isoFile.OpenFile(this.FileName, 3, 1))
                   using (IsolatedStorageFileStream isolatedStorageFileStream = new IsolatedStorageFileStream(this.FileName, FileMode.Open, FileAccess.Read))
                    {
						if (isolatedStorageFileStream.Length > 0L)
						{
							using (StreamReader streamReader = new StreamReader(isolatedStorageFileStream))
							{
								string text = streamReader.ReadToEnd();
								if (!string.IsNullOrEmpty(text))
								{
									Dictionary<string, object> dic = JSON.JsonDecode(text) as Dictionary<string, object>;
									result = new Body(dic);
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("ReadBodyFromFile failed!", e);
			}
			return result;
		}

		private bool WriteBodyToFile(Body body)
		{
			try
			{
				string text;
				if (body != null && (text = JSON.JsonEncode(body.ToDictionary())) != null)
				{
					using (IsolatedStorageFileStream isolatedStorageFileStream = new IsolatedStorageFileStream(this.FileName, FileMode.OpenOrCreate, FileAccess.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(isolatedStorageFileStream))
						{
							streamWriter.Write(text);
							return true;
						}
					}
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log("WriteBodyToFile failed!", e);
			}
			return false;
		}
	}
}
