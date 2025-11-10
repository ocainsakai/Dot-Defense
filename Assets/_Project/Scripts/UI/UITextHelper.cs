using System;
using TMPro;
using UnityEngine;

public class UITextHelper : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] private string head;
    [SerializeField] private string middle;
    [SerializeField] private string end;

    private void Start()
    {
        UpdateText();
    }

    public void SetHead(string head)
    {
        this.head = head;
        UpdateText();   
    }

    public void SetMiddle(string middle)
    {
        this.middle = middle;
        UpdateText();
    }

    public void SetMiddle(int middle)
    {
        this.middle = middle.ToString();
        UpdateText();
    }
    public void SetEnd(string end)
    {
        this.end = end;
        UpdateText();
    }

    private void UpdateText()
    {
        text.text = $"{head} {middle} {end}";
    }
}

