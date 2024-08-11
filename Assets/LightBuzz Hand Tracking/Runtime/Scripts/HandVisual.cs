using UnityEngine;

namespace LightBuzz.HandTracking
{
    public class HandVisual : MonoBehaviour
    {
        [Header("2D or 3D space")]
        [SerializeField] private bool _is2D = false;

        [ConditionalHide("_is2D", false)]
        [SerializeField] private ImageView _image;

        [Header("Colors")]
        [SerializeField] private Color32 _leftHandColor = new Color32(0, 153, 229, 200);
        [SerializeField] private Color32 _leftBoxColor = new Color32(0, 153, 229, 128);
        [SerializeField] private Color32 _rightHandColor = new Color32(0, 230, 110, 200);
        [SerializeField] private Color32 _rightBoxColor = new Color32(0, 230, 110, 128);

        [Header("Points")]
        [SerializeField] private GameObject[] _points;

        [Header("Lines")]
        [SerializeField] private LineRenderer _thumb;
        [SerializeField] private LineRenderer _index;
        [SerializeField] private LineRenderer _middle;
        [SerializeField] private LineRenderer _ring;
        [SerializeField] private LineRenderer _pinky;
        [SerializeField] private LineRenderer _mcpLine;
        [SerializeField] private LineRenderer _palmLine;

        [SerializeField] private LineRenderer _boundingBox;

        private Hand _hand;
        private Vector3 _offset = Vector3.zero;

        public Hand Hand
        {
            get => _hand;
            set => _hand = value;
        }

        public Vector3 Offset
        {
            get => _offset;
            set => _offset = value;
        }

        public HandSide Side { get; private set; }

        public bool Is2D
        {
            get => _is2D;
            set
            {
                _is2D = value;
                Recreate();
            }
        }

        public ImageView Image
        {
            get => _image;
            set => _image = value;
        }

        // ============== Added ==============
        void HideSpheres(bool Status)
        {
            int Amount = _points.Length;
            for (int i = 0; i < Amount; i++)
            {
                _points[i].gameObject.SetActive(!Status);
            }
        }
        // ==================================

        private void Recreate()
        {
            Color32 handColor = Side == HandSide.Left ? _leftHandColor : _rightHandColor;
            Color32 boxColor = Side == HandSide.Left ? _leftBoxColor : _rightBoxColor;

            _thumb.startColor = handColor;
            _thumb.endColor = handColor;
            _index.startColor = handColor;
            _index.endColor = handColor;
            _middle.startColor = handColor;
            _middle.endColor = handColor;
            _ring.startColor = handColor;
            _ring.endColor = handColor;
            _pinky.startColor = handColor;
            _pinky.endColor = handColor;
            _mcpLine.startColor = handColor;
            _mcpLine.endColor = handColor;
            _palmLine.startColor = handColor;
            _palmLine.endColor = handColor;

            _boundingBox.startColor = boxColor;
            _boundingBox.endColor = boxColor;

            if (_is2D) return;

            if (_thumb.startWidth != 0.01f)
            {
                _thumb.startWidth = 0.01f;
                _thumb.endWidth = 0.01f;
            }
            if (_index.startWidth != 0.01f)
            {
                _index.startWidth = 0.01f;
                _index.endWidth = 0.01f;
            }
            if (_middle.startWidth != 0.01f)
            {
                _middle.startWidth = 0.01f;
                _middle.endWidth = 0.01f;
            }
            if (_ring.startWidth != 0.01f)
            {
                _ring.startWidth = 0.01f;
                _ring.endWidth = 0.01f;
            }
            if (_pinky.startWidth != 0.01f)
            {
                _pinky.startWidth = 0.01f;
                _pinky.endWidth = 0.01f;
            }
            if (_mcpLine.startWidth != 0.01f)
            {
                _mcpLine.startWidth = 0.01f;
                _mcpLine.endWidth = 0.01f;
            }
            if (_palmLine.startWidth != 0.01f)
            {
                _palmLine.startWidth = 0.01f;
                _palmLine.endWidth = 0.01f;
            }
            if (_boundingBox.startWidth != 0.01f)
            {
                _boundingBox.startWidth = 0.01f;
                _boundingBox.endWidth = 0.01f;
            }
        }

