
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeGenerator : MonoBehaviour {

    public Dropdown elementsList;
    public Button showButton;
    public Button hideButton;
    public Button addAboveButton;
    public Parser parser;
    public InputField variableName;
    public InputField compareValue;
    public Dropdown symbolDropdown;
    public List<InputField> arguments;
    public GameObject codePanel;
    public GameObject childrenPanel;
    public GameObject openBracketPrefab;
    public GameObject closeBracketPrefab;

    public InputField functionName;
    public Button addArgButton;
    public Button removeArgButton;

    public GameObject closeBracketInstance;

    public CodeGenerator parent;
    public List<CodeGenerator> children = new List<CodeGenerator>();

    public CreatedFunction createdFunction;

    public bool insert = false;
    public int onIndex = 0;

    public int panelSize = 1;

    public string selectedItem = "";

    // Use this for initialization
    void Start () {
        elementsList.Hide();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ShowElementList()
    {
        elementsList.transform.localScale = new Vector3(1, 1, 1);
        elementsList.ClearOptions();
        elementsList.options.Add(new Dropdown.OptionData() { text = "wybierz polecenie" });
        foreach (var element in parser.codeElements)
        {
            elementsList.options.Add(new Dropdown.OptionData() { text = element.Key});
        }
        foreach (var element in parser.functionList)
        {
            elementsList.options.Add(new Dropdown.OptionData() { text = element.elementName });
        }

        elementsList.captionText.text = elementsList.options[elementsList.value].text;
        elementsList.Show();
        hideButton.transform.localScale = new Vector3(1, 1, 1);
        showButton.transform.localScale = new Vector3(0, 0, 0);
    }

    public void HideElementList()
    {
        //if it is not root delete whole element
        if (selectedItem == "")
            return;
        if (parent != null)
        {
            parent.shrinkPanel(panelSize);
            parent.children.Remove(this);

            Object.Destroy(closeBracketInstance);
            Object.Destroy(gameObject);
        }
        elementsList.transform.localScale = new Vector3(0,0,0);
        elementsList.Hide();
        hideButton.transform.localScale = new Vector3(0,0,0);
        showButton.transform.localScale = new Vector3(1,1,1);
        foreach(var el in arguments)
        {
            el.transform.localScale = new Vector3(0, 0, 0);
        }
        FindObjectOfType<FuncCreator>().SaveFunctions();
    }

    public void SelectedItem()
    {
        addAboveButton.interactable = true;
        selectedItem = elementsList.options[elementsList.value].text;
        if (selectedItem != "wybierz polecenie")
        {
            if(parser.codeElements.ContainsKey(selectedItem))
                parser.codeElements[selectedItem].SelectedEvent(this);
            else
            {
                parser.functionList.ForEach(elem =>
                {
                    if(elem.elementName == selectedItem)
                    {
                        elem.SelectedEvent(this);
                    }
                });
            }
        }
        FindObjectOfType<FuncCreator>().SaveFunctions();
    }

    public void AddCodeAbove()
    {
        CodeGenerator newElement = Object.Instantiate(parser.generator, parent.childrenPanel.transform);
        newElement.parser = parser;
        newElement.codePanel = parent.childrenPanel;
        newElement.parent = parent;
        newElement.insert = true;
        newElement.onIndex = parent.children.IndexOf(this);
        newElement.ShowElementList();
        newElement.transform.SetSiblingIndex(transform.GetSiblingIndex());
        parent.growPanel(1);
    }

    //first == 0 means that this is growth of parent of element
    //this prevents unstoppable growth of space in recursive calls
    public void growPanel(int panelSize)
    {
        var tr = GetComponent<RectTransform>();
        this.panelSize += panelSize;

        tr.sizeDelta = new Vector2(tr.sizeDelta.x, tr.sizeDelta.y + panelSize * 35);
        if (parent != null)
        {
            parent.growPanel(panelSize);
        }
        else
        {
            var tr2 = codePanel.GetComponent<RectTransform>();
            tr2.sizeDelta = new Vector2(tr2.sizeDelta.x, (this.panelSize + 1) * 35 + 15);
        }
        codePanel.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        codePanel.GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
    }

    public void shrinkPanel(int panelSize)
    {
        this.panelSize -= panelSize;

        var tr = GetComponent<RectTransform>();
        tr.sizeDelta = new Vector2(tr.sizeDelta.x, tr.sizeDelta.y - panelSize * 35);

        if (parent != null)
            parent.shrinkPanel(panelSize);
        else
        {
            var tr2 = codePanel.GetComponent<RectTransform>();
            tr2.sizeDelta = new Vector2(tr2.sizeDelta.x, (this.panelSize + 1) * 35 + 15);
        }

        codePanel.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
        codePanel.GetComponent<VerticalLayoutGroup>().SetLayoutVertical(); 
    }

    public void widthenPanel(int size)
    {
        var tr = GetComponent<RectTransform>();

        tr.sizeDelta = new Vector2(tr.sizeDelta.x + size, tr.sizeDelta.y);

        codePanel.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputHorizontal();
        codePanel.GetComponent<VerticalLayoutGroup>().SetLayoutHorizontal();
        codePanel.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
    }

    public string printCode()
    {
        string result = "";
        if (selectedItem != "wybierz polecenie" && selectedItem != "WykonajPolecenia" && selectedItem != "")
            if(parser.codeElements.ContainsKey(selectedItem))
                result += parser.codeElements[selectedItem].printStartCode(this);
            else
                parser.functionList.ForEach(elem =>
                {
                    if (elem.elementName == selectedItem)
                    {
                        result += elem.printStartCode(this);
                    }
                });

        //foreach (var child in children)
        //{
        //    result += child.printCode();
        //}
        var childrenCount = childrenPanel.transform.childCount;
        for (int i = 0; i < childrenCount; i++)
        {
            
            var code = childrenPanel.transform.GetChild(i).GetComponent<CodeGenerator>();
            if (code != null) result += code.printCode();
        }
        if (selectedItem != "wybierz polecenie" && selectedItem != "WykonajPolecenia" && selectedItem != "")
            if (parser.codeElements.ContainsKey(selectedItem))
                result += parser.codeElements[selectedItem].printEndCode(this);
            else
                parser.functionList.ForEach(elem =>
                {
                    if (elem.elementName == selectedItem)
                    {
                        result += elem.printEndCode(this);
                    }
                });
        return result;
    }

    public void changeFuncName()
    {
        string newFuncName = functionName.text;

        if (newFuncName.Contains(";"))
        {
            functionName.text = createdFunction.elementName;
            return;
        }

        foreach (var elem in parser.functionList)
        {
            if (elem.elementName == newFuncName)
            {
                functionName.text = createdFunction.elementName;
                return;
            }
        }

        createdFunction.label.GetComponent<Text>().text = newFuncName;
        createdFunction.elementName = newFuncName;
        FindObjectOfType<FuncCreator>().SaveFunctions();
    }

    public void addArgument()
    {
        int lastIndex = createdFunction.argsNames.Count;

        arguments.Add(Object.Instantiate(arguments[lastIndex], transform));
        arguments[lastIndex].transform.localScale = new Vector3(1, 1, 1);
        arguments[lastIndex].placeholder.GetComponent<Text>().text = "nazwa arg" + lastIndex;
        arguments[lastIndex].onEndEdit.AddListener(delegate{ changeArgName(lastIndex); });

        var changePosition = arguments[lastIndex + 1].transform.localPosition;
        arguments[lastIndex + 1].transform.localPosition = new Vector3(changePosition.x + 105, changePosition.y, changePosition.z);

        createdFunction.argsNames.Add("argument" + lastIndex);

        addArgButton.transform.localPosition = new Vector3(changePosition.x + 105, changePosition.y, changePosition.z);
        widthenPanel(100);

        removeArgButton.transform.localScale = Vector3.one;
        FindObjectOfType<FuncCreator>().SaveFunctions();
    }

    public void addArgument(string name)
    {
        int lastIndex = createdFunction.argsNames.Count;

        arguments.Add(Object.Instantiate(arguments[lastIndex], transform));
        arguments[lastIndex].transform.localScale = new Vector3(1, 1, 1);
        arguments[lastIndex].text = name;
        arguments[lastIndex].onEndEdit.AddListener(delegate { changeArgName(lastIndex); });

        var changePosition = arguments[lastIndex + 1].transform.localPosition;
        arguments[lastIndex + 1].transform.localPosition = new Vector3(changePosition.x + 105, changePosition.y, changePosition.z);

        createdFunction.argsNames.Add(name);

        addArgButton.transform.localPosition = new Vector3(changePosition.x + 105, changePosition.y, changePosition.z);
        widthenPanel(100);

        removeArgButton.transform.localScale = Vector3.one;
    }

    public void removeArgument()
    {
        int lastIndex = createdFunction.argsNames.Count;
        var changePosition = arguments[lastIndex].transform.localPosition;


        arguments[lastIndex - 1].transform.localScale = new Vector3(0,0,0);
        createdFunction.argsNames.RemoveAt(lastIndex - 1);
        Object.Destroy(arguments[lastIndex]);
        arguments.RemoveAt(lastIndex);

        addArgButton.transform.localPosition = new Vector3(changePosition.x - 105, changePosition.y, changePosition.z);
        widthenPanel(-100);

        if(lastIndex == 1)
            removeArgButton.transform.localScale = Vector3.zero;
        FindObjectOfType<FuncCreator>().SaveFunctions();
    }

    public void changeArgName(int index)
    {
        createdFunction.argsNames[index] = arguments[index].text;
        FindObjectOfType<FuncCreator>().SaveFunctions();
    }
}
