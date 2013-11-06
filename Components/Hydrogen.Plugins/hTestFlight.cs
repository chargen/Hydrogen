#region Copyright Notice & License Information
// 
// hTestFlight.cs
//  
// Author:
//   Matthew Davey <matthew.davey@dotbunny.com>
//
// Copyright (C) 2013 dotBunny Inc. (http://www.dotbunny.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
#endregion

using UnityEngine;

/// <summary>
/// Hydrogen.Plugins.TestFlight Instance
/// </summary>
public class hTestFlight : MonoBehaviour
{
	public bool presistant = true;
	public string tokenAndroid = "";
	public string tokenIOS = "";
	private bool _paused = false;
	
	/// <summary>
	/// Internal fail safe to maintain instance across threads
	/// </summary>
	/// <remarks>
	/// Multithreaded Safe Singleton Pattern
	/// </remarks>
	/// <description>
	/// http://msdn.microsoft.com/en-us/library/ms998558.aspx
	/// </description>
	private static readonly System.Object _syncRoot = new System.Object();
    
	/// <summary>
	/// Internal reference to the static instance of the TestFlight interface.
	/// </summary>
	private static volatile hTestFlight _staticInstance;
	
	/// <summary>
	/// Gets the TestFlight interface instance.
	/// </summary>
	/// <value>
	/// The TestFlight interface
	/// </value>
	public static hTestFlight Instance 
	{
        get {
            if (_staticInstance == null) {				
                lock (_syncRoot) {
                    _staticInstance = FindObjectOfType (typeof(hTestFlight)) as hTestFlight;
                    
					// If we don't have it, lets make it!
					if (_staticInstance == null) {
						GameObject newTestFlight = new GameObject("TestFlight");
						newTestFlight.AddComponent<hTestFlight>();
						_staticInstance = newTestFlight.GetComponent<hTestFlight>();	
                    }
                }
            }
            return _staticInstance;
        }
    }
	
	public void Awake()
	{
		// Should this gameObject be kept around :) I think so.
		if ( presistant ) DontDestroyOnLoad( this.gameObject );
		
				
		
		// Depending on the platform there are some things that need to be handled before we can 
		// take off and start submitting data
		Hydrogen.Plugins.TestFlight.Initialize();
		
#if (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
		Hydrogen.Plugins.TestFlight.TakeOff(tokenIOS);
#elif UNITY_ANDROID && !UNITY_EDITOR
		Hydrogen.Plugins.TestFlight.TakeOff(tokenAndroid);
#endif
		Hydrogen.Plugins.TestFlight.StartSession();
	}
	
	public void SubmitFeedback(string message)
	{
		if ( !Hydrogen.Plugins.TestFlight.Flying || !Hydrogen.Plugins.TestFlight.Session)
		{
			Debug.Log ("Unable to submit feedback, TestFlight is not in flight.");
			return;
		}
		
		Hydrogen.Plugins.TestFlight.SubmitFeedback(message);
	}
	
	public void PassCheckpoint(string checkpointName)
	{
		if ( !Hydrogen.Plugins.TestFlight.Flying || !Hydrogen.Plugins.TestFlight.Session )
		{
			Debug.Log ("Unable to send checkpoint data, TestFlight is not in flight.");
			return;
		}
		
		Hydrogen.Plugins.TestFlight.PassCheckpoint(checkpointName);
	}
	
	public void AddCustomEnvironmentInformation(string data, string key)
	{
		if ( !Hydrogen.Plugins.TestFlight.Flying || !Hydrogen.Plugins.TestFlight.Session) 
		{
			Debug.Log ("Unable to send information, TestFlight is not in flight.");
			return;
		}
		
		Hydrogen.Plugins.TestFlight.AddCustomEnvironmentInformation(data,key);
	}
	
	public void Log(string message)
	{
		if ( !Hydrogen.Plugins.TestFlight.Flying || !Hydrogen.Plugins.TestFlight.Session ) 
		{
			Debug.Log ("Unable to send log data, TestFlight is not in flight.");
			return;
		}
		
		Hydrogen.Plugins.TestFlight.Log(message);
	}
	
	public void LogAsync(string message)
	{
		if ( !Hydrogen.Plugins.TestFlight.Flying || !Hydrogen.Plugins.TestFlight.Session ) 
		{
			Debug.Log ("Unable to send log data, TestFlight is not in flight.");
			return;
		}
		Hydrogen.Plugins.TestFlight.LogAsync(message);
	}
	
	/// <summary>
	/// Raised when your application is paused, as a more accurate depiction of Session times.
	/// </summary>
	/// <remarks>
	/// This was meant originally as a fix for Android, but we've carried it over to iOS and 
	/// switched everything to manual for better integration with Unity
	/// </remarks>
	public void OnApplicationPause()
	{
		if ( _paused ) 
		{
			_paused = false;
			Hydrogen.Plugins.TestFlight.StartSession();
		}
		else
		{
			_paused = true;
			Hydrogen.Plugins.TestFlight.EndSession();
		}	
	}
	
	public void OnApplicationQuit()
	{
		Hydrogen.Plugins.TestFlight.EndSession();
	}
}