using System;
using System.Collections.Generic;

namespace Cars
{
    [Serializable]
    public class Car
    {
        #region Property

        public int ID { get; set; }
        public CarType Type { get; set; }
        public CarState State { get; set; }

        public List<CarTrail> Trails { get; set; }

        #endregion

        #region Constructor

        public Car(int id, CarType type)
        {
            ID = id;
            Type = type;
            Trails = new List<CarTrail>();
        }

        #endregion
        
    }
}