
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using static UserSettingsXML;

public class UserSettingsLoader : MonoBehaviour
{
    private const string LogHeader = "[USER_SETTINGS]";

    public struct UserSettingsParameter<T>
    {
        public T Value;
        public bool Loaded;

        public override string ToString()
        {
            return "[" + (Loaded ? "Loaded" : "Not loaded") + "] " + Value.ToString();
        }
    }

    public class UserSettings
    {
        public UserSettingsParameter<bool> Keyboard_EnableShortcuts;

        public UserSettingsParameter<bool> DataCollection_AutoStart;
        public UserSettingsParameter<string> DataCollection_OutputPath;

        public UserSettingsParameter<int> VR_SamplingRate;
        public UserSettingsParameter<int> VR_SamplingBufferSize;

        public UserSettingsParameter<bool> Shimmer_Enabled;
        public UserSettingsParameter<string> Shimmer_DeviceName;
        public UserSettingsParameter<int> Shimmer_HeartbeatsToAverage;
        public UserSettingsParameter<int> Shimmer_TrainingPeriodPPG;
        public UserSettingsParameter<double> Shimmer_SamplingRate;
        public UserSettingsParameter<int> Shimmer_SamplingBufferSize;
        public UserSettingsParameter<bool> Shimmer_EnableAccelerator;
        public UserSettingsParameter<bool> Shimmer_EnableGSR;
        public UserSettingsParameter<bool> Shimmer_EnablePPG;

        public UserSettingsParameter<bool> EyeTracking_Enabled;
        public UserSettingsParameter<int> EyeTracking_SamplingRate;
        public UserSettingsParameter<int> EyeTracking_SamplingBufferSize;

        public UserSettingsParameter<bool> FaceTracking_Enabled;
        public UserSettingsParameter<int> FaceTracking_SamplingRate;
        public UserSettingsParameter<int> FaceTracking_SamplingBufferSize;

        public UserSettingsParameter<bool> Feedback_Enabled;
        public UserSettingsParameter<bool> Feedback_AfterScenario;
        public UserSettingsParameter<bool> Feedback_AfterLevel;

        public override string ToString()
        {
            return "Keyboard_EnableShortcuts=" + Keyboard_EnableShortcuts + "\n" +
            "DataCollection_AutoStart=" + DataCollection_AutoStart + "\n" +
            "DataCollection_OutputPath=" + DataCollection_OutputPath + "\n" +
            "VR_SamplingRate=" + VR_SamplingRate + "\n" +
            "VR_SamplingBufferSize=" + VR_SamplingBufferSize + "\n" +
            "Shimmer_Enabled=" + Shimmer_Enabled + "\n" +
            "Shimmer_DeviceName=" + Shimmer_DeviceName + "\n" +
            "Shimmer_HeartbeatsToAverage=" + Shimmer_HeartbeatsToAverage + "\n" +
            "Shimmer_TrainingPeriodPPG=" + Shimmer_TrainingPeriodPPG + "\n" +
            "Shimmer_SamplingRate=" + Shimmer_SamplingRate + "\n" +
            "Shimmer_SamplingBufferSize=" + Shimmer_SamplingBufferSize + "\n" +
            "Shimmer_EnableAccelerator=" + Shimmer_EnableAccelerator + "\n" +
            "Shimmer_EnableGSR=" + Shimmer_EnableGSR + "\n" +
            "Shimmer_EnablePPG=" + Shimmer_EnablePPG + "\n" +
            "EyeTracking_Enabled=" + EyeTracking_Enabled + "\n" +
            "EyeTracking_SamplingRate=" + EyeTracking_SamplingRate + "\n" +
            "EyeTracking_SamplingBufferSize=" + EyeTracking_SamplingBufferSize + "\n" +
            "FaceTracking_Enabled=" + FaceTracking_Enabled + "\n" +
            "FaceTracking_SamplingRate=" + FaceTracking_SamplingRate + "\n" +
            "FaceTracking_SamplingBufferSize=" + FaceTracking_SamplingBufferSize + "\n" +
            "Feedback_Enabled=" + Feedback_Enabled + "\n" +
            "Feedback_AfterScenario=" + Feedback_AfterScenario + "\n" +
            "Feedback_AfterLevel=" + Feedback_AfterLevel + "\n";
        }
    }

    public static readonly UserSettings userSettings = new();

