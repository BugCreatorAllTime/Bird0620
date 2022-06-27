using System;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class EditorSavegame {
	[MenuItem("Assets/Savegame/ClearSavegame")]
	public static void ClearPlayerPrefSavegame() {
		PlayerPrefs.DeleteKey(CONST.PLAYER_PREF_SAVE_GAME_DATA);
		PlayerPrefs.DeleteKey(CONST.PLAYER_PREF_SETTING_DATA);
		PlayerPrefs.DeleteKey(CONST.PLAYER_PREF_USER_DATA);
	}
}