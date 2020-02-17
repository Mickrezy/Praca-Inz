using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Repeat : CodeElement {

    public override void SelectedEvent(CodeGenerator generator)
    {
        generator.elementsList.interactable = false;
        generator.arguments[0].transform.localScale = new Vector3(1, 1, 1);

        addChildrenCode(generator);

        addNextCode(generator);

        if(generator.parent != null)
        {
            generator.parent.growPanel(1);
        }


        generator.parent.children.Add(generator);
    }

    public override string printStartCode(CodeGenerator generator)
    {
        var value = "";
        value += "r;" + generator.arguments[0].text + "\n";
        value += "{\n";
        return value;
    }

    public override string printEndCode(CodeGenerator generator)
    {
        return "}\n";
    }

}
