using UnityEngine;

namespace Cars
{
    public class CarTrail
    {
        #region Property

        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        #endregion

        #region Constructor

        public CarTrail(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        #endregion
        
    }
}