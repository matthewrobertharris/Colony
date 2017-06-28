using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.EventSystems;

public class GetY : MyNode {

	private GameObject position;

	public override void outFocus() {
		saveSelection();
		if (position != null) {
			(position.GetComponent<MyNode> ()).outFocus ();
		}
		if (transform.parent.name == "Left_1") {
			GameObject.Find ("Left_2").transform.localScale = new Vector3 (0,0,0);
		} else if (transform.parent.name == "Right_1") {
			GameObject.Find ("Right_2").transform.localScale = new Vector3 (0,0,0);
		}
	}

	public override void onFocus() {
		if (transform.parent.name == "Left_1") {
			GameObject.Find ("Left_2").transform.localScale = new Vector3 (1, 1, 1);
			if (position != null) {
				position.transform.parent = GameObject.Find ("Left_2").transform;
				(position.GetComponent<MyNode> ()).onFocus ();
			}
		} else if (transform.parent.name == "Right_1") {
			GameObject.Find ("Right_2").transform.localScale = new Vector3 (1, 1, 1);
			if (position != null) {
				position.transform.parent = GameObject.Find ("Right_2").transform;
				(position.GetComponent<MyNode> ()).onFocus ();
			}
		}
	}

	public override void saveSelection () {
		if (transform.parent.name == "Left_1") {
			if (GameObject.Find ("Left_2").transform.childCount > 0) {
				position = GameObject.Find ("Left_2").transform.GetChild (0).gameObject;
				(position.GetComponent<MyNode> ()).saveSelection ();
			}
		} else if (transform.parent.name == "Right_1") {
			if (GameObject.Find ("Right_2").transform.childCount > 0) {
				position = GameObject.Find ("Right_2").transform.GetChild (0).gameObject;
				(position.GetComponent<MyNode> ()).saveSelection ();
			}
		} else {
			position = null;
		}
	}

	public override XmlElement toXML(XmlDocument doc) {
		XmlElement element = doc.CreateElement( string.Empty, this.name, string.Empty );
		if (position != null) {
			XmlElement positionInput =  doc.CreateElement( string.Empty, "position", string.Empty );
			positionInput.AppendChild((position.GetComponent<MyNode>()).toXML(doc));
			element.AppendChild (positionInput);
		}
		return element;
	}

	public override bool validPosition() {
		return transform.parent.name == "Left_1" || transform.parent.name == "Right_1";
	}

	public override void DropMyNode() {
		onFocus ();
	}

	void OnDestroy() { 
		if (position != null) {
			GameObject.Destroy (position);
		}
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
