using System.IO;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;

public class MovieClipGenerator : MonoBehaviour
{
    public string RecorderName = "Recorder";
    public string RecordingsPath = "Recordings";

    RecorderController _recorderController;

    void OnEnable()
    {
        var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        _recorderController = new RecorderController(controllerSettings);
        var animationOutputFolder = Path.Combine(Application.dataPath, RecordingsPath);
        var animationRecorder = ScriptableObject.CreateInstance<AnimationRecorderSettings>();
        animationRecorder.name = RecorderName;
        animationRecorder.Enabled = true;
        animationRecorder.AnimationInputSettings = new AnimationInputSettings
        {
            gameObject = gameObject,
            Recursive = true,
        };
        animationRecorder.AnimationInputSettings.AddComponentToRecord(typeof(Transform));
        animationRecorder.OutputFile = Path.Combine(animationOutputFolder, gameObject.name) + DefaultWildcard.GeneratePattern("GameObject") + "_v" + DefaultWildcard.Take;
        controllerSettings.AddRecorderSettings(animationRecorder);
        controllerSettings.SetRecordModeToManual();
        controllerSettings.FrameRate = 60.0f;
        RecorderOptions.VerboseMode = false;
        _recorderController.PrepareRecording();
        _recorderController.StartRecording();
    }

    void OnDisable()
    {
        _recorderController.StopRecording();
    }
}
