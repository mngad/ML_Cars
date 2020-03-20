using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        public GameObject theAgent;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
        {

            
            Car car = theAgent.GetComponent<Car>();
            float h = car.actionX;
            float v = car.actionY;
            float b = car.actionZ;
            // pass the input to the car!
            /*            float h = CrossPlatformInputManager.GetAxis("Horizontal");
                        float v = CrossPlatformInputManager.GetAxis("Vertical");*/
#if !MOBILE_INPUT
            float handbrake = CrossPlatformInputManager.GetAxis("Jump");
            m_Car.Move(h, v, b, 0f);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
    }
}
