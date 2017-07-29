using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace LudumDare39
{
    public class MoveCamera : MonoBehaviour
    {
        public const float DefaultPlayerZoomMultiplier = 1f;
        public const float DefaultOverrideZoomMultiplier = -1f;

        public const string PlayerMultiplierField = "DefaultZoomMultiplier";
        public const string MinMultiplierField = "MinZoomMultiplier";
        public const string MaxMultiplierField = "MaxZoomMultiplier";
        public const string KeyboardControlsField = "KeyboardZoomMultiplier";
        public const string ScrollWheelField = "ScrollWheelZoomMultiplier";

        static MoveCamera instance = null;

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
        float playerMultiplier = DefaultPlayerZoomMultiplier;
        float overrideMultiplier = DefaultOverrideZoomMultiplier;
        bool isSetup = false;

        #region Properties
        public static MoveCamera Instance
        {
            get
            {
                return instance;
            }
        }

        public Transform Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
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
                if(Target)
                {
                    return Target.position + defaultOffset * Multiplier;
                }
                else
                {
                    return transform.position;
                }
            }
        }

        public float DefaultZoomMultiplier
        {
            get
            {
                return RemoteSettings.GetFloat(PlayerMultiplierField, DefaultPlayerZoomMultiplier);
            }
        }

        public float MinZoomMultiplier
        {
            get
            {
                return RemoteSettings.GetFloat(MinMultiplierField, offsetMultiplierRange.x);
            }
        }

        public float MaxZoomMultiplier
        {
            get
            {
                return RemoteSettings.GetFloat(MaxMultiplierField, offsetMultiplierRange.y);
            }
        }
        #endregion

        public void FocusOnPlayer()
        {
            if(MovePlayer.Instance)
            {
                Target = MovePlayer.Instance.transform;
            }
        }

        public void ResetZoomLevel()
        {
            playerMultiplier = DefaultZoomMultiplier;
            overrideMultiplier = DefaultOverrideZoomMultiplier;
        }

        #region Unity Events
        private void Start()
        {
            // Setup instance
            instance = this;
        }

#if !SERVER || UNITY_EDITOR
        private void Update()
        {
            if ((MovePlayer.Instance) && (isSetup == false))
            {
                FocusOnPlayer();
                isSetup = true;
            }
            AdjustZoomMultiplier();
        }

        void FixedUpdate()
        {
            transform.position = Vector3.Slerp(transform.position, TargetPosition, Time.deltaTime * slerpMultiplier);
        }
#endif
        #endregion

        private void AdjustZoomMultiplier()
        {
            // Resolve how much to adjust the multiplier value
            float magnitude = CrossPlatformInputManager.GetAxis("Vertical");
            if (Mathf.Approximately(magnitude, 0) == false)
            {
                magnitude *= RemoteSettings.GetFloat(KeyboardControlsField, keyboardZoomMultiplier);
            }
            else
            {
                magnitude = CrossPlatformInputManager.GetAxis("Mouse ScrollWheel");
                magnitude *= RemoteSettings.GetFloat(ScrollWheelField, scrollZoomMultiplier);
            }

            // Check if we want to reverse direction
            if (reverseZoomDirection == true)
            {
                magnitude *= -1;
            }

            // Adjust the multiplier by the magnitude, albeit clamped by offsetMultiplierRange
            playerMultiplier = Mathf.Clamp((playerMultiplier + magnitude), MinZoomMultiplier, MaxZoomMultiplier);
        }

#if UNITY_EDITOR
        [ContextMenu("Set Offset")]
        public void SetOffset()
        {
            defaultOffset = transform.localPosition;
        }
#endif
    }
}