    public static bool loaded;
    public static bool parseOK;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnBeforeSceneLoad()
    {
        loaded = false;
        parseOK = true;

        // Load file
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        XDocument xml;
        try
        { 
            xml = XDocument.Load(path);
        }
        catch (Exception e)
        {
            Debug.LogError(LogHeader + " File " + fileName + " not found in path " + Application.streamingAssetsPath);
            Debug.LogException(e);
            return;
        }

        // Check if empty
        var settings = xml.Descendants(XMLElements.Settings);
        if (settings.Count() == 0)
        {
            Debug.LogError(LogHeader + " UserSettings is empty");
            return;
        }

        // Keyboard
        XElement keyboard = GetElement(settings, XMLElements.Keyboard.Name);
        ParseValue(keyboard, XMLElements.Keyboard.Name, XMLElements.Keyboard.EnableShortcuts, out userSettings.Keyboard_EnableShortcuts);

        // Data Collection
        XElement dataCollection = GetElement(settings, XMLElements.DataCollection.Name);
        ParseValue(dataCollection, XMLElements.DataCollection.Name, XMLElements.DataCollection.AutoStart, out userSettings.DataCollection_AutoStart);
        ParseValue(dataCollection, XMLElements.DataCollection.Name, XMLElements.DataCollection.OutputPath, out userSettings.DataCollection_OutputPath);

        // VR
        XElement vr = GetElement(settings, XMLElements.VR.Name);
        var vrConfig = vr.Element(XMLElements.VR.Config.Name);
        ParseValue(vrConfig, XMLElements.VR.Name + "/" + XMLElements.VR.Config.Name, XMLElements.VR.Config.SamplingRate, out userSettings.VR_SamplingRate);
        ParseValue(vrConfig, XMLElements.VR.Name + "/" + XMLElements.VR.Config.Name, XMLElements.VR.Config.SamplingBufferSize, out userSettings.VR_SamplingBufferSize);

        // Shimmer
        XElement shimmer = GetElement(settings, XMLElements.Shimmer.Name);
        ParseValue(shimmer, XMLElements.Shimmer.Name, XMLElements.Shimmer.Enabled, out userSettings.Shimmer_Enabled);
        ParseValue(shimmer, XMLElements.Shimmer.Name, XMLElements.Shimmer.DeviceName, out userSettings.Shimmer_DeviceName);

        var shimmerConfig = shimmer.Element(XMLElements.Shimmer.Config.Name);
        ParseValue(shimmerConfig, XMLElements.Shimmer.Name + "/" + XMLElements.Shimmer.Config.Name, XMLElements.Shimmer.Config.HeartbeatsToAverage, out userSettings.Shimmer_HeartbeatsToAverage);
        ParseValue(shimmerConfig, XMLElements.Shimmer.Name + "/" + XMLElements.Shimmer.Config.Name, XMLElements.Shimmer.Config.TrainingPeriodPPG, out userSettings.Shimmer_TrainingPeriodPPG);
        ParseValue(shimmerConfig, XMLElements.Shimmer.Name + "/" + XMLElements.Shimmer.Config.Name, XMLElements.Shimmer.Config.SamplingRate, out userSettings.Shimmer_SamplingRate);
        ParseValue(shimmerConfig, XMLElements.Shimmer.Name + "/" + XMLElements.Shimmer.Config.Name, XMLElements.Shimmer.Config.SamplingBufferSize, out userSettings.Shimmer_SamplingBufferSize);
       
        var shimmerSensors = shimmer.Element(XMLElements.Shimmer.Sensors.Name);
        ParseValue(shimmerSensors, XMLElements.Shimmer.Name + "/" + XMLElements.Shimmer.Sensors.Name, XMLElements.Shimmer.Sensors.EnableAccelerator, out userSettings.Shimmer_EnableAccelerator);
        ParseValue(shimmerSensors, XMLElements.Shimmer.Name + "/" + XMLElements.Shimmer.Sensors.Name, XMLElements.Shimmer.Sensors.EnableGSR, out userSettings.Shimmer_EnableGSR);
        ParseValue(shimmerSensors, XMLElements.Shimmer.Name + "/" + XMLElements.Shimmer.Sensors.Name, XMLElements.Shimmer.Sensors.EnablePPG, out userSettings.Shimmer_EnablePPG);

        // Eye Tracking
        XElement eyeTracking = GetElement(settings, XMLElements.EyeTracking.Name);
        ParseValue(eyeTracking, XMLElements.EyeTracking.Name, XMLElements.EyeTracking.Enabled, out userSettings.EyeTracking_Enabled);
        var eyeConfig = eyeTracking.Element(XMLElements.EyeTracking.Config.Name);
        ParseValue(eyeConfig, XMLElements.EyeTracking.Name + "/" + XMLElements.EyeTracking.Config.Name, XMLElements.EyeTracking.Config.SamplingRate, out userSettings.EyeTracking_SamplingRate);
        ParseValue(eyeConfig, XMLElements.EyeTracking.Name + "/" + XMLElements.EyeTracking.Config.Name, XMLElements.EyeTracking.Config.SamplingBufferSize, out userSettings.EyeTracking_SamplingBufferSize);

        // Face Tracking
        XElement facetracking = GetElement(settings, XMLElements.FaceTracking.Name);
        ParseValue(facetracking, XMLElements.FaceTracking.Name, XMLElements.FaceTracking.Enabled, out userSettings.FaceTracking_Enabled);
        var faceConfig = facetracking.Element(XMLElements.FaceTracking.Config.Name);
        ParseValue(faceConfig, XMLElements.FaceTracking.Name + "/" + XMLElements.FaceTracking.Config.Name, XMLElements.FaceTracking.Config.SamplingRate, out userSettings.FaceTracking_SamplingRate);
        ParseValue(faceConfig, XMLElements.FaceTracking.Name + "/" + XMLElements.FaceTracking.Config.Name, XMLElements.FaceTracking.Config.SamplingBufferSize, out userSettings.FaceTracking_SamplingBufferSize);

        // Feedback
        XElement feedback = GetElement(settings, XMLElements.Feedback.Name);
        ParseValue(feedback, XMLElements.Feedback.Name, XMLElements.Feedback.Enabled, out userSettings.Feedback_Enabled);
        ParseValue(feedback, XMLElements.Feedback.Name, XMLElements.Feedback.AfterScenario, out userSettings.Feedback_AfterScenario);
        ParseValue(feedback, XMLElements.Feedback.Name, XMLElements.Feedback.AfterLevel, out userSettings.Feedback_AfterLevel);

        loaded = true;
        Debug.Log(LogHeader + " UserSettings loaded" + (parseOK ? " successfully" : " with errors") + ": \n" + userSettings);
    }

