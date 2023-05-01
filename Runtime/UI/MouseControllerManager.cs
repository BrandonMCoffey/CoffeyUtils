using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

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
					var inputSystem = FindObjectOfType<InputSystemUIInputModule>();
					instance = inputSystem.gameObject.AddComponent<MouseControllerManager>();
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
	    
		private InputSystemUIInputModule _inputSystem;
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
	
		private void FindReferences()
		{
			_eventSystem = EventSystem.current;
			if (!_inputSystem) _inputSystem = GetComponent<InputSystemUIInputModule>();
			if (!_inputSystem) FindObjectOfType<InputSystemUIInputModule>();
		}
	
		private void OnEnable()
		{
			_inputSystem.move.action.performed += ReturnToKeyboardController;
			_inputSystem.point.action.performed += OnMouseMovement;
		}
	    
		private void OnDisable()
		{
			_inputSystem.move.action.performed -= ReturnToKeyboardController;
			_inputSystem.point.action.performed -= OnMouseMovement;
		}
	
		private void Update()
		{
			if (!_usingMouse)
			{
				var obj = _eventSystem.currentSelectedGameObject;
				if (obj) _currentlySelected = obj;
			}
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
		}
	
		private void ReturnToKeyboardController(InputAction.CallbackContext context)
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
	
		private void OnMouseMovement(InputAction.CallbackContext context)
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
		
		private void WarpMouse(bool toCenter)
		{
			var corner = new Vector2(Screen.width, Screen.height);
			if (toCenter)
			{
				Mouse.current.WarpCursorPosition(corner * 0.5f);
			}
			else
			{
				Mouse.current.WarpCursorPosition(corner - Vector2.one * 25);
			}
		}
	}
}