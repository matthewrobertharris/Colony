using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.EventSystems;

public class SetSecondary : Node {

	private GameObject secondary;

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
			if (secondary != null) {
				secondary.transform.parent = GameObject.Find ("Left_1").transform;
				(secondary.GetComponent<Node> ()).onFocus ();
			}

		}
	}

	public override void saveSelection() {
		if (GameObject.Find ("Left_1").transform.childCount > 0) {
			secondary = GameObject.Find ("Left_1").transform.GetChild(0).gameObject;
			(secondary.GetComponent<Node> ()).saveSelection ();
		} else {
			secondary = null;
		}
	}

	public override void outFocus() {
		// save selection to previous current node (i.e. the current one)
		// undisplay all the visuals
		saveSelection();
		instanceID = int.MinValue;
		if (secondary != null) {
			(secondary.GetComponent<Node> ()).outFocus ();
			secondary.transform.parent = null;
		}
		Selected.text = "";
		StartUp.init ();
	}

	void OnDestroy() { 
		if (secondary != null) {
			GameObject.Destroy (secondary);
		}
	}

	/*
	public override void updateXML(XmlElement nodeElement) {
		if (secondary != null) {
			nodeElement.SetAttribute ("position", secondary.name);
		}
	}*/

	public override XmlElement toXML(XmlDocument doc) {
		XmlElement element = doc.CreateElement( string.Empty, this.name, string.Empty );
		if (secondary != null) {
			XmlElement secondaryInput =  doc.CreateElement( string.Empty, "secondary", string.Empty );
			secondaryInput.AppendChild((secondary.GetComponent<Node>()).toXML(doc));
			element.AppendChild (secondaryInput);
		}
		return element;
	}
}
