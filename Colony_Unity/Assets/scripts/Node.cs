using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public int id;
	public static int instanceID = int.MinValue;
	public static int slotCounter = 0;
	public static int idCounter = 0;
	public static Dictionary<int, GameObject> nodes = new Dictionary<int, GameObject>();
	public static GameObject itemBeingDragged;
	[SerializeField] GameObject slot;
	protected Vector3 startPosition;
	protected Transform startParent;
	protected Text sel;
	public Text Selected {
		get {
			if (sel == null) {
				sel = GameObject.Find ("Selected").GetComponent<Text>();
			}
			return sel;
		}
	}


	#region IPointerDownHandler implementation
		
	// TODO The clicking to set focus doesn't quite work here
	public virtual void OnPointerDown( PointerEventData eventData )
	{
		//Debug.Log ("OnPointerDown");
	}

	#endregion

	public virtual void saveSelection() {
		// Not applicable for most Nodes, only those with stuff in the selection panel
		// Override in each sub-class
	}

	public virtual void onFocus() {
		// If the selection is in the main section
		// 		save selection to previous current node, if there was one
		// 		Get the new selection
		//		Load the necessary parts from the new selection
		// 		display the necessary parts (e.g. selected name)
		// 		undisplay the unnecessary parts
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
		}
	}

	public virtual void outFocus() {
		// save selection to previous current node (i.e. the current one)
		// undisplay all the visuals
		saveSelection();
		instanceID = int.MinValue;
		Selected.text = "";
		StartUp.init ();
	}

	#region IPointerUpHandler implementation

	public virtual void OnPointerUp( PointerEventData eventData )
	{
	}

	#endregion

	#region IPointerClickHandler implementation

	public virtual void OnPointerClick( PointerEventData eventData )
	{
		//Debug.Log ("OnPointerClick " + instanceID + " " + id);
		if (instanceID == id) {
			outFocus ();
		} else {
			onFocus ();
		}
	}

	#endregion

	#region IBeginDragHandler implementation

	public virtual void OnBeginDrag (PointerEventData eventData)
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

	public virtual void OnEndDrag (PointerEventData eventData)
	{
		itemBeingDragged = null;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;

		// This is to test whether the originals are going to the trash
		if (transform.parent.name == "Trash") {
			if (startParent.name == "SelectionSlot") {
				transform.parent = startParent;
				transform.position = startPosition;
				onFocus ();
			} else {
				GameObject.DestroyImmediate (transform.gameObject);
				Text sel = GameObject.Find ("Selected").GetComponent<Text> ();
				sel.text = "";
				instanceID = int.MinValue;
				StartUp.init ();
			}
			// This makes sure that the default is that selections cannot be made into the selection panel
		} else if (!validPosition ()) {
			transform.parent = startParent;
			transform.position = startPosition;
			onFocus ();
			// This is if there was no parent selected, ie it was dragged into space
		} else if (transform.parent == startParent) {
			transform.position = startPosition;
			onFocus ();
		} else if (startParent.name == "SelectionSlot") {
			GameObject newObj = GameObject.Instantiate (gameObject, startPosition, Quaternion.identity, startParent);
			newObj.name = gameObject.name;
			Node node = newObj.GetComponent<Node> ();
			Node.idCounter += 1;
			node.id = Node.idCounter;
			Node.nodes.Add (node.id, newObj);
			DropNode ();
			onFocus ();
		} else {
			// always, when the node is dropped, save the current selection, so it will save any selection nodes too
			GameObject selection;
			if (Node.nodes.TryGetValue (Node.instanceID, out selection)) {
				Node selectionNode = selection.GetComponent<Node> ();
				selectionNode.saveSelection ();
				//selectionNode.outFocus ();
				selectionNode.onFocus ();

			}

		}
		reorderNodes ();

	}

	#endregion

	public virtual bool validPosition() {
		return transform.parent.name != "Left_1" && transform.parent.name != "Left_2" && transform.parent.name != "Right_1" && transform.parent.name != "Right_2";
	}

	public virtual void DropNode() {
		Vector3 pos = transform.position;
		switch (transform.name) {
		case "PositionNode":
			//pos.y -= 100;
			AddSlot (pos, transform);
			AddSlot (pos, transform);
			AddSlot (pos, transform);
			AddSlot (pos, transform);
			AddSlot (pos, transform);
			//print ("Add 5 slots for position nodes (4 + current)");
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
		//onFocus ();
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
		Vector3 offset = new Vector3 (732, 342, 0);
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

	protected void AddSlot(Vector3 pos, Transform parent) {
		Transform newSlot = GameObject.Instantiate (slot.transform, pos, Quaternion.identity, parent);
		newSlot.name = newSlot.name + slotCounter++;
	}

	public virtual XmlElement toXML(XmlDocument doc) {
		XmlElement element = doc.CreateElement( string.Empty, this.name, string.Empty );
		return element;
	}
		
}
