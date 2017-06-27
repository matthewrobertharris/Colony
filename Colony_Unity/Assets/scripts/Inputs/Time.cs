using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.EventSystems;

public class Time : Node {

	private int time;
	private bool focus = false;

	public override void onFocus() {
		if (transform.parent.name == "Left_1") {
			GameObject.Find ("Left_Input_1").transform.localScale = new Vector3 (1, 1, 1);
			(GameObject.Find ("Left_Input_1").GetComponent<InputField> ()).text = time.ToString ();
		} else if (transform.parent.name == "Right_1") {
			GameObject.Find ("Right_Input_1").transform.localScale = new Vector3 (1, 1, 1);
			(GameObject.Find ("Right_Input_1").GetComponent<InputField> ()).text = time.ToString ();
		}
		focus = true;
	}

	public override void saveSelection () {
		if (focus) {
			if (transform.parent.name == "Left_1") {
				int.TryParse ((GameObject.Find ("Left_Input_1").GetComponent<InputField> ()).text, out time);
			} else if (transform.parent.name == "Right_1") {
				int.TryParse ((GameObject.Find ("Right_Input_1").GetComponent<InputField> ()).text, out time);
			}
		}
	}

	public override XmlElement toXML(XmlDocument doc) {
		XmlElement element = doc.CreateElement( string.Empty, this.name, string.Empty );
		element.SetAttribute ("time", time.ToString());
		return element;
	}

	public override void outFocus() {
		saveSelection();
		if (transform.parent.name == "Left_1") {
			(GameObject.Find ("Left_Input_1").GetComponent<InputField> ()).text = "";
			GameObject.Find ("Left_Input_1").transform.localScale = new Vector3 (0, 0, 0);
		} else if (transform.parent.name == "Right_1") {
			(GameObject.Find ("Right_Input_1").GetComponent<InputField> ()).text = "";
			GameObject.Find ("Right_Input_1").transform.localScale = new Vector3 (0, 0, 0);
		}
		focus = false;
	}

	public override bool validPosition() {
		return transform.parent.name == "Left_1" || transform.parent.name == "Right_1";
	}

	public override void DropNode() {
		onFocus ();
	}

	public override void OnBeginDrag (PointerEventData eventData)
	{
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		startParent = transform.parent;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		outFocus ();
	}
}