    private static XElement GetElement(IEnumerable<XElement> settings, string elementName)
    {
        var dataCollection = settings.Descendants(elementName);
        if (dataCollection.Count() == 1) return dataCollection.ElementAt(0);
        else if (dataCollection.Count() == 0)
        {
            Debug.LogError(LogHeader + " Unable to find element " + elementName);
            return null;
        }
        else
        {
            Debug.LogError(LogHeader + " Multiple definitions of " + elementName);
            return null;
        }
    }

    private static void ParseValue<T>(XElement element, string elementName, string attributeName, out UserSettingsParameter<T> output)
    {
        if (element == null)
        {
            PrintParseError<T>(elementName, attributeName, element);
            output.Value = default;
            output.Loaded = false;
            parseOK = false;
        }
        else
        {
            try
            {
                output.Value = Convert<T>(element.Attribute(attributeName).Value);
                output.Loaded = true;
                parseOK &= true;
            }
            catch (Exception)
            {
                PrintParseError<T>(elementName, attributeName, element);
                output.Value = default;
                output.Loaded = false;
                parseOK = false;
            }
        }
        
    }

    private static T Convert<T>(string input)
    {
        var converter = TypeDescriptor.GetConverter(typeof(T));
        if (converter != null)
            return (T)converter.ConvertFromString(input);
        throw new NullReferenceException();
    }

    private static void PrintParseError<T>(string elementName, string attributeName, XElement element)
    {
        if (element == null)
            Debug.LogError(LogHeader + " Element not found <" + elementName + "/" + attributeName + ">");
        else
        {
            var attribute = element.Attribute(attributeName);
            Debug.LogError(LogHeader + " Error parsing <" + elementName + "/" + attributeName + "> [found '" + (attribute == null ? "Not found" : attribute.Value) + "', expected " + typeof(T).Name + "]");
        }
    }

    public static void Load<T>(UserSettingsParameter<T> parameter, out T output, T defaultValue)
    {
        output = parameter.Loaded ? parameter.Value : defaultValue;
    }
}
