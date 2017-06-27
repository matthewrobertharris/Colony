using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.EventSystems;

public class DirectionTo : Node {

	private GameObject position;

	public override void onFocus() {
		GameObject previous;

		if (nodes.TryGetValue (instanceID, out previous)) {
			if (previous != null) {
				Node prevNode = previous.GetComponent<Node> ();
				prevNode.outFocus ();
			}
		}
		if (instanceID != id) {
			instanceID = id;
			Selected.text = transform.name + " " + id;
			GameObject.Find ("Left_1").transform.localScale = new Vector3 (1, 1, 1);
			if (position != null) {
				position.transform.parent = GameObject.Find ("Left_1").transform;
				(position.GetComponent<Node> ()).onFocus ();
			}

		}
	}

	public override void saveSelection() {
		if (GameObject.Find ("Left_1").transform.childCount > 0) {
			position = GameObject.Find ("Left_1").transform.GetChild(0).gameObject;
			(position.GetComponent<Node> ()).saveSelection ();
		} else {
			position = null;
		}
	}

	public override void outFocus() {
		// save selection to previous current node (i.e. the current one)
		// undisplay all the visuals
		saveSelection();
		instanceID = int.MinValue;
		if (position != null) {
			(position.GetComponent<Node> ()).outFocus ();
			position.transform.parent = null;
		}
		Selected.text = "";
		StartUp.init ();
	}

	void OnDestroy() { 
		if (position != null) {
			GameObject.Destroy (position);
		}
	}

	/*
	public override void updateXML(XmlElement nodeElement) {
		if (position != null) {
			nodeElement.SetAttribute ("position", position.name);
		}
	}*/

	public override void DropNode() {
		Vector3 pos = transform.position;
		AddSlot (pos, transform);
		AddSlot (pos, transform);
		AddSlot (pos, transform);
		AddSlot (pos, transform);
		AddSlot (pos, transform);
		onFocus ();
	}

	public override XmlElement toXML(XmlDocument doc) {
		XmlElement element = doc.CreateElement( string.Empty, this.name, string.Empty );
		if (position != null) {
			XmlElement positionInput =  doc.CreateElement( string.Empty, "position", string.Empty );
			positionInput.AppendChild((position.GetComponent<Node>()).toXML(doc));
			element.AppendChild (positionInput);
		}
		return element;
	}
}
