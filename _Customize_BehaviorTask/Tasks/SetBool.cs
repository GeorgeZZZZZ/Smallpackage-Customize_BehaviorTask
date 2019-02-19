//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
    [TaskCategory("George's Script")]
    [TaskDescription("Set Bool Value in Behavior Tree.")]
    public class SetBool : Action
    {
        public SharedBool BoolInBehavTree;
        public SharedBool ValueWnatToSet;
        public override TaskStatus OnUpdate()
        {
            BoolInBehavTree.Value = ValueWnatToSet.Value; 
            
            return TaskStatus.Success;
        }
    }
}