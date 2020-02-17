
using UnityEngine;

public class Variable : CodeElement
{
    public override string printStartCode(CodeGenerator generator)
    {
        //return generator.variableName.text + " = " + generator.arguments[0].text + "\n";
        return "v;" + generator.variableName.text + ";" + generator.arguments[0].text + "\n";
    }

    public override string printEndCode(CodeGenerator generator)
    {
        return "";
    }

    public override void SelectedEvent(CodeGenerator generator)
    {

        generator.arguments[0].transform.localScale = new Vector3(1, 1, 1);
        generator.elementsList.transform.localScale = new Vector3(0, 0, 0);
        generator.variableName.transform.localScale = new Vector3(1, 1, 1);

        addNextCode(generator);

        generator.parent.children.Add(generator);
    }
}