        public void Load(Hand hand, Vector3 offset = default)
        {
            _hand = hand;
            _offset = offset;

            if (_hand == null || _hand.Confidence < 0.0f)
            {
                Toggle(false);
                return;
            }

            Side = _hand.Side;

            // ===== Added Condition =====
            if (_is2D == true)
            {
                Toggle(true);
               
            } else
            {
                Toggle(false);
                HideSpheres(true);
            }
            // ==========================

            Recreate();

            var root = _hand.FingerJoints[FingerJointType.Root];
            var thumbCmc = _hand.FingerJoints[FingerJointType.ThumbCMC];
            var thumbMcp = _hand.FingerJoints[FingerJointType.ThumbMCP];
            var thumbIp = _hand.FingerJoints[FingerJointType.ThumbIP];
            var thumbTip = _hand.FingerJoints[FingerJointType.ThumbTip];
            var indexMcp = _hand.FingerJoints[FingerJointType.IndexMCP];
            var indexPip = _hand.FingerJoints[FingerJointType.IndexPIP];
            var indexDip = _hand.FingerJoints[FingerJointType.IndexDIP];
            var indexTip = _hand.FingerJoints[FingerJointType.IndexTip];
            var middleMcp = _hand.FingerJoints[FingerJointType.MiddleMCP];
            var middlePip = _hand.FingerJoints[FingerJointType.MiddlePIP];
            var middleDip = _hand.FingerJoints[FingerJointType.MiddleDIP];
            var middleTip = _hand.FingerJoints[FingerJointType.MiddleTip];
            var ringMcp = _hand.FingerJoints[FingerJointType.RingMCP];
            var ringPip = _hand.FingerJoints[FingerJointType.RingPIP];
            var ringDip = _hand.FingerJoints[FingerJointType.RingDIP];
            var ringTip = _hand.FingerJoints[FingerJointType.RingTip];
            var pinkyMcp = _hand.FingerJoints[FingerJointType.PinkyMCP];
            var pinkyPip = _hand.FingerJoints[FingerJointType.PinkyPIP];
            var pinkyDip = _hand.FingerJoints[FingerJointType.PinkyDIP];
            var pinkyTip = _hand.FingerJoints[FingerJointType.PinkyTip];
            var palm = _hand.FingerJoints[FingerJointType.Palm];

            Vector3 pRoot = _is2D ? GetPosition2D(root.Position2D) : GetPosition3D(root.Position3D);
            Vector3 pThumbCmc = _is2D ? GetPosition2D(thumbCmc.Position2D) : GetPosition3D(thumbCmc.Position3D);
            Vector3 pThumbMcp = _is2D ? GetPosition2D(thumbMcp.Position2D) : GetPosition3D(thumbMcp.Position3D);
            Vector3 pThumbIp = _is2D ? GetPosition2D(thumbIp.Position2D) : GetPosition3D(thumbIp.Position3D);
            Vector3 pThumbTip = _is2D ? GetPosition2D(thumbTip.Position2D) : GetPosition3D(thumbTip.Position3D);
            Vector3 pIndexMcp = _is2D ? GetPosition2D(indexMcp.Position2D) : GetPosition3D(indexMcp.Position3D);
            Vector3 pIndexPip = _is2D ? GetPosition2D(indexPip.Position2D) : GetPosition3D(indexPip.Position3D);
            Vector3 pIndexDip = _is2D ? GetPosition2D(indexDip.Position2D) : GetPosition3D(indexDip.Position3D);
            Vector3 pIndexTip = _is2D ? GetPosition2D(indexTip.Position2D) : GetPosition3D(indexTip.Position3D);
            Vector3 pMiddleMcp = _is2D ? GetPosition2D(middleMcp.Position2D) : GetPosition3D(middleMcp.Position3D);
            Vector3 pMiddlePip = _is2D ? GetPosition2D(middlePip.Position2D) : GetPosition3D(middlePip.Position3D);
            Vector3 pMiddleDip = _is2D ? GetPosition2D(middleDip.Position2D) : GetPosition3D(middleDip.Position3D);
            Vector3 pMiddleTip = _is2D ? GetPosition2D(middleTip.Position2D) : GetPosition3D(middleTip.Position3D);
            Vector3 pRingMcp = _is2D ? GetPosition2D(ringMcp.Position2D) : GetPosition3D(ringMcp.Position3D);
            Vector3 pRingPip = _is2D ? GetPosition2D(ringPip.Position2D) : GetPosition3D(ringPip.Position3D);
            Vector3 pRingDip = _is2D ? GetPosition2D(ringDip.Position2D) : GetPosition3D(ringDip.Position3D);
            Vector3 pRingTip = _is2D ? GetPosition2D(ringTip.Position2D) : GetPosition3D(ringTip.Position3D);
            Vector3 pPinkyMcp = _is2D ? GetPosition2D(pinkyMcp.Position2D) : GetPosition3D(pinkyMcp.Position3D);
            Vector3 pPinkyPip = _is2D ? GetPosition2D(pinkyPip.Position2D) : GetPosition3D(pinkyPip.Position3D);
            Vector3 pPinkyDip = _is2D ? GetPosition2D(pinkyDip.Position2D) : GetPosition3D(pinkyDip.Position3D);
            Vector3 pPinkyTip = _is2D ? GetPosition2D(pinkyTip.Position2D) : GetPosition3D(pinkyTip.Position3D);
            Vector3 pPalm = _is2D ? GetPosition2D(palm.Position2D) : GetPosition3D(palm.Position3D);

            _points[0].transform.position = pRoot;
            _points[1].transform.position = pThumbCmc;
            _points[2].transform.position = pThumbMcp;
            _points[3].transform.position = pThumbIp;
            _points[4].transform.position = pThumbTip;
            _points[5].transform.position = pIndexMcp;
            _points[6].transform.position = pIndexPip;
            _points[7].transform.position = pIndexDip;
            _points[8].transform.position = pIndexTip;
            _points[9].transform.position = pMiddleMcp;
            _points[10].transform.position = pMiddlePip;
            _points[11].transform.position = pMiddleDip;
            _points[12].transform.position = pMiddleTip;
            _points[13].transform.position = pRingMcp;
            _points[14].transform.position = pRingPip;
            _points[15].transform.position = pRingDip;
            _points[16].transform.position = pRingTip;
            _points[17].transform.position = pPinkyMcp;
            _points[18].transform.position = pPinkyPip;
            _points[19].transform.position = pPinkyDip;
            _points[20].transform.position = pPinkyTip;
            _points[21].transform.position = pPalm;

            _thumb.SetPositions(new[] { pPalm, pThumbCmc, pThumbMcp, pThumbIp, pThumbTip });
            _index.SetPositions(new[] { pPalm, pIndexMcp, pIndexPip, pIndexDip, pIndexTip });
            _middle.SetPositions(new[] { pPalm, pMiddleMcp, pMiddlePip, pMiddleDip, pMiddleTip });
            _ring.SetPositions(new[] { pPalm, pRingMcp, pRingPip, pRingDip, pRingTip });
            _pinky.SetPositions(new[] { pPalm, pPinkyMcp, pPinkyPip, pPinkyDip, pPinkyTip });

            _mcpLine.SetPositions(new[] { pThumbMcp, pIndexMcp, pMiddleMcp, pRingMcp, pPinkyMcp, pRoot, pThumbCmc });

            _palmLine.SetPositions(new[] { pRoot, pPalm });

            _boundingBox.enabled = _is2D;

            if (_is2D)
            {
                _boundingBox.SetPositions(new[]
                {
                    (Vector3)_image.GetPosition(_hand.BoundingBox2D.min),
                    (Vector3)_image.GetPosition(new Vector2(_hand.BoundingBox2D.xMin, _hand.BoundingBox2D.yMax)),
                    (Vector3)_image.GetPosition(_hand.BoundingBox2D.max),
                    (Vector3)_image.GetPosition(new Vector2(_hand.BoundingBox2D.xMax, _hand.BoundingBox2D.yMin)),
                    (Vector3)_image.GetPosition(_hand.BoundingBox2D.min)
                });
            }


            // =========== Expanded =====================
            if (_is2D == false) // TRUE foor 2D, FALSE for 3D
            {
                if (Side == HandSide.Left)
                {
                    WebSocketConnection.Connection.SetHandInformation(true,
                                                                     pRoot,
                                                                     pPalm,

                                                                     pThumbTip,
                                                                     pThumbIp,
                                                                     pThumbMcp,
                                                                     pThumbCmc,

                                                                     pIndexTip,
                                                                     pIndexDip,
                                                                     pIndexPip,
                                                                     pIndexMcp,

                                                                     pMiddleTip,
                                                                     pMiddleDip,
                                                                     pMiddlePip,
                                                                     pMiddleMcp,

                                                                     pRingTip,
                                                                     pRingDip,
                                                                     pRingPip,
                                                                     pRingMcp,

                                                                     pPinkyTip,
                                                                     pPinkyDip,
                                                                     pPinkyPip,
                                                                     pPinkyMcp,

                                                                     _hand.BoundingBox2D.size.y
                                                                    );
                }
                else
                {
                    WebSocketConnection.Connection.SetHandInformation(false,
                                                                       pRoot,
                                                                       pPalm,

                                                                       pThumbTip,
                                                                       pThumbIp,
                                                                       pThumbMcp,
                                                                       pThumbCmc,

                                                                       pIndexTip,
                                                                       pIndexDip,
                                                                       pIndexPip,
                                                                       pIndexMcp,

                                                                       pMiddleTip,
                                                                       pMiddleDip,
                                                                       pMiddlePip,
                                                                       pMiddleMcp,

                                                                       pRingTip,
                                                                       pRingDip,
                                                                       pRingPip,
                                                                       pRingMcp,

                                                                       pPinkyTip,
                                                                       pPinkyDip,
                                                                       pPinkyPip,
                                                                       pPinkyMcp,

                                                                       _hand.BoundingBox2D.size.y
                                                                      );
                }

            }
            // ==========================================

        }

        public void Toggle(bool show)
        {
            _thumb.enabled = show;
            _index.enabled = show;
            _middle.enabled = show;
            _ring.enabled = show;
            _pinky.enabled = show;
            _mcpLine.enabled = show;
            _palmLine.enabled = show;
            _boundingBox.enabled = show;
        }

        private Vector3 GetPosition2D(Vector2 original)
        {
            if (_image == null) return (Vector3)original;

            return (Vector3)_image.GetPosition(original);
        }

        private Vector3 GetPosition3D(Vector3 original)
        {
            return original + _offset;
        }
    }
}
