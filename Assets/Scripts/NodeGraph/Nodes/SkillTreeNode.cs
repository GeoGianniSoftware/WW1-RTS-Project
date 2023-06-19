// public classes deriving from Node are registered as nodes for use within a graph
public class SkillTreeNode : XNode.Node
{
    // Adding [Input] or [Output] is all you need to do to register a field as a valid port on your node 

    public string name;
    [Input] public SkillTreeNode prerequisite;
    
    // The value of an output node field is not used for anything, but could be used for caching output results
    [Output] public SkillTreeNode unlocked;

    // The value of 'mathType' will be displayed on the node in an editable format, similar to the inspector
    public UnlockType mathType = UnlockType.Buff;
    public enum UnlockType { Ability, Buff}

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