using UnityEngine;
using UnityEngine.UI;
using TMPro; // Crucial: Always use TextMeshPro for UI

public class UIManager : MonoBehaviour
{
    [Header("Dependencies")]
    public TickManager tickManager;

    [Header("UI Elements")]
    public TextMeshProUGUI quotaText;
    public Button toggleModeButton;
    public TextMeshProUGUI toggleButtonText;
    public Image toggleButtonImage;

    [Header("Colors")]
    public Color buildModeColor = new Color(0.2f, 0.6f, 1f); // Blue
    public Color playModeColor = new Color(1f, 0.4f, 0.2f);  // Orange

    private void OnEnable()
    {
        // Listen for score changes
        FactoryEvents.OnQuotaUpdated += UpdateQuotaDisplay;

        // Hook up the button click event via code instead of the inspector for safety
        toggleModeButton.onClick.AddListener(OnToggleClicked);
    }

    private void OnDisable()
    {
        FactoryEvents.OnQuotaUpdated -= UpdateQuotaDisplay;
        toggleModeButton.onClick.RemoveListener(OnToggleClicked);
    }

    private void Start()
    {
        // Set initial button visuals
        UpdateButtonVisuals();
    }

    private void UpdateQuotaDisplay(int current, int target)
    {
        // Formats the text to look like: "Quota: 3 / 10"
        quotaText.text = $"Quota: {current} / {target}";
    }

    private void OnToggleClicked()
    {
        // Flip the simulation state
        tickManager.isSimulationRunning = !tickManager.isSimulationRunning;

        UpdateButtonVisuals();
    }

    private void UpdateButtonVisuals()
    {
        if (tickManager.isSimulationRunning)
        {
            toggleButtonText.text = "STOP (Build Mode)";
            toggleButtonImage.color = playModeColor;
        }
        else
        {
            toggleButtonText.text = "PLAY (Run Factory)";
            toggleButtonImage.color = buildModeColor;
        }
    }
}