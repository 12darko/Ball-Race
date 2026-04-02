using UnityEngine;

namespace EMA.Scripts.MyShortcuts
{
    public class ProviderThatNotToExecuteStartMethods : MonoBehaviour
    {
        [SerializeField] private bool doesItsActivenessIsDisablingAtInstantiating;

        private int doNotExecute = 0;
        private int controller;
        private bool Execute()
        {
            if (doNotExecute < controller) {
                doNotExecute++;
                return false;
            }
            return true;
        }

        private void Awake()
        {
            controller = doesItsActivenessIsDisablingAtInstantiating ? 3 : 2;

            if (!Execute()) return;
            CustomAwake();
        }

        private void Start()
        {
            if (!Execute()) return;
            CustomStart();
        }

        private void OnEnable()
        {
            if (!Execute()) return;
            CustomOnEnable();
        }

        private void OnDisable()
        {
            if (!Execute()) return;
            CustomOnDisable();
        }

        protected virtual void CustomAwake()
        {
        }
        protected virtual void CustomStart()
        {
        }
        protected virtual void CustomOnEnable()
        {
        }
        protected virtual void CustomOnDisable()
        {
        }
    }
}
