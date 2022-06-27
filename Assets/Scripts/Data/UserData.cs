using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class UserData
{
	[JsonIgnore] public Action OnValueChanged;

	private int _currentLevel;

	public int CurrentLevel
	{
		get { return _currentLevel; }
		set
		{
			_currentLevel = value;
			if (OnValueChanged != null) OnValueChanged.Invoke();
		}
	}


	private int _gem;
	public int Gem
	{
		get { return _gem; }
		set
		{
			_gem = value;
			if (OnValueChanged != null) OnValueChanged.Invoke();
		}
	}
	
	private int _key;
	public int Key
	{
		get { return _key; }
		set
		{
			_key = value;
			if (OnValueChanged != null) OnValueChanged.Invoke();
		}
	}

	private bool _isRemoveAds = false;
	public bool IsRemoveAds
	{
		get { return _isRemoveAds; }
		set
		{
			_isRemoveAds = value;
			if (OnValueChanged != null) OnValueChanged.Invoke();
		}
	}

	private bool _isFirstOpen = true;
	public bool IsFirstOpen
	{
		get { return _isFirstOpen; }
		set
		{
			_isFirstOpen = value;
			if (OnValueChanged != null) OnValueChanged.Invoke();
		}
	}
	
	private bool _isShowIntro = false;
	public bool IsShowIntro
	{
		get { return _isShowIntro; }
		set
		{
			_isShowIntro = value;
			if (OnValueChanged != null) OnValueChanged.Invoke();
		}
	}

	private bool _isAcceptGDPR = false;
	public bool IsAcceptGDPR
	{
		get { return _isAcceptGDPR; }
		set
		{
			_isAcceptGDPR = value;
			if (OnValueChanged != null) OnValueChanged.Invoke();
		}
	}
	
	private bool _isSubscribe = false;
	public bool IsSubscribe
	{
		get { return _isSubscribe; }
		set
		{
			_isSubscribe = value;
			if (OnValueChanged != null) OnValueChanged.Invoke();
		}
	}
}