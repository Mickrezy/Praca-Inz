using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyButtonManager : MonoBehaviour {

    public GameObject tank;
    public GameObject buttonList;
    public Parser parser;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(tank == null)
        {
            gameObject.SetActive(false);
            buttonList.GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
            buttonList.GetComponent<HorizontalLayoutGroup>().SetLayoutHorizontal();
            Destroy(this);
        }
	}

    public void OnClickDestroyTank ()
    {
        Destroy(tank);
        gameObject.SetActive(false);
        buttonList.GetComponent<HorizontalLayoutGroup>().CalculateLayoutInputHorizontal();
        buttonList.GetComponent<HorizontalLayoutGroup>().SetLayoutHorizontal();
        Destroy(this);


    }
}
