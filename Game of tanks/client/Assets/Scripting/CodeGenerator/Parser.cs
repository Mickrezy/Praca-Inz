using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using org.mariuszgromada.math.mxparser;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Threading;
using Random = UnityEngine.Random;

public class Parser : MonoBehaviour
{

    public GameObject tankContoller;
    public CodeGenerator generator;
    public Text text;
    public Button compileButton;

    public GameObject battleGround;
    public GameObject buttonList;

    public CodeGenerator generatorInstance;

    public List<CreatedFunction> functionList = new List<CreatedFunction>();

    public Color tankColor;
    public float colorDiff;


    public GameObject destructController;

    public int tanksLimit = 3;

    public GameObject[] tanks;

    public float tankCooldown = 10f;

    public float tankTimer = 0;

    private bool error = false;


    private string[] tankFuncs = new string[] {
        "MocPrawegoSilnika",
        "MocLewegoSilnika",
        "CzekajNaKomende",
        "ObrocWiezeWPrawo",
        "ObrocWiezeWLewo",
        "Strzel"};

    public Dictionary<string, CodeElement> codeElements = new Dictionary<string, CodeElement>
    {
        { "powtórz", new Repeat{elementName = "powtórz"} },
        { "ustaw zmienną", new Variable{elementName = "ustaw zmienną" } },
        { "warunek", new IfStatement{elementName = "warunek" } },
        { "MocPrawegoSilnika", new Command{elementName = "MocPrawegoSilnika", argsNumber = 1} },
        { "MocLewegoSilnika", new Command{elementName = "MocLewegoSilnika", argsNumber = 1} },
        { "CzekajNaKomende", new Command{elementName = "CzekajNaKomende", argsNumber = 1} },
        { "ObrocWiezeWPrawo", new Command{elementName = "ObrocWiezeWPrawo", argsNumber = 1} },
        { "ObrocWiezeWLewo", new Command{elementName = "ObrocWiezeWLewo", argsNumber = 1} },
        { "Strzel", new Command{elementName = "Strzel", argsNumber = 0} }
    };

    void Start()
    {

        //setup "main" function as root
        generatorInstance = (CodeGenerator)UnityEngine.Object.Instantiate(generator, this.transform);
        generatorInstance.showButton.transform.localScale = new Vector3(0, 0, 0);
        generatorInstance.elementsList.transform.localScale = new Vector3(1, 1, 1);
        generatorInstance.elementsList.transform.localPosition = new Vector3(0, 0, 0);
        generatorInstance.elementsList.interactable = false;
        generatorInstance.elementsList.captionText.text = "WykonajPolecenia";

        generatorInstance.openBracketPrefab.transform.localScale = new Vector3(1, 1, 1);

        generatorInstance.transform.position = new Vector3(0, 0, 0);

        generatorInstance.parser = this;
        generatorInstance.codePanel = this.gameObject;
        generatorInstance.growPanel(2);

        //setup actual code
        CodeGenerator childrenGenerator = UnityEngine.Object.Instantiate(generator, generatorInstance.childrenPanel.transform);
        childrenGenerator.transform.position = new Vector3(0, 0, 0);

        childrenGenerator.parser = generatorInstance.parser;
        childrenGenerator.codePanel = generatorInstance.childrenPanel;
        childrenGenerator.parent = generatorInstance;

        //close bracket
        generatorInstance.closeBracketInstance = UnityEngine.Object.Instantiate(generatorInstance.closeBracketPrefab, generatorInstance.codePanel.transform);


        //setup compile button
        compileButton.onClick.AddListener(ParseTheCode);

        tankTimer = GameManager.instance.timer;
    }

    private void Update()
    {

        int count = 0;
        for (int i = 0; i < tanksLimit; i++)
            count += tanks[i] ? 1 : 0;

        if (tankTimer - GameManager.instance.timer < tankCooldown && !GameManager.instance.training)
        {
            compileButton.interactable = false;
            compileButton.GetComponentInChildren<Text>().text = (tankCooldown - (int)(tankTimer - GameManager.instance.timer)).ToString();
        }
        else if (tanksLimit > count)
        {
            tankCooldown = 3.0f;
            compileButton.interactable = true;
            compileButton.GetComponentInChildren<Text>().text = "Walcz! " + string.Format("({0}/{1})", count, tanksLimit);
        }
        else
        {
            compileButton.interactable = false;
            compileButton.GetComponentInChildren<Text>().text = "Osiagnieto limit";
        }
    }

