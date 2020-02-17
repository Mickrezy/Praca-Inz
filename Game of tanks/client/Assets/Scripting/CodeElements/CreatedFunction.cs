using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatedFunction : CodeElement {

    public GameObject label { get; set; }
    public CodeGenerator code { get; set; }
    public List<string> argsNames { get; set; }

    public override string printStartCode(CodeGenerator generator)
    {
        string result = "f;" + elementName;

        for (int i = 0; i < argsNames.Count; i++)
        {
            result += ";" + argsNames[i] + ";" + generator.arguments[i].text;
        }

        result += "\n";

        return result;
    }

    public override string printEndCode(CodeGenerator generator)
    {
        return "";
    }

    public override void SelectedEvent(CodeGenerator generator)
    {
        generator.elementsList.interactable = false;

        for (int i = 0; i < argsNames.Count; i++)
        {
            generator.arguments.Add(Object.Instantiate(generator.arguments[i],generator.transform));
            generator.arguments[i].transform.localScale = new Vector3(1, 1, 1);
            generator.arguments[i].placeholder.GetComponent<Text>().text = argsNames[i];

            var changePosition = generator.arguments[i + 1].transform.localPosition;
            generator.arguments[i + 1].transform.localPosition = new Vector3(changePosition.x + 105, changePosition.y, changePosition.z);
        }
        
        addNextCode(generator);

        generator.parent.children.Add(generator);
    }
}
