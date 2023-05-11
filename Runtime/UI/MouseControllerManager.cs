using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

namespace CoffeyUtils
{
	public class MouseControllerManager : MonoBehaviour
	{
		private static MouseControllerManager instance;
		public static MouseControllerManager Instance
		{
			get
			{
				if (instance) return instance;
				instance = FindObjectOfType<MouseControllerManager>();
				if (!instance)
				{
#if ENABLE_INPUT_SYSTEM
					var inputSystem = FindObjectOfType<InputSystemUIInputModule>();
					instance = inputSystem.gameObject.AddComponent<MouseControllerManager>();
#elif ENABLE_LEGACY_INPUT_MANAGER
					var inputSystem = FindObjectOfType<StandaloneInputModule>();
					instance = inputSystem.gameObject.AddComponent<MouseControllerManager>();
#endif
				}
				instance.FindReferences();
				return instance;
			}
		}
	    
		private bool _inGame;
	    
		public static bool InGame;
		private static bool CanUpdate => !InGame;
	    
		private static bool _usingMouse;
		[SerializeField] private bool _assumeMouseFirst = true;
		[SerializeField] private float _ignoreMouseTime = 0.1f;
		[SerializeField, ReadOnly] private GameObject _currentlySelected;
		
#if ENABLE_INPUT_SYSTEM
		// New Input System
		private InputSystemUIInputModule _inputSystem;

		private void FindReferences()
		{
			_eventSystem = EventSystem.current;
			_currentlySelected = _eventSystem.firstSelectedGameObject;
			if (!_inputSystem) _inputSystem = GetComponent<InputSystemUIInputModule>();
			if (!_inputSystem) FindObjectOfType<InputSystemUIInputModule>();
		}
		private void OnEnable()
		{
			_inputSystem.move.action.performed += OnKeyboardControllerMovement;
			_inputSystem.point.action.performed += OnMouseMovement;
		}
		private void OnDisable()
		{
			_inputSystem.move.action.performed -= OnKeyboardControllerMovement;
			_inputSystem.point.action.performed -= OnMouseMovement;
		}
		private void OnKeyboardControllerMovement(InputAction.CallbackContext context) => OnKeyboardControllerMovement();
		private void OnMouseMovement(InputAction.CallbackContext context) => OnMouseMovement();
		
		private void WarpMouse(bool toCenter)
		{
			var corner = new Vector2(Screen.width, Screen.height);
			if (toCenter) Mouse.current.WarpCursorPosition(corner * 0.5f);
			else Mouse.current.WarpCursorPosition(corner - Vector2.one * 25);
		}
#elif ENABLE_LEGACY_INPUT_MANAGER
		// Old Input System
		private StandaloneInputModule _inputSystem;
		
		private void FindReferences()
		{
			_eventSystem = EventSystem.current;
			_currentlySelected = _eventSystem.firstSelectedGameObject;
			if (!_inputSystem) _inputSystem = GetComponent<StandaloneInputModule>();
			if (!_inputSystem) FindObjectOfType<StandaloneInputModule>();
		}

		private Vector3 _prevMousePos;
		private void CheckMouseMovement()
		{
			if (!CanUpdate) return; // No need to check for mouse updates
			var mousePos = Input.mousePosition;
			if (_usingMouse)
			{
				// Check for Keyboard or Controller Movement
				if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
				{
					OnKeyboardControllerMovement();
				}
			}
			else
			{
				// Check for Mouse Movement
				if (Vector3.Distance(mousePos, _prevMousePos) > 1f)
				{
					_prevMousePos = mousePos;
					OnMouseMovement();
				}
			}
		}
		
		private void WarpMouse(bool toCenter)
		{
			// No Support for Warping Mouse Position in the Old Input System
		}
#endif
		private EventSystem _eventSystem;
	
		private static bool _wasController;
		
		private bool _ignoreMouseMovement;
	
		private void Awake()
		{
			InGame = false;
			instance = this;
			FindReferences();
		}
	    
		private void Start()
		{
			if (_wasController && !_assumeMouseFirst)
			{
				SetUsingMouse(!_wasController);
			}
			else
			{
				SetUsingMouse(_assumeMouseFirst);
				_wasController = !_assumeMouseFirst;
			}
			if (_wasController)
			{
				StartCoroutine(IgnoreMouseMovement());
				WarpMouse(false);
			}
		}
		
		private void Update()
		{
			if (_inGame != InGame)
			{
				_inGame = InGame;
				if (_inGame)
				{
					_currentlySelected = null;
					SetUsingMouse(false);
					_usingMouse = false;
					WarpMouse(false);
				}
			}
			if (!_usingMouse)
			{
				var obj = _eventSystem.currentSelectedGameObject;
				if (obj) _currentlySelected = obj;
			}
#if ENABLE_LEGACY_INPUT_MANAGER
			CheckMouseMovement();
#endif
		}
	
		private void OnKeyboardControllerMovement()
		{
			if (!CanUpdate) return;
			_wasController = true;
			if (!_usingMouse) return;
			WarpMouse(false);
			StartCoroutine(IgnoreMouseMovement());
			SetUsingMouse(false);
		}
	    
		private IEnumerator IgnoreMouseMovement()
		{
			if (_ignoreMouseMovement) yield break;
			_ignoreMouseMovement = true;
			for (float t = 0; t < _ignoreMouseTime; t += Time.deltaTime)
			{
				yield return null;
			}
			_ignoreMouseMovement = false;
		}
	
		private void OnMouseMovement()
		{
			if (!CanUpdate || _ignoreMouseMovement) return;
			if (_wasController)
			{
				WarpMouse(true);
				_wasController = false;
				return;
			}
			if (_usingMouse) return;
			SetUsingMouse(true);
		}
	
		private void SetUsingMouse(bool usingMouse, bool setSelected = true)
		{
			_usingMouse = usingMouse;
			Cursor.visible = usingMouse;
			if (setSelected) SetSelected(usingMouse ? null : _currentlySelected);
		}
	
		public static void UpdateSelected(GameObject obj)
		{
			if (obj) Instance._currentlySelected = obj;
			if (!_usingMouse) Instance.SetSelected(obj);
		}
	
		private void SetSelected(GameObject obj)
		{
			_eventSystem.SetSelectedGameObject(obj);
		}
	}
}