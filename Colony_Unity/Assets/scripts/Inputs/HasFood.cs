using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.EventSystems;

public class HasFood : MyNode {

	private GameObject position;

	public override void outFocus() {
		saveSelection();
		if (position != null) {
			(position.GetComponent<MyNode> ()).outFocus ();
		}
		instanceID = int.MinValue;
		if (position != null) {
			position.transform.parent = null;
		}
		Selected.text = "";
		StartUp.init ();
	}

	public override void onFocus() {
		GameObject previous;

		if (nodes.TryGetValue (instanceID, out previous)) {
			if (previous != null) {
				MyNode prevMyNode = previous.GetComponent<MyNode> ();
				prevMyNode.outFocus ();
			}
		}
		if (instanceID != id) {
			instanceID = id;
			Selected.text = transform.name + " " + id;
			GameObject leftObj = GameObject.Find ("Left_1");
			leftObj.transform.localScale = new Vector3 (1, 1, 1);
			if (position != null) {
				position.transform.SetParent(leftObj.transform);
				(position.GetComponent<MyNode> ()).onFocus ();
			}
		}
	}

	public override void saveSelection () {
		if (GameObject.Find ("Left_1").transform.childCount > 0) {
			position = GameObject.Find ("Left_1").transform.GetChild(0).gameObject;
			(position.GetComponent<MyNode> ()).saveSelection ();
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

	public override void DropMyNode() {
		Vector3 pos = transform.position;
		AddSlot (pos, transform);
		AddSlot (pos, transform);
	}
}
