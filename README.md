## Hand Client

You can connect to the sync server over websocket here: `URI = "wss://droid-osmosis.onrender.com"`

Suggested schema -- feel free to clarify or adjust as makes sense. Let's use JSON for now, we can change if performance warrants it.

```javascript
{
    clientId: ... // uuid to differentiate between clients
    timestamp: datetimeUTC // delta for time lapse between frames
    root: Vector3D // wrist position or root of hand
    // List of joint names e.g. use what makes sense
    thumbBase: Vector3D
    thumbMid: Vector3D
    thumbTip: Vector3D
    ...
}
```

Updated by EdgarasArt (2024.08.11)
1) Unity3D 2022.3.24f1 version was used for building Hands and Fingers tracking application.
- I suggest installing Unity Hub and select Unity3D version for installation from there. Any Unity3D 2022.3.x version, where x - any number should be good.
- After Unity3D installation select iOS module that is needed to export the app to Xcode and then accordingly from Xcode to iOS device or Testflight.

2) MainScene should be loaded once Unity3D project is opened (Located in: Assets/_AppContent/Scenes).
- File --> Build Settings --> Switch to iOS --> Build (it will build to Xcode).
- Go to File --> Build Settings --> PlayerSettings if you need to change bundle identifier or set build/version numbers.

3) Sending hands/fingers joints positional information over websocket handles WebSocketConnection.cs script.
If Inputfield on the screen is left empty we use:
string DefaultWebSocketURL = "wss://droid-osmosis-1.onrender.com/";
From the moment we enter something else, the new websocket URL is used.

4) HandManager.cs and HandVisual.cs is part of the used Lightbuzz plugin that is used within the scene (https://assetstore.unity.com/packages/tools/ai-ml-integration/hand-finger-tracking-ios-android-mac-windows-285126). The latter (HandVisual.cs) was modified in order to apply it to our needs.

5) Based on Front/Back camera selection from the dropdown menu has impact on the way X/Y positions are acquired inverted of the joints, therefore, to avoid this additional conditions are added for each hand.




**Classes for JSON structure:**
public class HandPositionalInformation
{
    public string clientId;
	public string client_type;
    public float timestamp;
    public HandInfo LeftHand;
    public HandInfo RightHand;
}

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

public class HandPart
{
    public Vector3 tip;
    public Vector3 distal;
    public Vector3 proximal;
    public Vector3 meta;
}


**JSON structure example that is sent over the websocket:**
{
   "clientId":"9dae853f1ce82e85ed2537a301f4e33db77564f8",
   "client_type":"ios",
   "timestamp":0.009999999776482582,
   "LeftHand":{
      "wrist":{
         "x":0.0,
         "y":0.0,
         "z":0.0
      },
      "palm":{
         "x":0.0,
         "y":0.0,
         "z":0.0
      },
      "thumb":{
         "tip":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "distal":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "proximal":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "meta":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         }
      },
      "index":{
         "tip":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "distal":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "proximal":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "meta":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         }
      },
      "middle":{
         "tip":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "distal":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "proximal":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "meta":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         }
      },
      "ring":{
         "tip":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "distal":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "proximal":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "meta":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         }
      },
      "pinky":{
         "tip":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "distal":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "proximal":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         },
         "meta":{
            "x":0.0,
            "y":0.0,
            "z":0.0
         }
      }
   },
   "RightHand":{
      "wrist":{
         "x":-0.178722083568573,
         "y":0.08712898194789887,
         "z":0.519607424736023
      },
      "palm":{
         "x":-0.1390758454799652,
         "y":0.09768427163362503,
         "z":0.5006153583526611
      },
      "thumb":{
         "tip":{
            "x":-0.13422943651676179,
            "y":0.1943676620721817,
            "z":0.4738638997077942
         },
         "distal":{
            "x":-0.14422856271266938,
            "y":0.17008697986602784,
            "z":0.47882115840911868
         },
         "proximal":{
            "x":-0.15618711709976197,
            "y":0.142720028758049,
            "z":0.49382543563842776
         },
         "meta":{
            "x":-0.16301028430461884,
            "y":0.11668473482131958,
            "z":0.5032618045806885
         }
      },
      "index":{
         "tip":{
            "x":-0.046283189207315448,
            "y":0.1577790081501007,
            "z":0.4288969039916992
         },
         "distal":{
            "x":-0.059992771595716479,
            "y":0.15218079090118409,
            "z":0.45856720209121706
         },
         "proximal":{
            "x":-0.07783875614404679,
            "y":0.1416662484407425,
            "z":0.4730418026447296
         },
         "meta":{
            "x":-0.10762736201286316,
            "y":0.12967774271965028,
            "z":0.47893714904785159
         }
      },
      "middle":{
         "tip":{
            "x":-0.026376307010650636,
            "y":0.1354440599679947,
            "z":0.4229782223701477
         },
         "distal":{
            "x":-0.0488700270652771,
            "y":0.12462647259235382,
            "z":0.4428856670856476
         },
         "proximal":{
            "x":-0.06509630382061005,
            "y":0.11465471982955933,
            "z":0.4665195047855377
         },
         "meta":{
            "x":-0.10129881650209427,
            "y":0.10684661567211151,
            "z":0.4800191819667816
         }
      },
      "ring":{
         "tip":{
            "x":-0.03728935867547989,
            "y":0.09357115626335144,
            "z":0.420134574174881
         },
         "distal":{
            "x":-0.055785536766052249,
            "y":0.09058047086000443,
            "z":0.4408968985080719
         },
         "proximal":{
            "x":-0.07592035830020905,
            "y":0.08860209584236145,
            "z":0.4625433087348938
         },
         "meta":{
            "x":-0.10061996430158615,
            "y":0.08519698679447174,
            "z":0.4765201508998871
         }
      },
      "pinky":{
         "tip":{
            "x":-0.07237471640110016,
            "y":0.050271909683942798,
            "z":0.4292919635772705
         },
         "distal":{
            "x":-0.0842115506529808,
            "y":0.05491797626018524,
            "z":0.4522441327571869
         },
         "proximal":{
            "x":-0.09720130264759064,
            "y":0.05975878983736038,
            "z":0.46707072854042055
         },
         "meta":{
            "x":-0.11515210568904877,
            "y":0.07175633311271668,
            "z":0.47214367985725405
         }
      }
   }
}

