using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CoffeyUtils
{
	public class AnimatedButtonHoverSelect : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private SfxReference _sfxOnSelectHover;
		[SerializeField] private SfxReference _sfxOnClick;
		[SerializeField] private bool _animateScale = true;
		[SerializeField, ShowIf("_animateScale")] private Transform _transform;
		[SerializeField, ShowIf("_animateScale")] private float _animationTime = 0.2f;
		[SerializeField, ShowIf("_animateScale")] private float _widthScale = 1.06f;
		[SerializeField, ShowIf("_animateScale")] private float _heightScale = 1.12f;
		[SerializeField, ShowIf("_animateScale")] private AnimationCurve _deltaCurve
			= new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
			
		[SerializeField, ReadOnly] private float _delta;
		
		private Coroutine _routine;
		
		private void OnValidate()
		{
			if (!_transform) _transform = transform;
		}
	    
		private void HoverSelect(bool active)
		{
			if (active) _sfxOnSelectHover.Play();
			if (_animateScale)
			{
				if (_routine != null) StopCoroutine(_routine);
				_routine = StartCoroutine(HoverSelectRoutine(active));
			}
		}
		
		private IEnumerator HoverSelectRoutine(bool active)
		{
			if (_animationTime <= 0)
			{
				Debug.LogWarning($"Invalid Animation Time of {_animationTime}", gameObject);
				_animationTime = 1f;
			}
			float inverseTime = 1f / _animationTime;
			if (active)
			{
				for (float t = _delta; t < 1; t += Time.deltaTime * inverseTime)
				{
					UpdateDelta(t);
					yield return null;
				}
				UpdateDelta(1);
			}
			else
			{
				for (float t = _delta; t > 0; t -= Time.deltaTime * inverseTime)
				{
					UpdateDelta(t);
					yield return null;
				}
				UpdateDelta(0);
			}
			_routine = null;
		}
		
		private void UpdateDelta(float delta)
		{
			_delta = delta;
			var d = _deltaCurve.Evaluate(delta);
			var w = Mathf.Lerp(1, _widthScale, d);
			var h = Mathf.Lerp(1, _heightScale, d);
			_transform.localScale = new Vector3(w, h, 1);
		}
	
		public void OnSelect(BaseEventData eventData) => HoverSelect(true);
		public void OnDeselect(BaseEventData eventData) => HoverSelect(false);
		public void OnPointerEnter(PointerEventData eventData) => HoverSelect(true);
		public void OnPointerExit(PointerEventData eventData) => HoverSelect(false);
		
		public void OnSubmit(BaseEventData eventData)
		{
			_sfxOnClick.Play();
		}
	
		public void OnPointerClick(PointerEventData eventData)
		{
			_sfxOnClick.Play();
		}
	}
}