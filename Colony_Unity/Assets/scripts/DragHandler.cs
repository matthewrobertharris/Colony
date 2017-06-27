using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public static GameObject itemBeingDragged;
	[SerializeField] GameObject slot;
	Vector3 startPosition;
	Transform startParent;
	int counter = 0;

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		startParent = transform.parent;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		transform.position = Input.mousePosition;
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		itemBeingDragged = null;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;

		if (transform.parent.name == "Trash") {
			if (startParent.name == "SelectionSlot") {
				transform.parent = startParent;
				transform.position = startPosition;
			} else {
				GameObject.DestroyImmediate (transform.gameObject);
				Text sel = GameObject.Find ("Selected").GetComponent<Text>();
				sel.text = "";
			}
		} else if(transform.parent == startParent) {
			transform.position = startPosition;
		} 
		else if (startParent.name == "SelectionSlot") {
			GameObject newObj = GameObject.Instantiate (gameObject, startPosition, Quaternion.identity, startParent);
			// TODO update the naming convention for creating new objects
			newObj.name = gameObject.name;
			DropNode ();
		}  
		reorderNodes ();
	}

	#endregion

	void DropNode() {
		Vector3 pos = transform.position;
		switch (transform.name) {
		case "PositionNode":
			//pos.y -= 100;
			AddSlot (pos, transform);
			AddSlot (pos, transform);
			AddSlot (pos, transform);
			AddSlot (pos, transform);
			//print ("Add 4 slots for position nodes");
			break;
		case "NumericNode":
			//pos.y -= 100;
			AddSlot (pos, transform);
			AddSlot (pos, transform);
			AddSlot (pos, transform);
			//print ("Add 3 slots for numeric nodes");
			break;
		case "BooleanNode":
			//pos.y -= 100;
			AddSlot (pos, transform);
			AddSlot (pos, transform);
			//print ("Add 2 slots for boolean nodes");
			break;
		case "OutputNode":
			//print ("Add no slots (output node goes here)");
			break;
		default:
			//print ("Invalid node selected");
			break;
		}
	}

	public static void reorderNodes() {
		Slot root = getRoot ();
		LinkedList<Slot> list = new LinkedList<Slot> ();
		int leaves = postOrder (root, list);
		LinkedList<Vector3> posList = updateNodePositions (list, leaves, 5);
		displayNodes (posList, list);
	}

	public static Slot getRoot() {
		return GameObject.Find ("Root").GetComponent<Slot>();
	}

	public static int postOrder(Slot slot, LinkedList<Slot> list) {
		int leaves = 0;
		list.AddFirst (slot);
		if (slot.transform.childCount > 0) {
			Transform node = slot.transform.GetChild (0).gameObject.transform;
			int children = node.childCount;
			if (children == 0) {
				leaves = 1;	// This is so the Output nodes don't remove a leave node
			} else {
				for (int i = 0; i < children; ++i) {
					leaves += postOrder (node.GetChild (i).GetComponent<Slot>(), list);
				}
			}
		} else {
				leaves = 1;
		}
		return leaves;
	}

	public static LinkedList<Vector3> updateNodePositions(LinkedList<Slot> list, int leaves, int maxHeight) {
		int cellWidth = 30;
		int cellHeight = 40;
		int curLeaves = 0;
		LinkedList<Vector3> positions = new LinkedList<Vector3> ();
		foreach (Slot slot in list) {
			if (slot.transform.childCount == 0) {
				positions.AddLast(updateNode (slot, curLeaves * cellWidth, slot.height * (-cellHeight)));
				curLeaves += 1;
			} else {
				Transform node = slot.transform.GetChild (0); // For output nodes
				if (node.childCount == 0) {
					positions.AddLast (updateNode (slot, curLeaves * cellWidth, slot.height * (-cellHeight)));
					curLeaves += 1;
				} else {
					positions.AddLast (updateBranchNode (slot, slot.height * (-cellHeight)));
				}
			}
		}
		return positions;
	}

	public static void displayNodes(LinkedList<Vector3> positions, LinkedList<Slot> list) {
		LinkedListNode<Slot> node1 = list.Last;
		Vector3 offset = new Vector3 (234, 342, 0);
		for (int i = 0; i < list.Count; i++) {
			Slot slot = (Slot)node1.Value;
			slot.transform.position = slot.MyPosition + offset;
			node1 = node1.Previous;
		}
	}
		

	public static Vector3 updateNode(Slot slot, float x, float y) {
		Vector3 pos = new Vector3 (x, y, 0);
		slot.MyPosition = pos;
		return pos;
	}

	public static Vector3 updateBranchNode(Slot slot, float y) {
		float x = slot.MyPosition.x;
		Transform node = slot.transform.GetChild (0);
		if (node.childCount > 0) {
			Slot child0 = (Slot)node.GetChild (0).GetComponent<Slot> ();
			float startX = child0.MyPosition.x;
			Slot childN = (Slot)node.GetChild (node.childCount - 1).GetComponent<Slot> ();
			float endX = childN.MyPosition.x;
			x = (startX + endX) / 2;
		}
		return updateNode (slot, x, y);
	}

	public static string outputList(LinkedList<Slot> list) {
		System.Text.StringBuilder builder = new System.Text.StringBuilder ();
		foreach (Slot t in list) {
			builder.Append (t.name + " ");
		}
		return builder.ToString();
	}

	void AddSlot(Vector3 pos, Transform parent) {
		Transform newSlot = GameObject.Instantiate (slot.transform, pos, Quaternion.identity, parent);
		newSlot.name = newSlot.name + counter++;
	}
}
