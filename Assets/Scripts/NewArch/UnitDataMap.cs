using System;
using System.Collections.Generic;

namespace UnitDataMapping
{
    public class UnitDataMap<T>
    {
        private Dictionary<IUnit,T> dictionary;

        public UnitDataMap()
        {
            dictionary = new Dictionary<IUnit,T>
        }

        public PutData(IUnit ability,IAbilityData data)
        {
            if(dictionary.keys.contains(ability) != true)
            {
                dictionary.add(ability,data);
            }
            else
            {
                dictionary[ability] = data;
            }
            
        }

        public IAbilityData GetData(IAbility ability)
        {
            IAbilityData data = null;
            if(dictionary.keys.contains(ability) != true)
            {
                data = dictionary[ability];
            }
            return data;
        }
    }

    public class FireballAbility : IAbility
    {

        private Dictionary<IUnit,int> unitsFireballExecutions = new Dictionary<IUnit, int>();

        public bool IsValidTarget(IUnit unit, ITarget target)
        {
            // Implement validation logic
            return target != null && target.IsEnemy;
        }

        public void Command(IUnit unit, ITarget target, Action onAbilityExecuted)
        {
            //Imagine we want to track how many times this ability was executed by this unit
            //The third execution damage is doubled!
            //But this is a scriptableObject so we need to map the data to the unit
            
            int multiplier = 1;
            int numbersOfExecutions = GetUnitData(unit);
            numbersOfExecutions++;
            PutUnitData(unit,numbersOfExecutions);
            
            if(numbersOfExecutions % 3 == 0){
                multiplier = 2;
            }

            Debug.Log("Executing Fireball Ability");
            target.TakeDamage(50*multiplier);

            // Call the callback action
            onAbilityExecuted?.Invoke();
        }

        public bool IsAutoTarget()
        {
            return false;
        }

        private void PutUnitData(IUnit unit,int data)
        {
            if(unitsFireballExecutions.ContainsKey(unit))
            {
                unitsFireballExecutions[unit] = data;
            }       
        }

        private int GetUnitData(IUnit unit)
        {
            int data = 0; //default value
            if(unitsFireballExecutions.ContainsKey(unit) != true)
            {
                data = unitsFireballExecutions[unit];
            }
            else
            {
                //First time we try to get the data, we instantiate it
                unitsFireballExecutions.Add(unit,data);
            }
            return data;
        }

        public bool IsAvailable()
        {
            throw new NotImplementedException();
        }

        public void Command(IUnit unit, ITarget target, Action onAbilityExecuted)
        {
            throw new NotImplementedException();
        }

        public void OnTargetHoverEnter(ITarget target)
        {
            throw new NotImplementedException();
        }

        public void OnTargetHoverExit(ITarget target)
        {
            throw new NotImplementedException();
        }

        public void Selected()
        {
            throw new NotImplementedException();
        }

        public void Deselected()
        {
            throw new NotImplementedException();
        }

        public bool AllowsMovement()
        {
            throw new NotImplementedException();
        }
    }
}
