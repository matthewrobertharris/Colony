using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.EventSystems;

public class SetPrimary : MyNode {

	private GameObject primary;

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
			GameObject.Find ("Left_1").transform.localScale = new Vector3 (1, 1, 1);
			if (primary != null) {
				primary.transform.parent = GameObject.Find ("Left_1").transform;
				(primary.GetComponent<MyNode> ()).onFocus ();
			}

		}
	}

	public override void saveSelection() {
		if (GameObject.Find ("Left_1").transform.childCount > 0) {
			primary = GameObject.Find ("Left_1").transform.GetChild(0).gameObject;
			(primary.GetComponent<MyNode> ()).saveSelection ();
		} else {
			primary = null;
		}
	}

	public override void outFocus() {
		// save selection to previous current node (i.e. the current one)
		// undisplay all the visuals
		saveSelection();
		instanceID = int.MinValue;
		if (primary != null) {
			(primary.GetComponent<MyNode> ()).outFocus ();
			primary.transform.parent = null;
		}
		Selected.text = "";
		StartUp.init ();
	}

	void OnDestroy() { 
		if (primary != null) {
			GameObject.Destroy (primary);
		}
	}

	/*
	public override void updateXML(XmlElement nodeElement) {
		if (primary != null) {
			nodeElement.SetAttribute ("position", primary.name);
		}
	}*/

	public override XmlElement toXML(XmlDocument doc) {
		XmlElement element = doc.CreateElement( string.Empty, this.name, string.Empty );
		if (primary != null) {
			XmlElement primaryInput =  doc.CreateElement( string.Empty, "primary", string.Empty );
			primaryInput.AppendChild((primary.GetComponent<MyNode>()).toXML(doc));
			element.AppendChild (primaryInput);
		}
		return element;
	}
}
