using UnityEngine;

namespace NaughtyCharacter
{
    public abstract class Controller : MonoBehaviour
    {
        protected Character _character;

        private void Awake()
        {
            Init();
        }

        public virtual void Init()
        {
            _character = GetComponent<Character>();
        }

        public abstract void OnInputUpdate();
        public abstract void OnBeforeCharacterMoved();
        public abstract void OnAfterCharacterMoved();
    }
}
