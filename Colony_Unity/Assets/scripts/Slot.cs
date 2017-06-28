using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Xml;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler {

	private Vector3 pos;

	public Vector3 MyPosition {
		get {
			return pos;
		}
		set {
			pos = value;
		}
	}

	private string save;
	public string SaveFile {
		get {
			if (save == null || save == "") {
				save = GameObject.Find ("SaveFile").GetComponent<InputField>().text;
			}
			return save;
		}
		set {
			save = value;
		}
	}

	public int height {
		get {
			if (transform.name == "Root") {
				return 0;
			} else {
				Transform parent1 = transform.parent;
				if (parent1) {
					Transform parent2 = parent1.transform.parent;
					if (parent2) {
						int temp = parent2.GetComponent<Slot> ().height;
						return temp + 1;
					}
				}
			}
			return -1;
		}
	}

	public GameObject item {
		get {
			if (transform.childCount > 0) {
				return transform.GetChild (0).gameObject;
			}
			return null;
		}
	}

	public void SaveTree(string path) {
		XmlDocument doc = new XmlDocument( );
		XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration( "1.0", "UTF-8", null );
		XmlElement root = doc.DocumentElement;
		doc.InsertBefore( xmlDeclaration, root );
	
		GameObject previous;

		if (MyNode.nodes.TryGetValue (MyNode.instanceID, out previous)) {
			MyNode prevMyNode = previous.GetComponent<MyNode> ();
			prevMyNode.saveSelection ();
		}

		XmlElement element = ToXML(doc);
		doc.AppendChild( element );

		if (SaveFile != null && SaveFile != "") {
			path = SaveFile;
		}
		doc.Save( path );
	}

	public XmlElement ToXML(XmlDocument doc) {
		XmlElement slotElement = doc.CreateElement( string.Empty, "Slot", string.Empty );

		if (transform.childCount > 0) {
			Transform nodeObj = transform.GetChild (0).gameObject.transform;
			MyNode node = (MyNode)nodeObj.GetComponent<MyNode> ();
			XmlElement nodeElement = node.toXML (doc);
			slotElement.AppendChild( nodeElement );
			int children = nodeObj.childCount;
			if (children > 0) {
				for (int i = 0; i < children; ++i) {
					nodeElement.AppendChild(nodeObj.GetChild (i).GetComponent<Slot>().ToXML(doc));
				}
			}
		}
		return slotElement;
	}

	public override string ToString() {
		System.Text.StringBuilder builder = new System.Text.StringBuilder ();
		builder.Append (name + "\n");
		if (transform.childCount > 0) {
			Transform node = transform.GetChild (0).gameObject.transform;
			builder.Append (node.name + "\n");
			int children = node.childCount;
			if (children > 0) {
				for (int i = 0; i < children; ++i) {
					builder.Append(node.GetChild (i).GetComponent<Slot>().ToString() + "\n");
				}
			}
		} 
		return builder.ToString();
	}

	#region IDropHandler implementation

	public void OnDrop (PointerEventData eventData)
	{
		if (!item) {
			MyNode.itemBeingDragged.transform.SetParent (transform);
		}
	}

	#endregion


}
