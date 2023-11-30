using System.Collections.Generic;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using Valve.VR;

[RequireComponent(typeof(SteamVR_LoadLevel))]
public class SceneController : MonoBehaviour {
    [SerializeField] private List<SceneReference> scenes;

    private SteamVR_LoadLevel _loadLevel;
    private bool _isLoading;

    private void Awake() {
        _loadLevel = GetComponent<SteamVR_LoadLevel>();
    }

    public void QuitToDesktop() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetLocale(Locale locale) {
        LocalizationSettings.Instance.SetSelectedLocale(locale);
    }

    public void LoadSceneAsync(SceneReference scene) {
        LoadSceneAsync(scene.BuildIndex);
    }

    public void LoadSceneAsync(int index) {
        if (index >= scenes.Count || index < 0) return;
        SceneReference s = scenes[index];
        if (!s.IsSafeToUse) return;
        _loadLevel.levelName = scenes[index].Name;
        _isLoading = true;
        _loadLevel.Trigger();
    }

    private void Update() {
        if (_isLoading) return;
        const int key = (int)KeyCode.Keypad0;
        for (int i = 0; i < scenes.Count; i++) {
            if (SceneManager.GetActiveScene().buildIndex == i) continue;
            if (Input.GetKeyUp((KeyCode)key + i))
                LoadSceneAsync(scenes[i]);
        }

        if (Input.GetKeyUp(KeyCode.Escape)) {
            Application.Quit();
        }
    }
}