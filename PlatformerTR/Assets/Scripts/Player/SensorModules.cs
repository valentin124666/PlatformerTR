using System;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class SensorModules : MonoBehaviour
    {
        [SerializeField] private SensorView groundSensor;
        [SerializeField] private SensorView[] rightSensor;
        [SerializeField] private SensorView[] leftSensor;

        public event Action<bool> StandingOnGroundEvent;
        public event Action<bool> WallSlideRightEvent;

        private void Awake()
        {
            groundSensor.EnterTrigger += StandingOnGround;
            groundSensor.ExitTrigger += StandingOnGround;

            foreach (var sensor in rightSensor)
            {
                sensor.EnterTrigger += WallSlideRight;
                sensor.ExitTrigger += WallSlideRight;
            }

            foreach (var sensor in leftSensor)
            {
                sensor.EnterTrigger += WallSlideRight;
                sensor.ExitTrigger += WallSlideRight;
            }
        }

        private void StandingOnGround()
        {
            StandingOnGroundEvent?.Invoke(groundSensor.State());
        }

        private void WallSlideRight()
        {
            WallSlideRightEvent?.Invoke(rightSensor.All(sensor => sensor.State()) || leftSensor.All(sensor => sensor.State()));
        }
    }
}