    void ParseTheCode()
    {
        tankTimer = GameManager.instance.timer;
        StartCoroutine(ParseTheCode(generatorInstance.printCode()));
    }

    IEnumerator ParseTheCode(string input)
    {
        LogPlease(input,false);

        Dictionary<String, KeyValuePair<int, string>> parsedFunctions = new Dictionary<string, KeyValuePair<int, string>>();
        foreach(var func in functionList){
            parsedFunctions.Add(func.elementName, new KeyValuePair<int, string>(func.argsNames.Count, func.code.printCode()));
        }
        error = false;
        var tank = Instantiate(tankContoller, battleGround.transform);
        tank.GetComponentInChildren<CannonController>().battleGround = battleGround;
        var tankDestroyButton = Instantiate(destructController, buttonList.transform);

        for (int i = 0; i < tanksLimit; i++)
        {
            if (!tanks[i])
            {
                tanks[i] = tank;
                var sprites = tank.GetComponentsInChildren<SpriteRenderer>();
                Color newColor = new Color(tankColor.r - colorDiff * i,
                    tankColor.g - colorDiff * i,
                    tankColor.b - colorDiff * i, tankColor.a);
                var image = tankDestroyButton.GetComponentsInChildren<Image>();

                foreach (var sprite in sprites)
                {
                    sprite.color = newColor;
                }

                image[1].color = newColor;
                break;
            }
        }        

        var script = tankDestroyButton.GetComponent<DestroyButtonManager>();
        script.tank = tank;
        script.buttonList = buttonList;
        script.parser = this;
        buttonList.GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
        buttonList.GetComponent<HorizontalLayoutGroup>().SetLayoutHorizontal();
        var watch = System.Diagnostics.Stopwatch.StartNew();
        yield return StartCoroutine(ParseTheCode(input, new Dictionary<string, float>(), 1, tank,parsedFunctions));
        if (!error)
        {
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            text.text = "wykonano polecenia w " + elapsedMs + "ms";
        }
        Destroy(tank);
        Destroy(tankDestroyButton);
        buttonList.GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
        buttonList.GetComponent<HorizontalLayoutGroup>().SetLayoutHorizontal();

    }

    
    // parsing autogenerated code
    IEnumerator ParseTheCode(string input, Dictionary<string, float> higherVariables, int lineNumber, GameObject tank, Dictionary<String, KeyValuePair<int, string>> parsedFunctions)
    {
        //LogPlease("Parsing started...",false);
        // recursive variables
        var variables = new Dictionary<string, float>();

        foreach (var variable in higherVariables)
        {
            variables.Add(variable.Key, variable.Value);
        }


        //parsing lines

        StringReader strReader = new StringReader(input);

        string line;
        while (null != (line = strReader.ReadLine()))
        {
            string[] splittedLine = line.Split(';');
            switch (splittedLine[0])
            {
                //r - repeat
                case "r":
                    int startLineNumber = lineNumber + 2;
                    int repeatCount = (int)ComputeExpressionIntoValue(splittedLine[1], variables, lineNumber);
                    if (null != (line = strReader.ReadLine()) && line.Trim() == "{")
                    {
                        int closeLoop = 1;
                        StringWriter strWriter = new StringWriter();
                        do
                        {
                            line = strReader.ReadLine().Trim();
                            switch(line){
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
                            lineNumber++;
                        } while (closeLoop != 0);
                        for (int i = 0; i < repeatCount; i++)
                            yield return StartCoroutine(ParseTheCode(strWriter.ToString(), variables, startLineNumber, tank, parsedFunctions));
                    }
                    else
                    {
                        ReportErrorInTheCode("Pętla powinna rozpoczynać się od znaku \"{\"");
                    }
                    lineNumber++;
                    break;
                //v - variable
                case "v":
                    LogPlease("var is detected",false);
                    string varName = splittedLine[1].Trim();

                    float value = ComputeExpressionIntoValue(splittedLine[2], variables, lineNumber);

                    if (variables.ContainsKey(varName))
                        variables[varName] = value;
                    else
                        variables.Add(varName, value);
                    break;
                //c - command(built-in functions)
                case "c":
                    foreach (string func in tankFuncs)
                    {
                        
                        if (func == splittedLine[1] && tank)
                        {
                            if (func == "CzekajNaKomende")
                            {
                                yield return new WaitForSeconds((float)ComputeExpressionIntoValue(splittedLine[2], variables, lineNumber) / 1000f);
                                break;
                            }
                            else if (func == "Strzel")
                            {
                                tank.SendMessage(splittedLine[1]);
                                break;
                            }
                            tank.SendMessage(splittedLine[1], ComputeExpressionIntoValue(splittedLine[2], variables, lineNumber));
                        }
                    }
                    break;
                //f - function
                case "f":
                    foreach(var elem in parsedFunctions)
                    {
                        if (elem.Key == splittedLine[1])
                        {
                            var funcArgs = new Dictionary<string, float>();
                            for (int i = 0; i < elem.Value.Key; i++)
                            {
                                try
                                {
                                    funcArgs.Add(splittedLine[i * 2 + 2], ComputeExpressionIntoValue(splittedLine[i * 2 + 3], variables, lineNumber));
                                }
                                catch(Exception e)
                                {
                                    ReportErrorInTheCode("funkcja " + elem.Key + " ma źle zdefiniowane argumenty!");
                                }
                            }

                            yield return StartCoroutine(ParseTheCode(elem.Value.Value, funcArgs, 1, tank, parsedFunctions));
                        }
                    }
                    break;
                //i - if statement(not sure if necessary in this usage)
                case "i":
                    int compare = ComputeExpressionIntoValue(splittedLine[1], variables, lineNumber).CompareTo(
                        ComputeExpressionIntoValue(splittedLine[3], variables, lineNumber));
                    
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
                            lineNumber++;
                        } while (closeLoop != 0);
                        if ((compare == 1 && (splittedLine[2] == ">" || splittedLine[2] == ">=")) ||
                            (compare == 0 && (splittedLine[2] == "==" || splittedLine[2] == ">=" || splittedLine[2] == "<=")) ||
                            (compare == -1 && (splittedLine[2] == "<" || splittedLine[2] == "<=")))
                        {
                            yield return StartCoroutine(ParseTheCode(strWriter.ToString(), variables, lineNumber + 2, tank, parsedFunctions));
                        }
                    }
   
                    lineNumber++;
                    
                    break;
            }
            lineNumber++;
        }

        //return variables that are available higher
        foreach (var variable in variables)
        {
            if (higherVariables.ContainsKey(variable.Key))
            {
                higherVariables[variable.Key] = variable.Value;
            }
        }
        yield return null;
    }


    private float ComputeExpressionIntoValue(string v, Dictionary<string,float> variables, int lineNumber)
    {
        //string varExpression = CheckForVariables(variable[1]);
        Expression e = new Expression(v);
        Argument[] a = new Argument[variables.Count];
        int i = 0;
        foreach (var vari in variables)
        {
            a.SetValue(new Argument(vari.Key, vari.Value), i);
            i++;
        }
        e.addArguments(a);
		if (!e.checkSyntax ()) {
			ReportErrorInTheCode ("wystąpił błąd przy liczeniu wyrażenia w linii " + lineNumber + ".");
		} else {
			float value = (float)e.calculate ();
			return value;
		}
		return 0;
    }

    private void ReportErrorInTheCode(string msg)
    {
        error = true;
        Debug.Log(msg);
        text.text = msg;
    }

    private void LogPlease(string msg, bool forUser)
    {
        Debug.Log(msg);
        if (forUser)
            text.text = msg;
    }

}