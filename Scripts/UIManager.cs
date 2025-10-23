using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{

    // ---- Canvases ----
    [Header("Canvases")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Canvas optionsCanvas;

    // ---- Panels under Options ----
    [Header("Options Panels")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject pillarSettingsPanel;
    [SerializeField] private GameObject obstacleSettingsPanel;
    [SerializeField] private GameObject soundSettingsPanel;

    private GameObject currentPanel;

    // ---- (Optional) Slider reference + helpers ----
    //[Header("Optional UI")]
    //[SerializeField] private Slider anySlider;

    public TMP_Text PillarDistanceValueText;
    public TMP_Text PillarDiameterValueText;
    public TMP_Text PathwayDistanceValueText;
    public TMP_Text SoundValueText;

    void Awake()
    {
        // Ensure Options starts with only the root visible
        ShowOnly(optionsPanel);
        currentPanel = optionsPanel;

        // Optional: ensure the right canvases are enabled at boot
        if (mainCanvas) mainCanvas.enabled = true;
        if (optionsCanvas) optionsCanvas.enabled = false;
    }

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("CorridorScene");
    }
    public void OnOptionClicked()
    {
        mainCanvas.enabled = false;
        optionsCanvas.enabled = true;
    }


    // ---------- Button hooks on Options root ----------
    public void ShowPillarSettings()
    {
        SwitchToSubPanel(pillarSettingsPanel);
    }

    public void ShowObstacleSettings()
    {
        SwitchToSubPanel(obstacleSettingsPanel);
    }

    public void ShowSoundSettings()
    {
        SwitchToSubPanel(soundSettingsPanel);
    }

    public void OnBackFromSubpanel()
    {
        if (currentPanel && currentPanel != optionsPanel)
            currentPanel.SetActive(false);

        optionsPanel.SetActive(true);
        currentPanel = optionsPanel;
    }

    public void OnBackFromOptions()
    {
        if (optionsCanvas) optionsCanvas.enabled = false;
        if (mainCanvas) mainCanvas.enabled = true;

        // reset Options state for next time
        ShowOnly(optionsPanel);
        currentPanel = optionsPanel;
    }

    // ---------- Helper methods ----------
    private void SwitchToSubPanel(GameObject target)
    {
        if (!target) return;
        ShowOnly(target);
        currentPanel = target;

        // make sure Options canvas is on when entering subpanels
        if (optionsCanvas && !optionsCanvas.enabled) optionsCanvas.enabled = true;
        if (mainCanvas && mainCanvas.enabled) mainCanvas.enabled = false;
    }

    public void OnPillarDistanceChanged(UnityEngine.UI.Slider pillarDistanceSlider)
    {
        float truncated = Mathf.Floor(pillarDistanceSlider.value * 10f) / 10f;
        GameSettings.Instance.PillarDistance = truncated;
        //public TMP_Text PillarDistanceValueText;

        this.PillarDistanceValueText.text = truncated.ToString();
    }

    public void OnPillarDiameterChanged(UnityEngine.UI.Slider pillarDiameterSlider)
    {

        GameSettings.Instance.PillarDiameter = pillarDiameterSlider.value;

        this.PillarDiameterValueText.text = pillarDiameterSlider.value.ToString();
    }

    public void OnPathwayDistanceChanged(UnityEngine.UI.Slider pathwayDistanceSlider)
    {
        GameSettings.Instance.PathwayDistance = pathwayDistanceSlider.value;

        this.PathwayDistanceValueText.text = pathwayDistanceSlider.value.ToString();
    }

    private void ShowOnly(GameObject panelToShow)
    {
        // hide all option panels
        optionsPanel.SetActive(false);
        pillarSettingsPanel.SetActive(false);
        obstacleSettingsPanel.SetActive(false);
        soundSettingsPanel.SetActive(false);

        // show target
        panelToShow.SetActive(true);
    }

    // ---------- Sound Slider methods ----------
    public void ToggleMute(bool isMute)
    {
        AudioManager.Instance.Mute = isMute;
    }

    public void GetSliderValueNormalized(UnityEngine.UI.Slider volumeSlider)
    {
        AudioManager.Instance.Volume = (volumeSlider.value / 100f);
        SoundValueText.text = volumeSlider.value.ToString();
    }
}
