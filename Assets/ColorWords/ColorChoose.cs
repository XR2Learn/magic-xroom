using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using DataCollection.Data;
using DataCollection.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Serialization;
using Valve.VR.InteractionSystem;
using static DataCollection.Data.UserEvents;
using Random = UnityEngine.Random;

[Serializable]
public class ColorEntry
{
    [SerializeField] private Color color;
    [SerializeField] private LocalizedString localizedString;

    public Color Color => color;
    public LocalizedString LocalizedString => localizedString;
}

public class ColorChoose : ProgressableGame
{
    [SerializeField] private List<MeshRenderer> meshRenderers;
    [SerializeField] private List<Interactable> interactables;
    [SerializeField] private TextMeshProUGUI colorText;
    [SerializeField] private LocalizeStringEvent timerText;
    [SerializeField] private List<ColorEntry> colorMap;
    [SerializeField] public int startingTimeLimit;
    [SerializeField] public int winStreakTimePenalty;
    [SerializeField] public int maxWins;
    [SerializeField] public GameObject tableHeight;

    private MeshRenderer _chosenCube;
    private int _winCount;
    private int _currentlyActive;
    private List<Vector3> _initPosition = new List<Vector3>();
    private List<Quaternion> _initRotation = new List<Quaternion>();
    private Color _initColor;
    private LocalizeStringEvent _colorLocalized;

    private double _startTime;
    private double _timeLimit;

    // Start is called before the first frame update
    private void Awake()
    {
        _colorLocalized = colorText.GetComponent<LocalizeStringEvent>();

        //Save all positions that later will be resetted
        foreach (MeshRenderer t in meshRenderers)
        {
            _initPosition.Add(t.transform.position);
            _initRotation.Add(t.transform.rotation);
        }

        _initColor = meshRenderers.First().material.color;
        enabled = false;
    }

    public void StartGame()
    {
        if (enabled) return;

        scenarioStarted?.Invoke("color_words");
        foreach (Interactable interactable in interactables)
        {
            interactable.onAttachedToHand += Attach;
            // interactable.onDetachedFromHand += Detach;
        }
        SetupGame();
        enabled = true;
        _startTime = Time.time;
        _timeLimit = startingTimeLimit;
        levelStarted?.Invoke(_currentlyActive);
        StartCoroutine(TimerCoroutine());
    }
    
    public void StopGame()
    {
        scenarioEnded?.Invoke("color_words");
        ResetCubePositions();
        
        foreach (Interactable interactable in interactables)
        {
            interactable.onAttachedToHand -= Attach;
            // interactable.onDetachedFromHand -= Detach;
        }
        foreach (MeshRenderer meshRenderer in meshRenderers)
            meshRenderer.material.color = _initColor;
        
        // StopCoroutine(TimerCoroutine());
        // UpdateTimer(0f);
        
        enabled = false;
        FeedbackBoard.Instance.Show(FeedbackBoard.FeedbackEventType.AFTER_SCENARIO);
    }

    private void Attach(Hand hand)
    {
        if (IsCubeColorCorrect(hand.currentAttachedObject))
        {
            //Check if we reach the maximum wins to pass to the next level
            if (_winCount % maxWins == 0)
            {
                if (_currentlyActive < meshRenderers.Count)
                {
                    //Activate next cube
                    meshRenderers[_currentlyActive].enabled = true;
                    _currentlyActive += 1;
                }
                else
                {
                    _timeLimit -= winStreakTimePenalty;
                }
            }

            RandomizeCubeColors();

            //Choose new winning cube
            _chosenCube = meshRenderers[Random.Range(0, _currentlyActive)];
            _colorLocalized.StringReference = colorMap.Find(c => c.Color == _chosenCube.material.color).LocalizedString;
            //Randomize next text color
            colorText.color = meshRenderers[Random.Range(0, _currentlyActive)].material.color;
            _winCount += 1;
            _startTime = Time.time;
            levelCompleted?.Invoke(_currentlyActive);
        }
        else
        {
            StopGame();
            levelFailed?.Invoke(_currentlyActive);
        }
    }

    private void Detach(Hand hand)
    {
        ResetCubePositions();
    }

    private bool IsCubeColorCorrect(GameObject attached)
    {
        return attached.GetComponent<MeshRenderer>().material.color == _chosenCube.material.color;
    }

    private void ResetCubePositions()
    {
        //Reset positions
        for (int i = 0; i < meshRenderers.Count; i++)
        {
            // meshRenderers[i].gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // meshRenderers[i].gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            _initPosition[i] = new Vector3(_initPosition[i].x, tableHeight.transform.position.y, _initPosition[i].z);
            meshRenderers[i].gameObject.transform.position = _initPosition[i];
            meshRenderers[i].gameObject.transform.rotation = _initRotation[i];
        }
    }

    private void RandomizeCubeColors()
    {
        //Randomize all color cubes
        foreach (MeshRenderer t in meshRenderers.Where(t => t.enabled))
            t.material.color = colorMap[Random.Range(0, colorMap.Count)].Color;
    }

    private void SetupGame()
    {
        //Choose a random color for the cubes
        foreach (MeshRenderer t in meshRenderers)
        {
            t.material.color = colorMap[Random.Range(0, colorMap.Count)].Color;
        }

        //disable all meshes avoiding the first 2
        for (int i = 2; i < meshRenderers.Count; i++)
        {
            meshRenderers[i].enabled = false;
        }

        //Choose from the currently active cubes the right one
        _currentlyActive = 2;
        _chosenCube = meshRenderers[Random.Range(0, _currentlyActive)];
        _colorLocalized.StringReference = colorMap.Find(c => c.Color == _chosenCube.material.color).LocalizedString;
        //Choose a random text color between active cube colors
        colorText.color = meshRenderers[Random.Range(0, _currentlyActive)].material.color;
    }

    private IEnumerator TimerCoroutine()
    {
        double elapsed;
        while ((elapsed = Time.time - _startTime) < _timeLimit && enabled)
        {
            UpdateTimer(_timeLimit - elapsed);
            yield return new WaitForSeconds(0.1f);
        }

        UpdateTimer(0f);

        if (enabled)
            StopGame();
    }

    private void UpdateTimer(double value)
    {
        DoubleVariable time = null;
        //Show the remaining time
        if (!timerText.StringReference.TryGetValue("time", out IVariable timeVar))
        {
            time = new DoubleVariable();
            timerText.StringReference.Add("time", time);
        }
        else
        {
            time = timeVar as DoubleVariable;
        }

        time!.Value = value;
    }

    private void Update()
    {
        ResetCubePositions();
    }
}