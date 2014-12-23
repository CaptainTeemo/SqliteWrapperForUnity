using UnityEngine;
using System.Collections;

public class AppController : MonoBehaviour {

	// Use this for initialization
	IEnumerator Start () {
		Application.targetFrameRate = 60;
		yield return StartCoroutine(DBManager.sharedManager().copyFileToDevice("PersistentData.db"));
		yield return StartCoroutine(DBManager.sharedManager().copyFileToDevice("UserData.db"));
		Application.LoadLevel("UI");
	}
}
