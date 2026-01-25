using System;
using UnityEngine;
using UnityEngine.InputSystem.Editor;
using UnityEngine.UI;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField] private Button[] buttons;
    private int index = 0;

    private void Start()
    {
        SelectButton(0);
        ButtonHoverSelect.OnButtonHoverr += UIButton_OnButtonHoverr;
    }

    private void UIButton_OnButtonHoverr(int obj)
    {
        index = obj;
        SelectButton(obj);
    }

    private void OnDestroy()
    {
        ButtonHoverSelect.OnButtonHoverr -= UIButton_OnButtonHoverr;
    }

    private void SelectButton(int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.GetChild(0).gameObject.SetActive(false);
        }


        buttons[index].transform.GetChild(0).gameObject.SetActive(true);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            index--;
        index = Mathf.Clamp(index, 0, buttons.Length);

            SelectButton(index);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {

            index++;
            index = Mathf.Clamp(index, 0, buttons.Length-1);
            SelectButton(index);

        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            buttons[index].onClick.Invoke();
        }

    }
}
