using System.Collections;
using UnityEngine;
using NativeWebSocket;
using UnityEngine.UI;

// https://github.com/droidsx/osmosis-unity-client
// https://github.com/endel/NativeWebSocket

public class WebSocketConnection : MonoBehaviour
{
    public TMPro.TextMeshProUGUI TextInfo;

    string DefaultWebSocketURL = "wss://droid-osmosis-1.onrender.com/";    //  Previous: "wss://droid-osmosis.onrender.com/"; 
 

    public static WebSocketConnection Connection;

    WebSocket websocket;

    public HandPositionalInformation HandPositionalInformation;

    string WebSocketURL;

    bool StopSendingData = false;

    private void Awake()
    {
        Connection = this;

        HandPositionalInformation.clientId = SystemInfo.deviceUniqueIdentifier;

        Debug.Log(JsonUtility.ToJson(HandPositionalInformation));
        WebSocketURL = DefaultWebSocketURL;
        SetWebSocketURL(WebSocketURL);
    }

    async void Start()
    {
        websocket.OnOpen += () =>
        {
            Debug.Log("Connection Open! Sending Data.");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
            websocket = new WebSocket(WebSocketURL);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection Closed. Trying to Reconnect.");
            StartCoroutine(Reconnect());
        };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            OnMessage(message);
        };

        // Keep sending messages at every 0.01s
        InvokeRepeating("SendWebSocketMessage", 0.0f, 0.01f);

