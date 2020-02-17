using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

public class FunctionLoader
{
    public Parser parser { get;  set; }
    public FuncCreator funcCreator { get; set; }

    public void SaveFunctions()
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/Functions.dat");

        List<SavedFunction> savedFuntions = new List<SavedFunction>();
        foreach (var func in parser.functionList)
        {
            SavedFunction savedFunction = new SavedFunction();
            savedFunction.funcName = func.elementName;
            savedFunction.argsList = func.argsNames;
            savedFunction.code = func.code.printCode();
            savedFuntions.Add(savedFunction);
        }

        binaryFormatter.Serialize(file, savedFuntions);
        file.Close();
    }

    public void LoadFunctions()
    {
        if (File.Exists(Application.persistentDataPath + "/Functions.dat"))
        {
            BinaryFormatter binaryFromatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Functions.dat", FileMode.Open);

            List<SavedFunction> savedFuntions = (List<SavedFunction>)binaryFromatter.Deserialize(file);

            Dictionary<SavedFunction, CreatedFunction> loadedFunctions = new Dictionary<SavedFunction, CreatedFunction>();

            foreach (var func in savedFuntions)
            {
                CreatedFunction createdFunction = funcCreator.NewFunction();

                createdFunction.label.GetComponent<Text>().text = func.funcName;
                createdFunction.code.functionName.text = func.funcName;
                createdFunction.elementName = func.funcName;

                loadedFunctions.Add(func, createdFunction);
                foreach(var arg in func.argsList)
                {
                    createdFunction.code.addArgument(arg);
                }
            }
            foreach (var func in loadedFunctions)
            {
                LoadCode(func.Key.code, func.Value.code);
            }
            file.Close();
        }
    }

    private CodeGenerator GetLastChild(CodeGenerator code)
    {
        return code.childrenPanel.transform.GetChild(code.childrenPanel.transform.childCount - 1).GetComponent<CodeGenerator>();
    }

    public void LoadCode(string input, CodeGenerator code)
    {
        StringReader strReader = new StringReader(input);

        string line;
        while (null != (line = strReader.ReadLine()))
        {
            string[] splittedLine = line.Split(';');
            CodeGenerator child = GetLastChild(code);
            switch (splittedLine[0])
            {
                //r - repeat
                case "r":
                    selectElement(child, "powtórz");
                    child.arguments[0].text = splittedLine[1];
                    if (null != (line = strReader.ReadLine()) && line.Trim() == "{")
                    {
                        int closeLoop = 1;
                        StringWriter strWriter = new StringWriter();
                        do
                        {
                            line = strReader.ReadLine().Trim();
                            switch (line)
                            {
                                case "{":
                                    closeLoop++;
                                    strWriter.WriteLine(line);
                                    break;
                                case "}":
                                    closeLoop--;
                                    if (closeLoop != 0)
                                        strWriter.WriteLine(line);
                                    break;
                                default:
                                    strWriter.WriteLine(line);
                                    break;
                            }
                        } while (closeLoop != 0);
                        LoadCode(strWriter.ToString(), child);
                    }
                    break;
                //v - variable
                case "v":
                    selectElement(child, "ustaw zmienną");
                    child.variableName.text = splittedLine[1];
                    child.arguments[0].text = splittedLine[2];
                    break;
                //c - command(built-in functions)
                case "c":
                    selectElement(child, splittedLine[1]);
                    child.arguments[0].text = splittedLine[2];
                    break;
                //f - function
                case "f":
                    selectFunction(child, splittedLine[1]);
                    for(int i = 2; i< splittedLine.Length; i += 2)
                    {
                        child.arguments[(i - 2) / 2].text = splittedLine[i + 1];
                    }
                    break;
                //i - if statement(not sure if necessary in this usage)
                case "i":
                    selectElement(child, "warunek");
                    child.compareValue.text = splittedLine[1];
                    child.symbolDropdown.captionText.text = splittedLine[2];
                    child.arguments[0].text = splittedLine[3];
                    child.symbolDropdown.options.ForEach(opt =>
                    {
                        if (opt.text == splittedLine[2])
                            child.symbolDropdown.value = child.symbolDropdown.options.IndexOf(opt);
                    });
                    if (null != (line = strReader.ReadLine()) && line.Trim() == "{")
                    {
                        int closeLoop = 1;
                        StringWriter strWriter = new StringWriter();
                        do
                        {
                            line = strReader.ReadLine().Trim();
                            switch (line)
                            {
                                case "{":
                                    closeLoop++;
                                    strWriter.WriteLine(line);
                                    break;
                                case "}":
                                    closeLoop--;
                                    if (closeLoop != 0)
                                        strWriter.WriteLine(line);
                                    break;
                                default:
                                    strWriter.WriteLine(line);
                                    break;
                            }
                        } while (closeLoop != 0);
                        LoadCode(strWriter.ToString(), child);
                    }
                    break;
            }
        }
    }

    private void selectFunction(CodeGenerator node, String function)
    {
        node.ShowElementList();
        node.addAboveButton.interactable = true;
        node.elementsList.captionText.text = function;
        node.selectedItem = function;
        node.parser.functionList.ForEach(elem =>
        {
            if (elem.elementName == function)
            {
                elem.SelectedEvent(node);
            }
        });
    }

    private void selectElement(CodeGenerator node, String element)
    {
        node.ShowElementList();
        node.addAboveButton.interactable = true;
        node.elementsList.captionText.text = element;
        node.selectedItem = element;
        node.parser.codeElements[element].SelectedEvent(node);     
    }


    [Serializable]
    class SavedFunction
    {
        public string funcName;
        public List<string> argsList;
        public string code;
    }
}