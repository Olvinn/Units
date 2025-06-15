using UnityEngine;

namespace Base
{
    public static class AnimatorNames 
    {
        public static readonly int AttackPreparation = Animator.StringToHash("Attack Preparation");
        public static readonly int BlockPreparation = Animator.StringToHash("Block Preparation");
        public static readonly int Block = Animator.StringToHash("Block");
        public static readonly int Evade = Animator.StringToHash("Evade");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Speed = Animator.StringToHash("Speed");
        public static readonly int Stop = Animator.StringToHash("Stop");
    }
}
