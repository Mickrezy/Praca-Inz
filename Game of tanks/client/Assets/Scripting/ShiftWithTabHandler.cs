using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShiftWithTabHandler : MonoBehaviour {

    public EventSystem eventSystem;

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            try
            {
                eventSystem.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown().Select();
            }
            catch (Exception e)
            {

            }
        }
    }
}
