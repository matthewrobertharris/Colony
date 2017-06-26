using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.EventSystems;

public class XYValue : Positions {

	private int x;
	private int y;
	// This is used to keep track, basically, of whether the input boxes are alive or not to receive input.  It's a bit of a hack at the moment, but is working
	private bool focus = false;

	public bool SetValues() {
		if (transform.parent.name == "Left_1") {
			return SetXY ("Left_Input_1", "Left_Input_2");
		} else if (transform.parent.name == "Right_1") {
			return SetXY ("Right_Input_1", "Right_Input_2");
		} if (transform.parent.name == "Left_2") {
			return SetXY ("Left_Input_2", "Left_Input_3");
		} if (transform.parent.name == "Right_2") {
			return SetXY ("Right_Input_2", "Right_Input_3");
		} else {
			return false;
		}
	}

	public bool SetXY(string xName, string yName) {
		bool resultX = int.TryParse (GameObject.Find (xName).GetComponent<InputField> ().text, out x);
		bool resultY = int.TryParse (GameObject.Find (yName).GetComponent<InputField> ().text, out y);
		return resultX && resultY;
	}
		
	public override void saveSelection () {
		if (focus) {
			SetValues ();
		}
	}

	public override void outFocus() {
		saveSelection ();
		if (transform.parent.name == "Left_1") {
			(GameObject.Find ("Left_Input_1").GetComponent<InputField>()).text = "";
			(GameObject.Find ("Left_Input_2").GetComponent<InputField>()).text = "";
			GameObject.Find ("Left_Input_1").transform.localScale = new Vector3 (0, 0, 0);
			GameObject.Find ("Left_Input_2").transform.localScale = new Vector3 (0, 0, 0);
		}
		else if (transform.parent.name == "Left_2") {
			(GameObject.Find ("Left_Input_2").GetComponent<InputField>()).text = "";
			(GameObject.Find ("Left_Input_3").GetComponent<InputField>()).text = "";
			GameObject.Find ("Left_Input_2").transform.localScale = new Vector3 (0, 0, 0);
			GameObject.Find ("Left_Input_3").transform.localScale = new Vector3 (0, 0, 0);

		}
		else if (transform.parent.name == "Right_1") {
			(GameObject.Find ("Right_Input_1").GetComponent<InputField>()).text = "";
			(GameObject.Find ("Right_Input_2").GetComponent<InputField>()).text = "";
			GameObject.Find ("Right_Input_1").transform.localScale = new Vector3 (0, 0, 0);
			GameObject.Find ("Right_Input_2").transform.localScale = new Vector3 (0, 0, 0);
		}
		else if (transform.parent.name == "Right_2") {
			(GameObject.Find ("Right_Input_2").GetComponent<InputField>()).text = "";
			(GameObject.Find ("Right_Input_3").GetComponent<InputField>()).text = "";
			GameObject.Find ("Right_Input_2").transform.localScale = new Vector3 (0, 0, 0);
			GameObject.Find ("Right_Input_3").transform.localScale = new Vector3 (0, 0, 0);
		}
		focus = false;
	}
		
	public override void onFocus() {
		
		transform.localScale = new Vector3 (1, 1, 1);
		if (transform.parent.name == "Left_1") {
			GameObject.Find ("Left_Input_1").transform.localScale = new Vector3 (1, 1, 1);
			(GameObject.Find ("Left_Input_1").GetComponent<InputField>()).text = x.ToString();
			GameObject.Find ("Left_Input_2").transform.localScale = new Vector3 (1, 1, 1);
			(GameObject.Find ("Left_Input_2").GetComponent<InputField>()).text = y.ToString();
		}
		else if (transform.parent.name == "Left_2") {
			GameObject.Find ("Left_Input_2").transform.localScale = new Vector3 (1, 1, 1);
			(GameObject.Find ("Left_Input_2").GetComponent<InputField>()).text = x.ToString();
			GameObject.Find ("Left_Input_3").transform.localScale = new Vector3 (1, 1, 1);
			(GameObject.Find ("Left_Input_3").GetComponent<InputField>()).text = y.ToString();
		}
		else if (transform.parent.name == "Right_1") {
			GameObject.Find ("Right_Input_1").transform.localScale = new Vector3 (1, 1, 1);
			(GameObject.Find ("Right_Input_1").GetComponent<InputField>()).text = x.ToString();
			GameObject.Find ("Right_Input_2").transform.localScale = new Vector3 (1, 1, 1);
			(GameObject.Find ("Right_Input_2").GetComponent<InputField>()).text = y.ToString();
		}
		else if (transform.parent.name == "Right_2") {
			GameObject.Find ("Right_Input_2").transform.localScale = new Vector3 (1, 1, 1);
			(GameObject.Find ("Right_Input_2").GetComponent<InputField>()).text = x.ToString();
			GameObject.Find ("Right_Input_3").transform.localScale = new Vector3 (1, 1, 1);
			(GameObject.Find ("Right_Input_3").GetComponent<InputField>()).text = y.ToString();
		}
		// This is for saving the selection, so if you drag a selection from left-right/right-left, it keeps track of the selection's child objects
		/*GameObject selection;
		if (Node.nodes.TryGetValue (Node.instanceID, out selection)) {
			Node selectionNode = selection.GetComponent<Node> ();
			selectionNode.saveSelection ();
		}*/
		focus = true;
	}

	public override XmlElement toXML(XmlDocument doc) {
		XmlElement element = doc.CreateElement( string.Empty, this.name, string.Empty );
		element.SetAttribute ("x", x.ToString());
		element.SetAttribute ("y", y.ToString());
		return element;
	}

	public override void OnBeginDrag (PointerEventData eventData)
	{
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		startParent = transform.parent;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
		outFocus ();
		// There is a problem when dragging something into the selection slot that this doesn't work for.  
		//onFocus ();
	}



}
