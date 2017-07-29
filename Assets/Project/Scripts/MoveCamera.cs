using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace LudumDare39
{
    public class MoveCamera : MonoBehaviour
    {
        public const float DefaultMultiplier = -1f;

        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("target")]
        Transform defaultTarget;
        [SerializeField]
        float slerpMultiplier = 5f;

        [Header("Offsets")]
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("offset")]
        Vector3 defaultOffset = new Vector3(0f, 7.5f, 0f);
        [SerializeField]
        Vector2 offsetMultiplierRange = new Vector2(0.5f, 3f);

        [Header("Camera Controls")]
        [SerializeField]
        float keyboardZoomMultiplier = 0.1f;
        [SerializeField]
        float scrollZoomMultiplier = 0.1f;
        [SerializeField]
        bool reverseZoomDirection = true;

        Transform target;
        float playerMultiplier = 1;
        float overrideMultiplier = DefaultMultiplier;

        public Transform Target
        {
            get
            {
                return target;
            }
            set
            {
                target = defaultTarget;
                if (value)
                {
                    target = value;
                }
            }
        }

        public float Multiplier
        {
            get
            {
                if(overrideMultiplier < 0)
                {
                    return playerMultiplier;
                }
                else
                {
                    return overrideMultiplier;
                }
            }
            set
            {
                overrideMultiplier = value;
            }
        }

        public Vector3 TargetPosition
        {
            get
            {
                return Target.position + defaultOffset * Multiplier;
            }
        }

        private void Start()
        {
            Target = null;
        }

#if !SERVER
        private void Update()
        {
            // Resolve how much to adjust the multiplier value
            float magnitude = CrossPlatformInputManager.GetAxis("Vertical");
            if(Mathf.Approximately(magnitude, 0) == false)
            {
                magnitude *= keyboardZoomMultiplier;
            }
            else
            {
                magnitude = CrossPlatformInputManager.GetAxis("Mouse ScrollWheel");
                magnitude *= scrollZoomMultiplier;
            }

            // Check if we want to reverse direction
            if(reverseZoomDirection == true)
            {
                magnitude *= -1;
            }

            // Adjust the multiplier by the magnitude, albeit clamped by offsetMultiplierRange
            playerMultiplier = Mathf.Clamp((playerMultiplier + magnitude), offsetMultiplierRange.x, offsetMultiplierRange.y);
        }

        void FixedUpdate()
        {
            transform.position = Vector3.Slerp(transform.position, TargetPosition, Time.deltaTime * slerpMultiplier);
        }
#endif

#if UNITY_EDITOR
        [ContextMenu("Set Offset")]
        public void SetOffset()
        {
            defaultOffset = transform.localPosition;
        }
#endif
    }
}
