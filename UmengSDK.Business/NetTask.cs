using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UmengSDK.Common;
using UmengSDK.Model;
using UmengSDK.Third.Gzip;

namespace UmengSDK.Business
{
	internal class NetTask
	{
		public delegate void ResponseCallback(string response);

		public NetTask.ResponseCallback SendResponseCallback;

		private string _message;

		private int _retryTime;

		private string[] _send_url;

		public int Timeout = 5000;

		public bool _isBusy;

		private bool _isCompressed = true;

		public NetTask(MessageType type, string message = null)
		{
			this._retryTime = 0;
			switch (type)
			{
			case MessageType.AppLog:
				this._send_url = Constants.SEND_LOG_URL;
				this._isCompressed = true;
				break;
			case MessageType.CheckUpdate:
				this._message = MessageBuilder.buildUpdateMessage();
				this._send_url = Constants.SEND_UPDATE_URL;
				this._isCompressed = false;
				break;
			case MessageType.OnlineConfig:
				this._message = MessageBuilder.buildConfigMessage();
				this._send_url = Constants.SEND_CONFIG_URL;
				this._isCompressed = false;
				break;
			case MessageType.OnlineParam:
				this._send_url = Constants.SEND_CONFIG_URL;
				this._isCompressed = false;
				break;
			default:
				DebugUtil.Log("Invalid message type", "udebug----------->");
				return;
			}
			if (!string.IsNullOrEmpty(message))
			{
				this._message = message;
			}
		}

		public NetTask(Launch launch, Terminate terminate, Body body)
		{
			this._retryTime = 0;
			this._message = MessageBuilder.buildLogMessage(launch, terminate, body);
            DebugUtil.Log("AAAA"+this._message);
			this._send_url = Constants.SEND_LOG_URL;
			this._isCompressed = true;
		}

		public NetTask(Body body)
		{
			this._retryTime = 0;
			this._message = MessageBuilder.getCurrentMessage(body);
			this._send_url = Constants.SEND_LOG_URL;
			this._isCompressed = true;
		}

		public void sendMessage()
		{
			if (string.IsNullOrEmpty(this._message))
			{
				DebugUtil.Log("message is null", "udebug----------->");
				return;
			}
			this.retrySendMessage();
		}

		private void sendMessage(string url)
		{
			this._retryTime++;
			this._isBusy = true;
			Uri uri = new Uri(url);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType ="application/x-www-form-urlencoded";
			httpWebRequest.Headers["X-Umeng-Sdk"] = "windowsphone/1.0";
	//todo		if (this._isCompressed)
		//	{
		//		httpWebRequest.Headers["Content-Encoding"] = "gzip";
		//	}
			httpWebRequest.BeginGetRequestStream(new AsyncCallback(this.RequestReady), httpWebRequest);
		}

		private void RequestReady(IAsyncResult asyncResult)
		{
			ThreadPool.QueueUserWorkItem(delegate(object state)
			{
				try
				{
					Thread.CurrentThread.Name = "UmengNetTask";
					HttpWebRequest httpWebRequest = asyncResult.AsyncState as HttpWebRequest;
					if (this._isCompressed && false)
					{
						using (Stream stream = httpWebRequest.EndGetRequestStream(asyncResult))
						{
							string s = Uri.EscapeDataString("content=" + this._message);
							byte[] array = GZipStream.CompressString(s);
							stream.Write(array, 0, array.Length);
							goto IL_B7;
						}
					}
					using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.EndGetRequestStream(asyncResult)))
					{
						streamWriter.Write("content=");
						streamWriter.Write(Uri.EscapeDataString(this._message));
					}
					IL_B7:
					HttpWebRequestState httpWebRequestState = new HttpWebRequestState(httpWebRequest);
					httpWebRequest.BeginGetResponse(new AsyncCallback(this.ResponseReady), httpWebRequestState);
					if (!httpWebRequestState.TimeoutEvent.WaitOne(this.Timeout) && httpWebRequest != null)
					{
						httpWebRequest.Abort();
					}
				}
				catch (Exception e)
				{
					DebugUtil.Log(e);
				}
			});
		}

		private void ResponseReady(IAsyncResult asyncResult)
		{
			string response = null;
			bool flag = false;
			HttpWebRequestState httpWebRequestState = asyncResult.AsyncState as HttpWebRequestState;
			httpWebRequestState.TimeoutEvent.Set();
			try
			{
				HttpWebRequest request = httpWebRequestState.Request;
				HttpWebResponse httpWebResponse = request.EndGetResponse(asyncResult) as HttpWebResponse;
				if (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.OK)
				{
					Stream responseStream = httpWebResponse.GetResponseStream();
					response = new StreamReader(responseStream, Encoding.UTF8).ReadToEnd();
					httpWebResponse.Close();
					responseStream.Close();
					responseStream.Dispose();
					flag = true;
					DebugUtil.Log("\n-Send message successed", "udebug----------->");
					DebugUtil.Log("**************\n" + this._message + "\n**************", "");
				}
				else
				{
					DebugUtil.Log("get no response", "udebug----------->");
					flag = this.retrySendMessage();
				}
			}
			catch (Exception e)
			{
				DebugUtil.Log(e);
				flag = this.retrySendMessage();
			}
			if (flag)
			{
				if (this.SendResponseCallback != null)
				{
					this.SendResponseCallback(response);
				}
				this._message = null;
				this._isBusy = false;
			}
		}

		private bool retrySendMessage()
		{
			if (this._send_url == null || this._retryTime >= this._send_url.Length)
			{
				return true;
			}
			this.sendMessage(this._send_url[this._retryTime]);
			return false;
		}

		private string EncoderString(string inputStr)
		{
			return Uri.EscapeDataString(inputStr);
		}
	}
}
