using UnityEngine;

namespace NaughtyCharacter
{
	public abstract class Controller : ScriptableObject
	{
		public Character Character { get; set; }

		public abstract void Init();
		public abstract void OnCharacterUpdate();
		public abstract void OnCharacterFixedUpdate();
	}
}
