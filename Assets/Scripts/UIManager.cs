using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject inGameUI;
    public GameObject menuUI;
    public Button easyButton;
    public Button normalButton;
    public Button hardButton;
    public Button customButton;
    public GameObject quitUI;
    public Button noButton;
    public Button yesButton;
    public GameObject winMessage;
    public GameObject lostMessage;
    public GameObject customMenu;
    public Slider widthInput;
    public Slider heightInput;
    public Slider mineInput;
    public Text widthInputText;
    public Text heightInputText;
    public Text mineInputText;
    public Button customStartButton;
    public Text inGameMineCount;
    public Text inGameMinesFlagged;
    public GameObject inGameHelper;

    private void Start()
    {

    }
}
