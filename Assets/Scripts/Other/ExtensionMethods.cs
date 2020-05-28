using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public static class Transforms {

	public static void DestroyChildren(this Transform t, bool destroyImmediately = false) {
		foreach (Transform child in t) {
			if (destroyImmediately) {
				MonoBehaviour.DestroyImmediate(child.gameObject);
			}
			else {
				MonoBehaviour.Destroy(child.gameObject);
			}
		}
	}
}

public static class MonoBehaviours {
	public static void ActivateChildren(this GameObject o, bool value) {
		for (int i = 0; i < o.transform.childCount; i++) {
			GameObject child = o.transform.GetChild(i).gameObject;
			if (child != null)
				child.SetActive(value);
		}
	}
}

public static class GameObjects {
	
	public static void Trigger(this GameObject self, Action evt)
	{
		if (evt != null)
		{
			evt.Invoke();
		}
		else
		{
			Debug.LogWarning("Tried to invoke a null UnityEvent");
		}
	}
	
	public static void Trigger<T>(this GameObject self, Action<T> evt, T data) {
		if (evt != null)
		{
			evt.Invoke(data);
		}
		else
		{
			Debug.LogWarning("Tried to invoke a null UnityEvent on " + self.name + " with type '" + typeof(T) + "' with the following payload: " + data);
		}
	}
}

public static class Strings {
	public static string UppercaseFirst(this string s) {
		if (string.IsNullOrEmpty(s)) {
			return string.Empty;
		}
		return char.ToUpper(s[0]) + s.Substring(1);
	}

	public static string SnakeCaseToCapitalizedCase(this string s) {
		if (string.IsNullOrEmpty(s)) {
			return string.Empty;
		}
		string[] sA = s.Split('_');
		for (int i = 0; i < sA.Length; i++) {
			sA[i] = sA[i].UppercaseFirst();
		}
		return string.Join(" ", sA);
	}

	public static string SnakeCaseToUpperCase(this string s) {
		if (string.IsNullOrEmpty(s)) {
			return string.Empty;
		}
		string[] sA = s.Split('_');
		for (int i = 0; i < sA.Length; i++) {
			sA[i] = sA[i].ToUpper();
		}
		return string.Join(" ", sA);
	}
}

public static class Lists
{
	public static void Shuffle<T>(this IList<T> list)
	{
		var rng = new Random();
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static T RandomItem<T>(this IList<T> list)
	{
		if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot select a random item from an empty list");
		return list[UnityEngine.Random.Range(0, list.Count)];
	}

	public static T RemoveRandom<T>(this IList<T> list)
	{
		if (list.Count == 0) throw new System.IndexOutOfRangeException("Cannot remove a random item from an empty list");
		int index = UnityEngine.Random.Range(0, list.Count);
		T item = list[index];
		list.RemoveAt(index);
		return item;
	}
	
	public static void AddIfAbsent<T>(this List<T> list, T item) {
		if (!list.Contains(item))
			list.Add(item);
	}
}

public static class Floats
{
	public static float LinearRemap(this float value,
		float valueRangeMin, float valueRangeMax,
		float newRangeMin, float newRangeMax)
	{
		return (value - valueRangeMin) / (valueRangeMax - valueRangeMin) * (newRangeMax - newRangeMin) + newRangeMin;
	}
}
