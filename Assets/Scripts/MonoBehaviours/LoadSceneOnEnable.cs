using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnEnable : MonoBehaviour {
	public string sceneToLoad;

	private void OnEnable() {
		SceneManager.LoadScene(sceneToLoad);
	}
}
