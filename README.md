## Hand Client

Outgoing stream:

```javascript
{
    syncId: ... // sequence order each synced message
    timestamp: datetimeUTC // allows us to calculate delta
    root: Vector3D
    thumbBase: Vector3D
    thumbMid: Vector3D
    thumbTip: Vector3D
    ...
}
```

Incoming stream:

```javascript
{
  terminate: bool
}
```
