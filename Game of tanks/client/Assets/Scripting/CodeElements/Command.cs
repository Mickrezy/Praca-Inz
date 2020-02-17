using System.Collections;
using UnityEngine;

public class Command : CodeElement{

    public int argsNumber { get; set; }

    public override string printStartCode(CodeGenerator generator)
    {
        var value = "c;" + elementName + ";";
        if(argsNumber > 0)
            value += generator.arguments[0].text;
        value += "\n";

        return value;
    }

    public override string printEndCode(CodeGenerator generator)
    {
        return "";
    }

    public override void SelectedEvent(CodeGenerator generator)
    {
        generator.elementsList.interactable = false;

        if(argsNumber > 0)
            generator.arguments[0].transform.localScale = new Vector3(1, 1, 1);
        
        addNextCode(generator);

        generator.parent.children.Add(generator);
    }
}
