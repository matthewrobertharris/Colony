using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Positions : MyNode {

	public override bool validPosition() {
		return transform.parent.name == "Left_1" || transform.parent.name == "Left_2" || transform.parent.name == "Right_1" || transform.parent.name == "Right_2";
	}

	public override void onFocus() {
		transform.localScale = new Vector3 (1, 1, 1);
	}

	public override void DropMyNode() {
		onFocus ();
	}
		
	public override void outFocus() {

	}

}
