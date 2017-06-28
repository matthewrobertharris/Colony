using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartUp.init ();
		MyNode[] nodes = GameObject.FindObjectsOfType<MyNode> ();
		for (int i = 0; i < nodes.Length; i++) {
			nodes[i].id = i;
			MyNode.idCounter = i;
			MyNode.nodes.Add (i, nodes [i].gameObject);
		}
		MyNode.idCounter += 1;
	}

	public static void init () {
		GameObject.Find ("Left_1").transform.localScale = new Vector3 (0, 0, 0);
		GameObject.Find ("Left_2").transform.localScale = new Vector3 (0, 0, 0);
		GameObject.Find ("Right_1").transform.localScale = new Vector3 (0, 0, 0);
		GameObject.Find ("Right_2").transform.localScale = new Vector3 (0, 0, 0);
		GameObject.Find ("Left_Input_1").transform.localScale = new Vector3 (0, 0, 0);
		GameObject.Find ("Left_Input_2").transform.localScale = new Vector3 (0, 0, 0);
		GameObject.Find ("Left_Input_3").transform.localScale = new Vector3 (0, 0, 0);
		GameObject.Find ("Right_Input_1").transform.localScale = new Vector3 (0, 0, 0);
		GameObject.Find ("Right_Input_2").transform.localScale = new Vector3 (0, 0, 0);
		GameObject.Find ("Right_Input_3").transform.localScale = new Vector3 (0, 0, 0);
	}
}
