using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class SettingData
{
	[JsonIgnore] public Action OnValueChanged;

	private bool _hasRate;
	public bool HasRate
	{
		get { return _hasRate; }
		set
		{
			_hasRate = value;
			if (OnValueChanged != null) OnValueChanged.Invoke();
		}
	}
	private bool _sound = true;
	public bool Sound
	{
		get { return _sound; }
		set
		{
			_sound = value;
			if (OnValueChanged != null) OnValueChanged.Invoke();
		}
	}

	private bool _music = true;
	public bool Music
	{
		get { return _music; }
		set
		{
			_music = value;
			if (OnValueChanged != null) OnValueChanged.Invoke();
		}
	}

	private bool _notification = true;
	public bool Notification
	{
		get { return _notification; }
		set
		{
			_notification = value;
			if (OnValueChanged != null) OnValueChanged.Invoke();
		}
	}
	
	private bool _vibration = false;
	public bool Vibration
	{
		get { return _vibration; }
		set
		{
			_vibration = value;
			if (OnValueChanged != null) OnValueChanged.Invoke();
		}
	}
}