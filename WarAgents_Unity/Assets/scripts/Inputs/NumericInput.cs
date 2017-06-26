using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.EventSystems;

public class NumericInput : Node {

	private GameObject left;
	private GameObject right;

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
			GameObject leftObj = GameObject.Find ("Left_1");
			GameObject rightObj = GameObject.Find ("Right_1");
			leftObj.transform.localScale = new Vector3 (1, 1, 1);
			rightObj.transform.localScale = new Vector3 (1, 1, 1);
			if (left != null) {
				left.transform.SetParent(leftObj.transform);
				(left.GetComponent<Node> ()).onFocus ();
			}

			if (right != null) {
				right.transform.SetParent(rightObj.transform);
				(right.GetComponent<Node> ()).onFocus ();
			}

		}
	}

	public override void saveSelection() {
		left = getSelection ("Left_1");
		if (left != null) {
			(left.GetComponent<Node> ()).saveSelection ();
		}
		right = getSelection ("Right_1");
		if (right != null) {
			(right.GetComponent<Node> ()).saveSelection ();
		}
	}

	private GameObject getSelection(string value) {
		if (GameObject.Find (value).transform.childCount > 0) {
			return GameObject.Find (value).transform.GetChild(0).gameObject;
		} else {
			return null;
		}
	}

	public override void outFocus() {
		saveSelection();
		if (left != null) {
			(left.GetComponent<Node> ()).outFocus ();
		}
		if (right != null) {
			(right.GetComponent<Node> ()).outFocus ();
		}
		instanceID = int.MinValue;
		if (left != null) {
			left.transform.parent = null;
		}
		if (right != null) {
			right.transform.parent = null;
		}
		Selected.text = "";
		StartUp.init ();
	}

	void OnDestroy() { 
		if (left != null) {
			GameObject.Destroy (left);
		}
		if (right != null) {
			GameObject.Destroy (right);
		}
	}

	public override XmlElement toXML(XmlDocument doc) {
		XmlElement element = doc.CreateElement( string.Empty, this.name, string.Empty );
		if (left != null) {
			XmlElement leftInput =  doc.CreateElement( string.Empty, "left", string.Empty );
			leftInput.AppendChild((left.GetComponent<Node>()).toXML(doc));
			element.AppendChild (leftInput);
		}
		if (right != null) {
			XmlElement rightInput =  doc.CreateElement( string.Empty, "right", string.Empty );
			rightInput.AppendChild((right.GetComponent<Node>()).toXML(doc));
			element.AppendChild (rightInput);
		}
		return element;
	}
}
