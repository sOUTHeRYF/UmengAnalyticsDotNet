using UnityEngine;
using System.Collections;
using UmengSDK;
public class MainGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UmengAnalytics.Init ("5791c0a367e58e3370000aee", "TestGame.Yodo1", "0.1.0.1");
		UmengAnalytics.StartTrack ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void OnTrackEvent()
	{
		UmengAnalytics.TrackEvent ("TestEvent");
	}
	public void OnGetOnLineParam()
	{
		Debug.LogError ("Param:" + "TestParam" + "_Value:" + UmengAnalytics.GetOnlineParam ("TestParam"));
	}
	public void OnApplicationQuit()
	{
		UmengAnalytics.EndTrack ();	
	}
}
