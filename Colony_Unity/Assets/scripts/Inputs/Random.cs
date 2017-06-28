using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.EventSystems;

public class Random : MyNode {

	public override void onFocus() {
	}

	public override void saveSelection () {
	}

	public override XmlElement toXML(XmlDocument doc) {
		XmlElement element = doc.CreateElement( string.Empty, this.name, string.Empty );
		return element;
	}
		

	public override bool validPosition() {
		return transform.parent.name == "Left_1" || transform.parent.name == "Right_1";
	}

	public override void DropMyNode() {
	}
}
