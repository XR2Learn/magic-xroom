/*********** Copyright © 2024 University of Applied Sciences of Southern Switzerland (SUPSI) ***********\
 
 Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
 associated documentation files (the "Software"), to deal in the Software without restriction,
 including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 subject to the following conditions:

 The above copyright notice and this permission notice shall be included in all copies or substantial
 portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
 LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
 SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

\*******************************************************************************************************/


using XR2Learn.DataCollection.Data;
using System;
using UnityEngine;

namespace XR2Learn.DataCollection.Managers
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