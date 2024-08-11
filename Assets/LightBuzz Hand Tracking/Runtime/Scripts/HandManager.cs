using System.Collections.Generic;
using UnityEngine;

namespace LightBuzz.HandTracking
{
    public class HandManager : MonoBehaviour
    {
        [SerializeField] private GameObject _handPrefab;

        [SerializeField] private bool _is2D = true;
        
        [ConditionalHide("_is2D", false)]
        [SerializeField] private ImageView _image;

        private readonly List<HandVisual> _handVisuals = new List<HandVisual>();

        public List<HandVisual> HandVisuals => _handVisuals;

        public bool Is2D
        {
            get => _is2D;
            set => _is2D = value;
        }

        private void Awake()
        {
            if (_is2D && _image == null)
            {
                _image = FindObjectOfType<ImageView>();

                if (_image == null)
                {
                    Debug.LogError("ImageView component not found.");
                }
            }

            if (_handPrefab == null)
            {
                Debug.LogError("HandPrefab component not found.");
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _handVisuals.Count; i++)
            {
                if (_handVisuals[i] == null) continue;

                Destroy(_handVisuals[i].gameObject);
            }

            _handVisuals.Clear();
        }

        public void Load(List<Hand> hands, List<Vector3> offsets = null)
        {
            if (hands == null)
            {
                Clear();
                return;
            }

            if (hands.Count != _handVisuals.Count)
            {
                Clear();
            }

            for (int i = 0; i < hands.Count; i++)
            {
                if (i >= _handVisuals.Count)
                {
                    CreateElement();
                }

                Hand hand = hands[i];
                Vector3 offset = offsets?[i] ?? Vector3.zero;

                _handVisuals[i].Load(hand, offset);
            }
        }

        private void CreateElement()
        {
            GameObject handObject = Instantiate(_handPrefab, transform);
            HandVisual handVisual = handObject.GetComponent<HandVisual>();
            handVisual.Is2D = _is2D;
            handVisual.Image = _image;

            _handVisuals.Add(handVisual);
        }
    }
}