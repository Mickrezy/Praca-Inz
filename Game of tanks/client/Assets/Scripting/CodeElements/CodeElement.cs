using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CodeElement {

    public string elementName { get; set; }

    abstract public void SelectedEvent(CodeGenerator generator);

    abstract public string printStartCode(CodeGenerator generator);

    abstract public string printEndCode(CodeGenerator generator);

    protected void addNextCode(CodeGenerator generator)
    {
        //next code
        if (!generator.insert)
        {
            CodeGenerator codeGenerator = Object.Instantiate(generator.parser.generator, generator.codePanel.transform);
            codeGenerator.transform.position = new Vector3(0, 0, 0);

            codeGenerator.parser = generator.parser;
            codeGenerator.codePanel = generator.codePanel;
            codeGenerator.parent = generator.parent;

            generator.parent.growPanel(1);
        }

    }

    protected void addChildrenCode(CodeGenerator generator)
    {
        generator.growPanel(2);
        generator.panelSize++;

        //open bracket
        generator.openBracketPrefab.transform.localScale = new Vector3(1, 1, 1);

        //children code
        CodeGenerator childrenGenerator = Object.Instantiate(generator.parser.generator, generator.childrenPanel.transform);
        childrenGenerator.transform.position = new Vector3(0, 0, 0);

        childrenGenerator.parser = generator.parser;
        childrenGenerator.codePanel = generator.childrenPanel;
        childrenGenerator.parent = generator;

        //close bracket
        generator.closeBracketInstance = Object.Instantiate(generator.closeBracketPrefab, generator.codePanel.transform);
        generator.closeBracketInstance.transform.SetSiblingIndex(generator.transform.GetSiblingIndex() + 1);
    }
}
