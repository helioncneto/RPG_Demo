using UnityEngine;
using System.Collections;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;

        public void StartAction(IAction action)
        {
            if (action == currentAction) return;
            if (currentAction != null)
            {
                currentAction.Cancel();
                //print("Cancelado " + currentAction);
            }
            currentAction = action;

        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}
