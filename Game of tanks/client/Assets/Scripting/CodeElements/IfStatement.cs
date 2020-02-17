using UnityEngine;

public class IfStatement : CodeElement
{

    public override string printStartCode(CodeGenerator generator)
    {
        var value = "";
        value += "i;" + generator.compareValue.text + ";" + generator.symbolDropdown.options[generator.symbolDropdown.value].text + ";" + generator.arguments[0].text + "\n";
        value += "{\n";
        return value;
    }

    public override string printEndCode(CodeGenerator generator)
    {
        return "}\n";
    }

    public override void SelectedEvent(CodeGenerator generator)
    {
        generator.elementsList.transform.localScale = new Vector3(0, 0, 0);
        generator.compareValue.transform.localScale = new Vector3(1, 1, 1);
        generator.symbolDropdown.transform.localScale = new Vector3(1, 1, 1);
        generator.arguments[0].transform.localScale = new Vector3(1, 1, 1);

        addChildrenCode(generator);

        addNextCode(generator);

        if (generator.parent != null)
        {
            generator.parent.growPanel(1);
        }


        generator.parent.children.Add(generator);
    }
}