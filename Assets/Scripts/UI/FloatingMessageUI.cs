using TMPro;
using UnityEngine;

public class FloatingMessageUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI messageTxt;

    [Header("Text colors")]
    [SerializeField]
    private Color info;
    [SerializeField]
    private Color error;
    [SerializeField]
    private Color warning;

    public void SetupUI(string message, FloatingMessageType type)
    {
        messageTxt.text = message;

        switch(type)
        {
            case FloatingMessageType.Error:
                messageTxt.color = error;
                break;
            case FloatingMessageType.Warning:
                messageTxt.color = warning; 
                break;
            case FloatingMessageType.Info: 
                messageTxt.color = info;
                break;
        }
    }
}
