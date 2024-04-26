using System;

using UnityEngine;
using UnityEngine.EventSystems;

using UInput = UnityEngine.Input;

namespace TS.Inputs
{
    public class TouchManager : MonoBehaviour
    {
        [Tooltip("This texture will be drawn to simulate the touch position.")]
        [SerializeField] private Texture2D fingerTexture;

        private Touch _touch;
        private Vector2 _startPosition, _currentPosition, _lastPosition, _deltaPosition;

        // events
        public event Action<Vector2,Vector2> OnSingleTouchDrag;
        public event Action<Vector2> OnSingleTouchUp;
        public event Action<Vector2> OnSingleTouchDown;


        #region Methods -> Unity Callbacks

        private void Update()
        {
            UpdateTouch();

            if (_touch == null || IsTouchOverUI())
                return;

            DetectSingleTouchGestures();
        }

        private void OnGUI()
        {
            if (!CanDrawTouchGUI())
                return;
            
            var pos = _touch.Position;
            var screenRect = new Rect(0, 0, fingerTexture.width, fingerTexture.height)
            {
                center = new Vector2(pos.x, Screen.height - pos.y),
            };

            GUI.DrawTexture(screenRect, fingerTexture);
        }

        #endregion

        #region Methods -> Private

        private void DetectSingleTouchGestures()
        {
            switch (_touch.Phase)
            {
                case TouchPhase.Canceled:
                    return;
                case TouchPhase.Began:
                {
                    _startPosition = _touch.Position;
                    _lastPosition = _startPosition;
                    OnSingleTouchDown?.Invoke(_startPosition);
                    break;
                }
                case TouchPhase.Ended:
                {
                    OnSingleTouchUp?.Invoke(_lastPosition);
                    break;
                }
                case TouchPhase.Stationary:
                case TouchPhase.Moved:
                {
                    _currentPosition = _touch.Position;
                    _deltaPosition = _lastPosition - _currentPosition;
                    _lastPosition = _currentPosition;

                    if (_deltaPosition.magnitude == 0f)
                        break;
                    
                    OnSingleTouchDrag?.Invoke(_currentPosition, _deltaPosition);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool IsTouchOverUI()
        {
            var system = EventSystem.current;
            if (system is null)
                return false;
            
            var editorIsPointerOverGo = system.IsPointerOverGameObject();
            var mobileIsPointerOverGo = system.IsPointerOverGameObject(0);
            return IsEditorGameView() ? editorIsPointerOverGo : mobileIsPointerOverGo;
        }

        private bool IsEditorGameView()
        {
#if UNITY_EDITOR
            return UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop;
#elif UNITY_IOS || UNITY_ANDROID
            return false;
#endif
        }

        private void UpdateTouch()
        {
            _touch = null;
            if (IsEditorGameView() || Application.isEditor)
            {
                if (IsEditorGameView() && 
                    TryConvertMouseToTouch(out var touch))
                {
                    UpdateTouch(touch.Position, touch.Phase);
                }
                else if (UInput.touchCount > 0)
                {
                    GetAndUpdateNativeTouch();
                }
            }
            else if(UInput.touchCount>0)
            {
                GetAndUpdateNativeTouch();
            }
        }

        private void UpdateTouch(Vector2 position, TouchPhase phase)
        {
            position.x = Mathf.Clamp(position.x, 0f, Screen.width);
            position.y = Mathf.Clamp(position.y, 0f, Screen.height);
            
            _touch = new Touch(position, phase);
        }

        private void GetAndUpdateNativeTouch()
        {
            var uTouch = UInput.GetTouch(0);
            UpdateTouch(uTouch.position, uTouch.phase);
        }
        
        private bool TryConvertMouseToTouch(out Touch touch)
        {
            var phase = TouchPhase.Canceled;
            if (UInput.GetMouseButtonDown(0))
            {
                phase = TouchPhase.Began;
            }
            else if (UInput.GetMouseButtonUp(0))
            {
                phase = TouchPhase.Ended;
            }
            else if (UInput.GetMouseButton(0))
            {
                phase = TouchPhase.Moved;
            }

            touch = null;
            if (phase != TouchPhase.Canceled)
            {
                touch = new Touch(UInput.mousePosition, phase);
            }

            return touch != null;
        }
        
        private bool CanDrawTouchGUI()
        {
            return Application.isPlaying &&
                   Application.isEditor && 
                   fingerTexture is not null &&
                   _touch is not null;
        }

        #endregion
    }
}