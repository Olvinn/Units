using System;
using System.Collections.Generic;
using Input;
using Units.Controllers;
using UnityEngine;

namespace UnitBehaviours
{
    public class ManualBehaviour : IUnitBehaviour
    {
        public event Action onCheckSurroundings;
        
        private IInput _input;
        private IUnitController _controller;
        
        public ManualBehaviour(IUnitController controller, IInput input)
        {
            _controller = controller;
            _input = input;
        }

        public void SetKnownEnemies(List<IUnitBehaviour> enemies)
        {
            throw new NotImplementedException();
        }

        public IUnitController GetController()
        {
            throw new System.NotImplementedException();
        }

        public void Attack(IUnitBehaviour unit)
        {
            throw new System.NotImplementedException();
        }

        public void MoveTo(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        public void SetManualControl(bool value)
        {
            throw new NotImplementedException();
        }
    }
}
