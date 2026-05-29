using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject hudPanel;

    [Header("Levels")]
    public GameObject level1;
    public GameObject level2;
    public GameObject level3;

    void Start()
    {
        // Set starting state
        mainMenuPanel.SetActive(true);
        hudPanel.SetActive(false);

        level1.SetActive(false);
        level2.SetActive(false);
        level3.SetActive(false);
    }

    public void StartLevel1()
    {
        mainMenuPanel.SetActive(false);
        hudPanel.SetActive(true);
        level1.SetActive(true);
    }
}