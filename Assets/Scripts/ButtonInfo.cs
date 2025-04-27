using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ButtonInfo
{
    public Button button;         // the left-panel Button
    public Sprite  image;         // the sprite to show
    [TextArea] public string text;// the description to show
}
