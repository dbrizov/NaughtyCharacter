using UnityEngine;
using UnityEngine.UI;

namespace NaughtyCharacter
{
	[RequireComponent(typeof(Text))]
	public class FpsCounter : MonoBehaviour
	{
		private float _fpsMeasurePeriod;
		private float _fpsNextPeriod;
		private int _fpsAccumulator;
		private int _currentFps;

		private Text _textComponent;

		private void Awake()
		{
			_textComponent = this.GetComponentInChildren<Text>(true);
		}

		private void OnEnable()
		{
			_fpsMeasurePeriod = 0.5f;
			_fpsNextPeriod = 0f;
			_fpsAccumulator = 0;
			_currentFps = 0;
		}

		private void Start()
		{
			_fpsNextPeriod = Time.realtimeSinceStartup + _fpsMeasurePeriod;
		}

		private void Update()
		{
			if (!_textComponent.enabled)
			{
				return;
			}

			// Measure average frames per second
			_fpsAccumulator++;
			if (Time.realtimeSinceStartup > _fpsNextPeriod)
			{
				_currentFps = (int)(_fpsAccumulator / _fpsMeasurePeriod);
				_fpsAccumulator = 0;
				_fpsNextPeriod += _fpsMeasurePeriod;
				_textComponent.text = _currentFps.ToString();
			}
		}
	}
}
