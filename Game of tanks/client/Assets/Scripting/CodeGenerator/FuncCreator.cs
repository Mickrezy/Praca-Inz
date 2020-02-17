using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

public class FuncCreator : MonoBehaviour {

    public Parser parser;
    public GameObject editorField;
    public GameObject funcListField;
    public ScrollRect functionScollRect;

    public GameObject labelPrefab;

    private FunctionLoader functionLoader;

    private string selectedFunction;

	// Use this for initialization
	void Awake () {
        functionLoader = new FunctionLoader { parser = parser, funcCreator = this };
        functionLoader.LoadFunctions();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ExportToFunction()
    {
        bool exist = false;
        foreach (var func in parser.functionList)
        {
            if(func.elementName == "ZCentrumDowodzenia")
            {
                exist = true;
                func.code.children.ForEach(child => { child.HideElementList(); });
                functionLoader.LoadCode(FindObjectOfType<Parser>().generatorInstance.printCode(), func.code);
            }
        }
        if (exist)
            return;
        CreatedFunction createdFunction = NewFunction();

        createdFunction.label.GetComponent<Text>().text = "ZCentrumDowodzenia";
        createdFunction.code.functionName.text = "ZCentrumDowodzenia";
        createdFunction.elementName = "ZCentrumDowodzenia";

        functionLoader.LoadCode(FindObjectOfType<Parser>().generatorInstance.printCode(), createdFunction.code);
    }

    public void AddNewFunction()
    {
        NewFunction();
    }

    public CreatedFunction NewFunction()
    {
        foreach(var func in parser.functionList)
        {
            func.code.codePanel.transform.localScale = Vector3.zero;
        }

        string newFuncName = "NowaFunkcja" + parser.functionList.Count;

        GameObject field = Object.Instantiate(editorField, editorField.transform.parent);
        GameObject label = Object.Instantiate(labelPrefab, funcListField.transform);
        functionScollRect.content = field.GetComponent<RectTransform>();

        label.GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener(delegate { SwitchFunction(label); });
#pragma warning disable CS0618 // Type or member is obsolete
        label.active = true;
#pragma warning restore CS0618 // Type or member is obsolete
        label.GetComponent<Text>().text = newFuncName;


        CodeGenerator codeGenerator = Object.Instantiate(parser.generator, field.transform);
        codeGenerator.parser = parser;
        codeGenerator.showButton.transform.localScale = Vector3.zero;
        codeGenerator.functionName.transform.localScale = new Vector3(1, 1, 1);
        codeGenerator.functionName.text = newFuncName;
        codeGenerator.addArgButton.transform.localScale = new Vector3(1, 1, 1);
        codeGenerator.codePanel = field;

        CreatedFunction createdFunction = new CreatedFunction { label = label, code = codeGenerator, elementName= newFuncName, argsNames = new List<string>()};
        codeGenerator.createdFunction = createdFunction;

        parser.functionList.Add(createdFunction);

        codeGenerator.growPanel(2);
        codeGenerator.panelSize++;

        //open bracket
        codeGenerator.openBracketPrefab.transform.localScale = new Vector3(1, 1, 1);

        //children code
        CodeGenerator childrenGenerator = Object.Instantiate(parser.generator, codeGenerator.childrenPanel.transform);
        childrenGenerator.transform.position = new Vector3(0, 0, 0);

        childrenGenerator.parser = codeGenerator.parser;
        childrenGenerator.codePanel = codeGenerator.childrenPanel;
        childrenGenerator.parent = codeGenerator;

        //close bracket
        codeGenerator.closeBracketInstance = Object.Instantiate(codeGenerator.closeBracketPrefab, codeGenerator.codePanel.transform);


        return createdFunction;
    }

    public void SaveFunctions()
    {
        functionLoader.SaveFunctions();
    }

    public void SwitchFunction(GameObject label)
    {
        selectedFunction = label.GetComponent<Text>().text;
        foreach (var func in parser.functionList)
        {
            if (func.elementName == selectedFunction)
            {
                func.code.codePanel.transform.localScale = Vector3.one;
                functionScollRect.content = func.code.codePanel.GetComponent<RectTransform>();

            }
            else
                func.code.codePanel.transform.localScale = Vector3.zero;
        }
    }

    public void DeleteFunction()
    {
        foreach (var func in parser.functionList)
        {
            if (func.elementName == selectedFunction)
            {
                Object.Destroy(func.code.codePanel);
                Object.Destroy(func.label);
                parser.functionList.Remove(func);
            }
        }
    }
}
