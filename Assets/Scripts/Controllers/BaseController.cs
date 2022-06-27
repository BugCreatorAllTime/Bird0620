using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class BaseController : MonoBehaviour {
	private static MainController _mainController;

	public static MainController MainController {
		get {
			if (_mainController == null) _mainController = FindObjectOfType<MainController>();
			return _mainController;
		}
	}

	[HideInInspector] public bool IsDestroyed = false;

	public virtual void Awake() {
		IsDestroyed = false;
	}

	public virtual void Start() {
	}

	public virtual void OnDestroy() {
		IsDestroyed = true;
	}

	public static UnityEngine.Sprite LoadSprite(string resources) {
		return Resources.Load<UnityEngine.Sprite>(resources);
	}

	public static T InstantiatePrefab<T>(string resources) {
		return InstantiatePrefab(resources).GetComponent<T>();
	}

	public static GameObject InstantiatePrefab(string resources) {
		return Instantiate(Resources.Load<GameObject>(resources));
	}

	public static void InvokeAction(UnityEvent action) {
		if (action != null) action.Invoke();
	}

	public static void InvokeAction<T>(Action<T> action, T param) {
		if (action != null) action.Invoke(param);
	}

	public static void InvokeAction<T>(List<Action<T>> actions, T param) {
		if (actions == null) return;
		actions.ForEach(action => { InvokeAction(action, param); });
	}

	public static void InvokeAction(List<Action> actions) {
		if (actions == null) return;
		actions.ForEach(InvokeAction);
	}

	public static void InvokeAction(Action action) {
		if (action != null) action.Invoke();
	}

	public static string SerializeObject(object obj) {
		return JsonConvert.SerializeObject(obj);
	}

	public static T CloneObject<T>(T obj) {
		return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
	}

	public static void RemoveAllChildren(Transform parentTransform) {
		foreach (Transform child in parentTransform) {
			Destroy(child.gameObject);
		}
	}

	public static void ForEach<T0, T>(Dictionary<T0, Dictionary<T0, T>> twoDimensionDictionary, Action<T> callback) {
		if (twoDimensionDictionary == null) return;
		foreach (var keyValuePair in twoDimensionDictionary) {
			foreach (var valuePair in keyValuePair.Value) {
				InvokeAction(callback, valuePair.Value);
			}
		}
	}
}