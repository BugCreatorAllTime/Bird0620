using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class DataManager
{
	public SaveGameData SaveGameData;
	public UserData UserData;
	public SettingData SettingData;

	public List<Action> OnUserChangeCallback = new List<Action>();
	public List<Action> OnSettingChangeCallback = new List<Action>();

	private static DataManager _instance;

	public static DataManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new DataManager();
			}

			return _instance;
		}
	}

	private DataManager()
	{
		InitUserData();
        InitSettingData();
		InitSaveGameData();
	}

	public void InitSaveGameData()
	{
		if (PlayerPrefs.HasKey(CONST.PLAYER_PREF_SAVE_GAME_DATA))
		{
			string saveGameStr = PlayerPrefs.GetString(CONST.PLAYER_PREF_SAVE_GAME_DATA);
			SaveGameData = JsonConvert.DeserializeObject<SaveGameData>(saveGameStr);
		}
		else
		{
			SaveGameData = new SaveGameData();
		}

		SaveGame();
	}

	public void SaveGame(SaveGameData savegameData = null)
	{
		if (savegameData != null) SaveGameData = savegameData;
		if (SaveGameData != null)
		{
			PlayerPrefs.SetString(CONST.PLAYER_PREF_SAVE_GAME_DATA, JsonConvert.SerializeObject(SaveGameData));
		}
	}

	private void InitSettingData()
	{
		if (PlayerPrefs.HasKey(CONST.PLAYER_PREF_SETTING_DATA))
		{
			string dataStr = PlayerPrefs.GetString(CONST.PLAYER_PREF_SETTING_DATA);
			Debug.Log("Setting Data: " + dataStr);
			SettingData = JsonConvert.DeserializeObject<SettingData>(dataStr);
		}
		else
		{
			SettingData = new SettingData();
		}

		SettingData.OnValueChanged = SaveSetting;
		SaveSetting();
	}

	public void SaveSetting()
	{
		Debug.Log("Save Setting");
		if (SettingData != null)
		{
			PlayerPrefs.SetString(CONST.PLAYER_PREF_SETTING_DATA, JsonConvert.SerializeObject(SettingData));
			for (int i = 0; i < OnSettingChangeCallback.Count; i++) OnSettingChangeCallback[i].Invoke();
		}
	}

	private void InitUserData()
	{
		if (PlayerPrefs.HasKey(CONST.PLAYER_PREF_USER_DATA))
		{
			string dataStr = PlayerPrefs.GetString(CONST.PLAYER_PREF_USER_DATA);
			Debug.Log("User Data: " + dataStr);
			UserData = JsonConvert.DeserializeObject<UserData>(dataStr);
		}
		else
		{
			UserData = new UserData();
		}

		UserData.OnValueChanged = SaveUserData;
		SaveUserData();
	}

	public void SaveUserData()
	{
		if (UserData != null)
		{
			PlayerPrefs.SetString(CONST.PLAYER_PREF_USER_DATA, JsonConvert.SerializeObject(UserData));
			for (int i = 0; i < OnUserChangeCallback.Count; i++) OnUserChangeCallback[i].Invoke();
		}
	}
}
