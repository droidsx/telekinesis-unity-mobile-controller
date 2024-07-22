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
