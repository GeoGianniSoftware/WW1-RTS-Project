using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[CreateAssetMenu]
public class ResearchGraph : NodeGraph {

	public void Reset() {
		foreach (ResearchNode node in nodes) {
			node.completed = false;
		}
	}

}