        await websocket.Connect();
    }

    void SetWebSocketURL(string _WebSocketURL)
    {
        if (string.IsNullOrEmpty(_WebSocketURL))
        {
            WebSocketURL = DefaultWebSocketURL;
        }
        else
        {
            WebSocketURL = _WebSocketURL;
        }
        Debug.Log("SetWebSocketURL: " + WebSocketURL);
        websocket = new WebSocket($"{WebSocketURL}");
    }

    public async void UpdateWebSocketURL(TMPro.TMP_InputField _InputField)
    {
        string _WebSocketURL = _InputField.text;

        if (string.IsNullOrEmpty(_WebSocketURL))
        {
            WebSocketURL = DefaultWebSocketURL;
        }
        else
        {
            WebSocketURL = _WebSocketURL;
        }
        Debug.Log("Updated WebSocketURL (URL): " + $"{WebSocketURL}");

        CancelInvoke("SendWebSocketMessage");

        await websocket.Close();

        websocket = new WebSocket($"{WebSocketURL}");

        Start();
    }

    public void StartStreaming()
    {
        StopSendingData = false;
    }

    public void PauseStreaming()
    {
        StopSendingData = true;
    }

    IEnumerator Reconnect()
    {
        yield return new WaitForSeconds(0.3f);
        Start();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    async void SendWebSocketMessage()
    {
        if (StopSendingData)
            return;

        if (websocket.State == WebSocketState.Open)
        {
            string PoseInfoToJson = JsonUtility.ToJson(HandPositionalInformation);

            await websocket.SendText(PoseInfoToJson);

            if (TextInfo)
            {
                TextInfo.text = $"Palm.\nLeft  X: {HandPositionalInformation.LeftHand.palm.x.ToString("F2")}" +
                    $"  Y: {HandPositionalInformation.LeftHand.palm.y.ToString("F2")}" +
                    $"  Z: {HandPositionalInformation.LeftHand.palm.z.ToString("F2")}" +
                    $"\nRight X: {HandPositionalInformation.RightHand.palm.x.ToString("F2")}" +
                    $"  Y: {HandPositionalInformation.RightHand.palm.y.ToString("F2")}" +
                    $"  Z: {HandPositionalInformation.RightHand.palm.z.ToString("F2")}";
            }
            Debug.Log("Sending JSON: " + PoseInfoToJson);
        }
    }

    public async void SendJson(string Json)
    {
        if (websocket.State == WebSocketState.Open)
        {
            Debug.Log("Sending Json: " + Json);
            await websocket.SendText(Json);
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    public void OnMessage(string json)
    {
        //Debug.Log("Received Info: " + json);
    }

    public LightBuzz.HandTracking.WebcamSelector WebcamSelector;
    public LightBuzz.HandTracking.WebcamSource WebcamSource;
    public void SetHandInformation(bool _LeftHand, 
                                    Vector3 _wrist, 
                                    Vector3 _palm, 

                                    Vector3 _thumbTip,
                                    Vector3 _thumbDistal,
                                    Vector3 _thumbProximal,
                                    Vector3 _thumbMeta, 

                                    Vector3 _indexTip,
                                    Vector3 _indexDistal,
                                    Vector3 _indexProximal,
                                    Vector3 _indexMeta, 

                                    Vector3 _middleTip,
                                    Vector3 _middleDistal,
                                    Vector3 _middleProximal,
                                    Vector3 _middleMeta,

                                    Vector3 _ringTip,
                                    Vector3 _ringDistal,
                                    Vector3 _ringProximal,
                                    Vector3 _ringMeta,

                                    Vector3 _pinkyTip,
                                    Vector3 _pinkyDistal,
                                    Vector3 _pinkyProximal,
                                    Vector3 _pinkyMeta,
                                    
                                    float _BoundingBoxHeight
                                    )
    {

        float _Depth = _BoundingBoxHeight / WebcamSource.Height;

        float RemappedDepth = 0f;
        if (_Depth >= 0.25f)
        {
            RemappedDepth = ReMapValue(_Depth, 0.25f, 1.25f, 0.5f, 0.05f);  
        } else
        {
            RemappedDepth = ReMapValue(_Depth, 0.05f, 0.25f, 1f, 0.5f);
        }
        Debug.Log("_boundingBox Height: " + _BoundingBoxHeight + " _Depth: " + _Depth + "  RemappedDepth: " + RemappedDepth);

        _Depth = RemappedDepth;

        if (_LeftHand)
        {
            if (WebcamSelector.FrontCameraEnabled == false)
            {
                /*
                HandPositionalInformation.LeftHand.wrist = _wrist;
                HandPositionalInformation.LeftHand.palm = _palm;

                HandPositionalInformation.LeftHand.thumb.tip = _thumbTip;
                HandPositionalInformation.LeftHand.thumb.distal = _thumbDistal;
                HandPositionalInformation.LeftHand.thumb.proximal = _thumbProximal;
                HandPositionalInformation.LeftHand.thumb.meta = _thumbMeta;

                HandPositionalInformation.LeftHand.index.tip = _indexTip;
                HandPositionalInformation.LeftHand.index.distal = _indexDistal;
                HandPositionalInformation.LeftHand.index.proximal = _indexProximal;
                HandPositionalInformation.LeftHand.index.meta = _indexMeta;

                HandPositionalInformation.LeftHand.middle.tip = _middleTip;
                HandPositionalInformation.LeftHand.middle.distal = _middleDistal;
                HandPositionalInformation.LeftHand.middle.proximal = _middleProximal;
                HandPositionalInformation.LeftHand.middle.meta = _middleMeta;

                HandPositionalInformation.LeftHand.ring.tip = _ringTip;
                HandPositionalInformation.LeftHand.ring.distal = _ringDistal;
                HandPositionalInformation.LeftHand.ring.proximal = _ringProximal;
                HandPositionalInformation.LeftHand.ring.meta = _ringMeta;

                HandPositionalInformation.LeftHand.pinky.tip = _pinkyTip;
                HandPositionalInformation.LeftHand.pinky.distal = _pinkyDistal;
                HandPositionalInformation.LeftHand.pinky.proximal = _pinkyProximal;
                HandPositionalInformation.LeftHand.pinky.meta = _pinkyMeta;
                */

                /*
                HandPositionalInformation.LeftHand.wrist = new Vector3(_wrist.x, _wrist.y, _wrist.z + _Depth);
                HandPositionalInformation.LeftHand.palm = new Vector3(_palm.x, _palm.y, _palm.z + _Depth);

                HandPositionalInformation.LeftHand.thumb.tip = new Vector3(_thumbTip.x, _thumbTip.y, _thumbTip.z + _Depth);
                HandPositionalInformation.LeftHand.thumb.distal = new Vector3(_thumbDistal.x, _thumbDistal.y, _thumbDistal.z + _Depth);
                HandPositionalInformation.LeftHand.thumb.proximal = new Vector3(_thumbProximal.x, _thumbProximal.y, _thumbProximal.z + _Depth);
                HandPositionalInformation.LeftHand.thumb.meta = new Vector3(_thumbMeta.x, _thumbMeta.y, _thumbMeta.z + _Depth);

                HandPositionalInformation.LeftHand.index.tip = new Vector3(_indexTip.x, _indexTip.y, _indexTip.z + _Depth);
                HandPositionalInformation.LeftHand.index.distal = new Vector3(_indexDistal.x, _indexDistal.y, _indexDistal.z + _Depth);
                HandPositionalInformation.LeftHand.index.proximal = new Vector3(_indexProximal.x, _indexProximal.y, _indexProximal.z + _Depth);
                HandPositionalInformation.LeftHand.index.meta = new Vector3(_indexMeta.x, _indexMeta.y, _indexMeta.z + _Depth);

                HandPositionalInformation.LeftHand.middle.tip = new Vector3(_middleTip.x, _middleTip.y, _middleTip.z + _Depth);
                HandPositionalInformation.LeftHand.middle.distal = new Vector3(_middleDistal.x, _middleDistal.y, _middleDistal.z + _Depth);
                HandPositionalInformation.LeftHand.middle.proximal = new Vector3(_middleProximal.x, _middleProximal.y, _middleProximal.z + _Depth);
                HandPositionalInformation.LeftHand.middle.meta = new Vector3(_middleMeta.x, _middleMeta.y, _middleMeta.z + _Depth);

                HandPositionalInformation.LeftHand.ring.tip = new Vector3(_ringTip.x, _ringTip.y, _ringTip.z + _Depth);
                HandPositionalInformation.LeftHand.ring.distal = new Vector3(_ringDistal.x, _ringDistal.y, _ringDistal.z + _Depth);
                HandPositionalInformation.LeftHand.ring.proximal = new Vector3(_ringProximal.x, _ringProximal.y, _ringProximal.z + _Depth);
                HandPositionalInformation.LeftHand.ring.meta = new Vector3(_ringMeta.x, _ringMeta.y, _ringMeta.z + _Depth);

                HandPositionalInformation.LeftHand.pinky.tip = new Vector3(_pinkyTip.x, _pinkyTip.y, _pinkyTip.z + _Depth);
                HandPositionalInformation.LeftHand.pinky.distal = new Vector3(_pinkyDistal.x, _pinkyDistal.y, _pinkyDistal.z + _Depth);
                HandPositionalInformation.LeftHand.pinky.proximal = new Vector3(_pinkyProximal.x, _pinkyProximal.y, _pinkyProximal.z + _Depth);
                HandPositionalInformation.LeftHand.pinky.meta = new Vector3(_pinkyMeta.x, _pinkyMeta.y, _pinkyMeta.z + _Depth);
                */

                HandPositionalInformation.LeftHand.wrist = new Vector3(_wrist.y, _wrist.x, _wrist.z + _Depth);
                HandPositionalInformation.LeftHand.palm = new Vector3(_palm.y, _palm.x, _palm.z + _Depth);

                HandPositionalInformation.LeftHand.thumb.tip = new Vector3(_thumbTip.y, _thumbTip.x, _thumbTip.z + _Depth);
                HandPositionalInformation.LeftHand.thumb.distal = new Vector3(_thumbDistal.y, _thumbDistal.x, _thumbDistal.z + _Depth);
                HandPositionalInformation.LeftHand.thumb.proximal = new Vector3(_thumbProximal.y, _thumbProximal.x, _thumbProximal.z + _Depth);
                HandPositionalInformation.LeftHand.thumb.meta = new Vector3(_thumbMeta.y, _thumbMeta.x, _thumbMeta.z + _Depth);

                HandPositionalInformation.LeftHand.index.tip = new Vector3(_indexTip.y, _indexTip.x, _indexTip.z + _Depth);
                HandPositionalInformation.LeftHand.index.distal = new Vector3(_indexDistal.y, _indexDistal.x, _indexDistal.z + _Depth);
                HandPositionalInformation.LeftHand.index.proximal = new Vector3(_indexProximal.y, _indexProximal.x, _indexProximal.z + _Depth);
                HandPositionalInformation.LeftHand.index.meta = new Vector3(_indexMeta.y, _indexMeta.x, _indexMeta.z + _Depth);

                HandPositionalInformation.LeftHand.middle.tip = new Vector3(_middleTip.y, _middleTip.x, _middleTip.z + _Depth);
                HandPositionalInformation.LeftHand.middle.distal = new Vector3(_middleDistal.y, _middleDistal.x, _middleDistal.z + _Depth);
                HandPositionalInformation.LeftHand.middle.proximal = new Vector3(_middleProximal.y, _middleProximal.x, _middleProximal.z + _Depth);
                HandPositionalInformation.LeftHand.middle.meta = new Vector3(_middleMeta.y, _middleMeta.x, _middleMeta.z + _Depth);

                HandPositionalInformation.LeftHand.ring.tip = new Vector3(_ringTip.y, _ringTip.x, _ringTip.z + _Depth);
                HandPositionalInformation.LeftHand.ring.distal = new Vector3(_ringDistal.y, _ringDistal.x, _ringDistal.z + _Depth);
                HandPositionalInformation.LeftHand.ring.proximal = new Vector3(_ringProximal.y, _ringProximal.x, _ringProximal.z + _Depth);
                HandPositionalInformation.LeftHand.ring.meta = new Vector3(_ringMeta.y, _ringMeta.x, _ringMeta.z + _Depth);

                HandPositionalInformation.LeftHand.pinky.tip = new Vector3(_pinkyTip.y, _pinkyTip.x, _pinkyTip.z + _Depth);
                HandPositionalInformation.LeftHand.pinky.distal = new Vector3(_pinkyDistal.y, _pinkyDistal.x, _pinkyDistal.z + _Depth);
                HandPositionalInformation.LeftHand.pinky.proximal = new Vector3(_pinkyProximal.y, _pinkyProximal.x, _pinkyProximal.z + _Depth);
                HandPositionalInformation.LeftHand.pinky.meta = new Vector3(_pinkyMeta.y, _pinkyMeta.x, _pinkyMeta.z + _Depth);
            } else
            {
                HandPositionalInformation.LeftHand.wrist = new Vector3(_wrist.y, _wrist.x, _wrist.z + _Depth);
                HandPositionalInformation.LeftHand.palm = new Vector3(_palm.y, _palm.x, _palm.z + _Depth);

                HandPositionalInformation.LeftHand.thumb.tip = new Vector3(_thumbTip.y, _thumbTip.x, _thumbTip.z + _Depth);
                HandPositionalInformation.LeftHand.thumb.distal = new Vector3(_thumbDistal.y, _thumbDistal.x, _thumbDistal.z + _Depth);
                HandPositionalInformation.LeftHand.thumb.proximal = new Vector3(_thumbProximal.y, _thumbProximal.x, _thumbProximal.z + _Depth);
                HandPositionalInformation.LeftHand.thumb.meta = new Vector3(_thumbMeta.y, _thumbMeta.x, _thumbMeta.z + _Depth);

                HandPositionalInformation.LeftHand.index.tip = new Vector3(_indexTip.y, _indexTip.x, _indexTip.z + _Depth);
                HandPositionalInformation.LeftHand.index.distal = new Vector3(_indexDistal.y, _indexDistal.x, _indexDistal.z + _Depth);
                HandPositionalInformation.LeftHand.index.proximal = new Vector3(_indexProximal.y, _indexProximal.x, _indexProximal.z + _Depth);
                HandPositionalInformation.LeftHand.index.meta = new Vector3(_indexMeta.y, _indexMeta.x, _indexMeta.z + _Depth);

                HandPositionalInformation.LeftHand.middle.tip = new Vector3(_middleTip.y, _middleTip.x, _middleTip.z + _Depth);
                HandPositionalInformation.LeftHand.middle.distal = new Vector3(_middleDistal.y, _middleDistal.x, _middleDistal.z + _Depth);
                HandPositionalInformation.LeftHand.middle.proximal = new Vector3(_middleProximal.y, _middleProximal.x, _middleProximal.z + _Depth);
                HandPositionalInformation.LeftHand.middle.meta = new Vector3(_middleMeta.y, _middleMeta.x, _middleMeta.z + _Depth);

                HandPositionalInformation.LeftHand.ring.tip = new Vector3(_ringTip.y, _ringTip.x, _ringTip.z + _Depth);
                HandPositionalInformation.LeftHand.ring.distal = new Vector3(_ringDistal.y, _ringDistal.x, _ringDistal.z + _Depth);
                HandPositionalInformation.LeftHand.ring.proximal = new Vector3(_ringProximal.y, _ringProximal.x, _ringProximal.z + _Depth);
                HandPositionalInformation.LeftHand.ring.meta = new Vector3(_ringMeta.y, _ringMeta.x, _ringMeta.z + _Depth);

                HandPositionalInformation.LeftHand.pinky.tip = new Vector3(_pinkyTip.y, _pinkyTip.x, _pinkyTip.z + _Depth);
                HandPositionalInformation.LeftHand.pinky.distal = new Vector3(_pinkyDistal.y, _pinkyDistal.x, _pinkyDistal.z + _Depth);
                HandPositionalInformation.LeftHand.pinky.proximal = new Vector3(_pinkyProximal.y, _pinkyProximal.x, _pinkyProximal.z + _Depth);
                HandPositionalInformation.LeftHand.pinky.meta = new Vector3(_pinkyMeta.y, _pinkyMeta.x, _pinkyMeta.z + _Depth);
            }
        }
        else
        {
            if (WebcamSelector.FrontCameraEnabled == false)
            {
                /*
                HandPositionalInformation.RightHand.wrist = _wrist;
                HandPositionalInformation.RightHand.palm = _palm;

                HandPositionalInformation.RightHand.thumb.tip = _thumbTip;
                HandPositionalInformation.RightHand.thumb.distal = _thumbDistal;
                HandPositionalInformation.RightHand.thumb.proximal = _thumbProximal;
                HandPositionalInformation.RightHand.thumb.meta = _thumbMeta;

                HandPositionalInformation.RightHand.index.tip = _indexTip;
                HandPositionalInformation.RightHand.index.distal = _indexDistal;
                HandPositionalInformation.RightHand.index.proximal = _indexProximal;
                HandPositionalInformation.RightHand.index.meta = _indexMeta;

                HandPositionalInformation.RightHand.middle.tip = _middleTip;
                HandPositionalInformation.RightHand.middle.distal = _middleDistal;
                HandPositionalInformation.RightHand.middle.proximal = _middleProximal;
                HandPositionalInformation.RightHand.middle.meta = _middleMeta;

                HandPositionalInformation.RightHand.ring.tip = _ringTip;
                HandPositionalInformation.RightHand.ring.distal = _ringDistal;
                HandPositionalInformation.RightHand.ring.proximal = _ringProximal;
                HandPositionalInformation.RightHand.ring.meta = _ringMeta;

                HandPositionalInformation.RightHand.pinky.tip = _pinkyTip;
                HandPositionalInformation.RightHand.pinky.distal = _pinkyDistal;
                HandPositionalInformation.RightHand.pinky.proximal = _pinkyProximal;
                HandPositionalInformation.RightHand.pinky.meta = _pinkyMeta;
                */

                /*
                HandPositionalInformation.RightHand.wrist = new Vector3(_wrist.x, _wrist.y, _wrist.z + _Depth);
                HandPositionalInformation.RightHand.palm = new Vector3(_palm.x, _palm.y, _palm.z + _Depth);

                HandPositionalInformation.RightHand.thumb.tip = new Vector3(_thumbTip.x, _thumbTip.y, _thumbTip.z + _Depth);
                HandPositionalInformation.RightHand.thumb.distal = new Vector3(_thumbDistal.x, _thumbDistal.y, _thumbDistal.z + _Depth);
                HandPositionalInformation.RightHand.thumb.proximal = new Vector3(_thumbProximal.x, _thumbProximal.y, _thumbProximal.z + _Depth);
                HandPositionalInformation.RightHand.thumb.meta = new Vector3(_thumbMeta.x, _thumbMeta.y, _thumbMeta.z + _Depth);

                HandPositionalInformation.RightHand.index.tip = new Vector3(_indexTip.x, _indexTip.y, _indexTip.z + _Depth);
                HandPositionalInformation.RightHand.index.distal = new Vector3(_indexDistal.x, _indexDistal.y, _indexDistal.z + _Depth);
                HandPositionalInformation.RightHand.index.proximal = new Vector3(_indexProximal.x, _indexProximal.y, _indexProximal.z + _Depth);
                HandPositionalInformation.RightHand.index.meta = new Vector3(_indexMeta.x, _indexMeta.y, _indexMeta.z + _Depth);

                HandPositionalInformation.RightHand.middle.tip = new Vector3(_middleTip.x, _middleTip.y, _middleTip.z + _Depth);
                HandPositionalInformation.RightHand.middle.distal = new Vector3(_middleDistal.x, _middleDistal.y, _middleDistal.z + _Depth);
                HandPositionalInformation.RightHand.middle.proximal = new Vector3(_middleProximal.x, _middleProximal.y, _middleProximal.z + _Depth);
                HandPositionalInformation.RightHand.middle.meta = new Vector3(_middleMeta.x, _middleMeta.y, _middleMeta.z + _Depth);

                HandPositionalInformation.RightHand.ring.tip = new Vector3(_ringTip.x, _ringTip.y, _ringTip.z + _Depth);
                HandPositionalInformation.RightHand.ring.distal = new Vector3(_ringDistal.x, _ringDistal.y, _ringDistal.z + _Depth);
                HandPositionalInformation.RightHand.ring.proximal = new Vector3(_ringProximal.x, _ringProximal.y, _ringProximal.z + _Depth);
                HandPositionalInformation.RightHand.ring.meta = new Vector3(_ringMeta.x, _ringMeta.y, _ringMeta.z + _Depth);

                HandPositionalInformation.RightHand.pinky.tip = new Vector3(_pinkyTip.x, _pinkyTip.y, _pinkyTip.z + _Depth);
                HandPositionalInformation.RightHand.pinky.distal = new Vector3(_pinkyDistal.x, _pinkyDistal.y, _pinkyDistal.z + _Depth);
                HandPositionalInformation.RightHand.pinky.proximal = new Vector3(_pinkyProximal.x, _pinkyProximal.y, _pinkyProximal.z + _Depth);
                HandPositionalInformation.RightHand.pinky.meta = new Vector3(_pinkyMeta.x, _pinkyMeta.y, _pinkyMeta.z + _Depth);
                */
                HandPositionalInformation.RightHand.wrist = new Vector3(_wrist.y, _wrist.x, _wrist.z + _Depth);
                HandPositionalInformation.RightHand.palm = new Vector3(_palm.y, _palm.x, _palm.z + _Depth);

                HandPositionalInformation.RightHand.thumb.tip = new Vector3(_thumbTip.y, _thumbTip.x, _thumbTip.z + _Depth);
                HandPositionalInformation.RightHand.thumb.distal = new Vector3(_thumbDistal.y, _thumbDistal.x, _thumbDistal.z + _Depth);
                HandPositionalInformation.RightHand.thumb.proximal = new Vector3(_thumbProximal.y, _thumbProximal.x, _thumbProximal.z + _Depth);
                HandPositionalInformation.RightHand.thumb.meta = new Vector3(_thumbMeta.y, _thumbMeta.x, _thumbMeta.z + _Depth);

                HandPositionalInformation.RightHand.index.tip = new Vector3(_indexTip.y, _indexTip.x, _indexTip.z + _Depth);
                HandPositionalInformation.RightHand.index.distal = new Vector3(_indexDistal.y, _indexDistal.x, _indexDistal.z + _Depth);
                HandPositionalInformation.RightHand.index.proximal = new Vector3(_indexProximal.y, _indexProximal.x, _indexProximal.z + _Depth);
                HandPositionalInformation.RightHand.index.meta = new Vector3(_indexMeta.y, _indexMeta.x, _indexMeta.z + _Depth);

                HandPositionalInformation.RightHand.middle.tip = new Vector3(_middleTip.y, _middleTip.x, _middleTip.z + _Depth);
                HandPositionalInformation.RightHand.middle.distal = new Vector3(_middleDistal.y, _middleDistal.x, _middleDistal.z + _Depth);
                HandPositionalInformation.RightHand.middle.proximal = new Vector3(_middleProximal.y, _middleProximal.x, _middleProximal.z + _Depth);
                HandPositionalInformation.RightHand.middle.meta = new Vector3(_middleMeta.y, _middleMeta.x, _middleMeta.z + _Depth);

                HandPositionalInformation.RightHand.ring.tip = new Vector3(_ringTip.y, _ringTip.x, _ringTip.z + _Depth);
                HandPositionalInformation.RightHand.ring.distal = new Vector3(_ringDistal.y, _ringDistal.x, _ringDistal.z + _Depth);
                HandPositionalInformation.RightHand.ring.proximal = new Vector3(_ringProximal.y, _ringProximal.x, _ringProximal.z + _Depth);
                HandPositionalInformation.RightHand.ring.meta = new Vector3(_ringMeta.y, _ringMeta.x, _ringMeta.z + _Depth);

                HandPositionalInformation.RightHand.pinky.tip = new Vector3(_pinkyTip.y, _pinkyTip.x, _pinkyTip.z + _Depth);
                HandPositionalInformation.RightHand.pinky.distal = new Vector3(_pinkyDistal.y, _pinkyDistal.x, _pinkyDistal.z + _Depth);
                HandPositionalInformation.RightHand.pinky.proximal = new Vector3(_pinkyProximal.y, _pinkyProximal.x, _pinkyProximal.z + _Depth);
                HandPositionalInformation.RightHand.pinky.meta = new Vector3(_pinkyMeta.y, _pinkyMeta.x, _pinkyMeta.z + _Depth);
            }
            else
            {
                HandPositionalInformation.RightHand.wrist = new Vector3(_wrist.y, _wrist.x, _wrist.z + _Depth);
                HandPositionalInformation.RightHand.palm = new Vector3(_palm.y, _palm.x, _palm.z + _Depth);

                HandPositionalInformation.RightHand.thumb.tip = new Vector3(_thumbTip.y, _thumbTip.x, _thumbTip.z + _Depth);
                HandPositionalInformation.RightHand.thumb.distal = new Vector3(_thumbDistal.y, _thumbDistal.x, _thumbDistal.z + _Depth);
                HandPositionalInformation.RightHand.thumb.proximal = new Vector3(_thumbProximal.y, _thumbProximal.x, _thumbProximal.z + _Depth);
                HandPositionalInformation.RightHand.thumb.meta = new Vector3(_thumbMeta.y, _thumbMeta.x, _thumbMeta.z + _Depth);

                HandPositionalInformation.RightHand.index.tip = new Vector3(_indexTip.y, _indexTip.x, _indexTip.z + _Depth);
                HandPositionalInformation.RightHand.index.distal = new Vector3(_indexDistal.y, _indexDistal.x, _indexDistal.z + _Depth);
                HandPositionalInformation.RightHand.index.proximal = new Vector3(_indexProximal.y, _indexProximal.x, _indexProximal.z + _Depth);
                HandPositionalInformation.RightHand.index.meta = new Vector3(_indexMeta.y, _indexMeta.x, _indexMeta.z + _Depth);

                HandPositionalInformation.RightHand.middle.tip = new Vector3(_middleTip.y, _middleTip.x, _middleTip.z + _Depth);
                HandPositionalInformation.RightHand.middle.distal = new Vector3(_middleDistal.y, _middleDistal.x, _middleDistal.z + _Depth);
                HandPositionalInformation.RightHand.middle.proximal = new Vector3(_middleProximal.y, _middleProximal.x, _middleProximal.z + _Depth);
                HandPositionalInformation.RightHand.middle.meta = new Vector3(_middleMeta.y, _middleMeta.x, _middleMeta.z + _Depth);

                HandPositionalInformation.RightHand.ring.tip = new Vector3(_ringTip.y, _ringTip.x, _ringTip.z + _Depth);
                HandPositionalInformation.RightHand.ring.distal = new Vector3(_ringDistal.y, _ringDistal.x, _ringDistal.z + _Depth);
                HandPositionalInformation.RightHand.ring.proximal = new Vector3(_ringProximal.y, _ringProximal.x, _ringProximal.z + _Depth);
                HandPositionalInformation.RightHand.ring.meta = new Vector3(_ringMeta.y, _ringMeta.x, _ringMeta.z + _Depth);

                HandPositionalInformation.RightHand.pinky.tip = new Vector3(_pinkyTip.y, _pinkyTip.x, _pinkyTip.z + _Depth);
                HandPositionalInformation.RightHand.pinky.distal = new Vector3(_pinkyDistal.y, _pinkyDistal.x, _pinkyDistal.z + _Depth);
                HandPositionalInformation.RightHand.pinky.proximal = new Vector3(_pinkyProximal.y, _pinkyProximal.x, _pinkyProximal.z + _Depth);
                HandPositionalInformation.RightHand.pinky.meta = new Vector3(_pinkyMeta.y, _pinkyMeta.x, _pinkyMeta.z + _Depth);
            }
        }
    }
    float ReMapValue(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

}

[System.Serializable]
public class HandPositionalInformation
{
    public string clientId;
    public float timestamp = 0.01f;
    public HandInfo LeftHand;
    public HandInfo RightHand;
}

[System.Serializable]
public class HandInfo
{
    public Vector3 wrist;
    public Vector3 palm;

    public HandPart thumb;
    public HandPart index;
    public HandPart middle;
    public HandPart ring;
    public HandPart pinky;
}

[System.Serializable]
public class HandPart
{
    public Vector3 tip;
    public Vector3 distal;
    public Vector3 proximal;
    public Vector3 meta;
}