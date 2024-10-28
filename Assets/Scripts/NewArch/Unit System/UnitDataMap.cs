using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnitDataMapping
{
    public class UnitDataMap<T>
    {
        private Dictionary<IUnit,T> dictionary;

        public UnitDataMap()
        {
            dictionary = new Dictionary<IUnit,T>();
        }

        public void PutData(IUnit unit, T data)
        {
            if(dictionary.Keys.Contains(unit) != true)
            {
                dictionary.Add(unit,data);
            }
            else
            {
                dictionary[unit] = data;
            }          
        }

        public T GetData(IUnit unit)
        {
            T data = default(T);
            if(dictionary.ContainsKey(unit) != true)
            {
                data = dictionary[unit];
            }
            return data;
        }
    }
}

    