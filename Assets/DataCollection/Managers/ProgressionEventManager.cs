using DataCollection.Data;
using System;
using UnityEngine;

namespace DataCollection.Managers
{
    public class ProgressionEventManager : MonoBehaviour
    { 
        public void OnTeleportIn(string scenario)
        {
            IOManager.Append(IOManager.Sensor.PROGRESS_EVENT, UserEvents.Teleport.TELEPORT_IN, scenario);
        }

        public void OnTeleportOut(string scenario)
        {
            IOManager.Append(IOManager.Sensor.PROGRESS_EVENT, UserEvents.Teleport.TELEPORT_OUT, scenario);
        }

        public void OnScenarioStarted(string scenario)
        {
            IOManager.Append(IOManager.Sensor.PROGRESS_EVENT, UserEvents.Scenario.SCENARIO_STARTED, scenario);
        }

        public void OnScenarioEnded(string scenario)
        {
            IOManager.Append(IOManager.Sensor.PROGRESS_EVENT, UserEvents.Scenario.SCENARIO_ENDED, scenario);
        }

        public void OnLevelStarted(int level)
        {
            IOManager.Append(IOManager.Sensor.PROGRESS_EVENT, UserEvents.Level.LEVEL_STARTED, level);
        }

        public void OnLevelFailed(int level)
        {
            IOManager.Append(IOManager.Sensor.PROGRESS_EVENT, UserEvents.Level.LEVEL_FAILED, level);
        }

        public void OnLevelCompleted(int level)
        {
            IOManager.Append(IOManager.Sensor.PROGRESS_EVENT, UserEvents.Level.LEVEL_COMPLETED, level);
        }

        public void OnFeedbackReceived(string feedback, long ms)
        {
            Enum.TryParse(feedback.ToUpper(), out UserEvents.Feedback f);
            IOManager.Append(IOManager.Sensor.PROGRESS_EVENT, f, ms);
        }
    }
}