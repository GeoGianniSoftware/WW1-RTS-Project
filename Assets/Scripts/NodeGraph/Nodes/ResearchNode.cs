// public classes deriving from Node are registered as nodes for use within a graph
using XNode;
using UnityEngine;
using System.Collections.Generic;

[NodeTint("#627f67")]
[System.Serializable]
public class ResearchNode : Node
{
    // Adding [Input] or [Output] is all you need to do to register a field as a valid port on your node 
    [Input] public ResearchNode prerequisite;
    
    public ResearchEntry entry;
    public bool completed = false;
    // The value of an output node field is not used for anything, but could be used for caching output results
    [Output] public List<ResearchNode> unlocks;

    private void OnValidate() {
        if(entry != null) {
            name = entry.name;
        }
    }

    public List<ResearchNode> getOutputs() {
        List<ResearchNode> temp = new List<ResearchNode>();
        foreach (XNode.NodePort port in Outputs) {
            for (int i = 0; i < port.ConnectionCount; i++) {
                ResearchNode tempNode = port.GetConnection(i).node as ResearchNode;
                temp.Add(tempNode);
            }
        }

        return temp;
    }

    public List<ResearchNode> getInputs() {
        List<ResearchNode> temp = new List<ResearchNode>();
        foreach (XNode.NodePort port in Inputs) {
            for (int i = 0; i < port.ConnectionCount; i++) {
                ResearchNode tempNode = port.GetConnection(i).node as ResearchNode;
                temp.Add(tempNode);
            }
        }

        return temp;
    }

    // The value of 'mathType' will be displayed on the node in an editable format, similar to the inspector
    //public bool canMove;
    //public float speedMod;

    // GetValue should be overridden to return a value for any specified output port
    public override object GetValue(XNode.NodePort port) {

        // Get new a and b values from input connections. Fallback to field values if input is not connected
        /*float a = GetInputValue<float>("a", this.a);
        float b = GetInputValue<float>("b", this.b);

        // After you've gotten your input values, you can perform your calculations and return a value
        if (port.fieldName == "result")
            switch (mathType) {
                case MathType.Add: default: return a + b;
                case MathType.Subtract: return a - b;
                case MathType.Multiply: return a * b;
                case MathType.Divide: return a / b;
            }
        else if (port.fieldName == "sum") return a + b;
        else return 0f;
        */
        return 0f;
    }
}