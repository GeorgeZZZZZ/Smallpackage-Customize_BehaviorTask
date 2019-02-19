using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GeorgeScript;
using static GeorgeScript.RTS_Centralization;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("George's Script")]
    [TaskDescription("Just read target from Selectable_Unit_Controller")]
    public class SelectableUnitValueManager : Action
    {
        [Tooltip("set walk speed to this unit")]
        public SharedFloat inputUnitWalkSpeed;  // set walk speed to this unit

        [Tooltip("set run speed to this unit")]
        public SharedFloat inputUnitRunSpeed;  // set run speed to this unit

        [Tooltip("give out managed unit speed to other task")]
        public SharedFloat outputUnitSpeed;  // give out managed unit speed to other task

        [Tooltip("give out unit next movement target to other task")]
        public SharedGameObject outputMoveTarget;  // give out unit next movement target to other task
        [Tooltip("give out unit next movement position to other task")]
        public SharedVector3 outputMoveVector;

        [Tooltip("indicate whether unit running or not")]
        public SharedBool isRunning;    // indicate whether unit running or not
        public SharedBool isEnemy;
        public SharedBool isResources;
        protected GeorgeScript.Selectable_Unit_Controller selectUnit;
        public override void OnStart()
        {
            selectUnit = GetComponent<GeorgeScript.Selectable_Unit_Controller>();
            selectUnit.newOrder += IsNewOrder;
            outputMoveTarget.Value = null;
            outputMoveVector.Value = transform.position;    // give current pos as initial position
        }
        public override TaskStatus OnUpdate()
        {
            if (selectUnit != null)
            {
                return TaskStatus.Running;
            }
            else
            {
                Debug.LogWarning("No Selectable_Unit_Controller script attached but try to access in behavior manager");
                return TaskStatus.Failure;
            }
        }

        public override void OnBehaviorComplete()
        {
            // unsubscribe after this behavior is finish or destroy
            // use null detect is because no matter this task is start or not, it execute this code when behavior tree comlete or destory
            unsubscribe_event();
        }
        public override void OnEnd()
        {
            // unsubscribe because OnStart will call every time after this task successed
            unsubscribe_event();
        }

        protected void IsNewOrder(RightClickInfoToUnits _info)
        {
            switch (_info.type)
            {
                case RC_InfoType.None:  // if just a false parameter passing then return
                    return;
                case RC_InfoType.pos:
                    isResources.Value = isEnemy.Value = false;
                    outputMoveVector.Value = _info.newPos;
                    outputMoveTarget.Value = null;  // clear the target for ai script to use vector as move target
                    break;
                case RC_InfoType.enemyTar:
                    isResources.Value = false;
                    isEnemy.Value = true;
                    outputMoveTarget.Value = _info.newTar;
                    break;
                case RC_InfoType.resource:
                    isResources.Value = true;
                    isEnemy.Value = false;
                    outputMoveTarget.Value = _info.newTar;
                    break;
            }
            isRunning.Value = _info.isRun;
            if (isRunning.Value)
            {
                outputUnitSpeed.Value = inputUnitRunSpeed.Value;
            }
            else outputUnitSpeed.Value = inputUnitWalkSpeed.Value;
        }
        protected void unsubscribe_event()
        {
            if (selectUnit != null) selectUnit.newOrder -= IsNewOrder;
        }
    }
}