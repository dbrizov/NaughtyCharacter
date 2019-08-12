using UnityEngine;

namespace NaughtyCharacter
{
	public class TestHelper : MonoBehaviour
	{
		[SerializeField]
		private int _targetFrameRate = 60;

		[SerializeField]
		private float _slowMotion = 0.1f;

		protected virtual void Awake()
		{
			Application.targetFrameRate = _targetFrameRate;
		}

		protected virtual void Update()
		{
			if (Input.GetButtonDown("Slow Motion"))
			{
				this.ToggleSlowMotion();
			}
		}

		private void ToggleSlowMotion()
		{
			Time.timeScale = Time.timeScale == 1f ? _slowMotion : 1f;
		}
	}
}
