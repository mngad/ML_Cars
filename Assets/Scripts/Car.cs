using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEditor;
using System.IO;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

namespace UnityStandardAssets.Vehicles.Car
{
    public class Car : Agent
    {
        // Start is called before the first frame update

        public GameObject thecar;
      

        //IFloatProperties m_ResetParams;
        
        public GameObject Holder;
        public Quaternion initRot;
        public Vector3 initPos;
        private int inc = 0;
        private float avg = 0f;
        public float time;
        public float actionX;
        public float actionY;
        public float actionZ;
        public int checkpointsHit = 0;
        private CarController m_Car; // the car controller we want to use
        public float points = 0;
        public float angle;
        public Vector3 otherAngle;
        private float count = 0;
        private float speedSum = 0;
        private void Awake()
        {
           Application.targetFrameRate = 300; 
            // get the car controller
            m_Car = GetComponent<CarController>();
            //Debug.Log("awake");
        }
        public void Update()
        {
            count += 1;
            speedSum += thecar.GetComponent<Rigidbody>().velocity.magnitude;

            time += Time.deltaTime;
            carCollide cc = thecar.GetComponent<carCollide>();

        
            if (cc.fence){
            
                SetReward(-2f);
                points -= 1f;
                cc.fence = false;
                EndEpisode();
                //Debug.Log("track");

            }
            if (cc.finish)
            {
               
                    cc.finish = false;
                    Debug.Log("finish, time = " + time);
                    Debug.Log("Holder.GetComponent<Fastest>().fastestTime = " + Holder.GetComponent<Fastest>().fastestTime);
                    if (Holder.GetComponent<Fastest>().fastestTime == 0 || time < Holder.GetComponent<Fastest>().fastestTime)
                    {
                        Debug.Log("here");
                        Holder.GetComponent<Fastest>().fastestTime = time;
                        Holder.GetComponent<Fastest>().fastestName = this.gameObject.name;
                        Holder.GetComponent<Fastest>().avgSpeed = speedSum / count;
                    }
                    SetReward(2f);
                    points += 2f;
                    EndEpisode();
                   
                
            }
            if (cc.checkpoint)
            {
                //Debug.Log("check");
                checkpointsHit += 1;
                SetReward(0.5f);
                points += 0.5f;
                cc.checkpoint = false;
            }

        }
        public override void Initialize()
        {

            
            initRot = thecar.transform.rotation;
            initPos = thecar.transform.position;
            //Debug.Log("InitPos = " + initPos);

            //m_ResetParams = Academy.Instance.FloatProperties;
            //SetResetParameters();

        }

        public override void OnActionReceived(float[] vectorAction)
        {
            actionX = 1f * Mathf.Clamp(vectorAction[0], -1f, 1f); //steer
            actionY = 10f * Mathf.Clamp(vectorAction[1], 0f, 1f); // accel
            actionZ = 0f;
            //actionZ= 1f * Mathf.Clamp(vectorAction[2], 0f, 1f);
            /*Debug.Log(actionX);
            Debug.Log(actionY);
            m_Car.Move(actionX, actionY, actionY, 0f);*/
            //SetReward(-0.0001f);
            
          
           /* if (thecar.GetComponent<Rigidbody>().velocity.z > 2f)
            {
                SetReward(0.01f);
            }
            if (thecar.GetComponent<Rigidbody>().velocity.z > 1f)
            {
                SetReward(0.005f);
            }*/
            if (thecar.GetComponent<Rigidbody>().velocity.magnitude <= 0.2 && checkpointsHit > 0 )
            {
                SetReward(-0.5f);
                EndEpisode();
                
            }
            if (thecar.GetComponent<Rigidbody>().velocity.magnitude >=1)
            {
                SetReward(0.01f * thecar.GetComponent<Rigidbody>().velocity.magnitude);

            }
            
            SetReward(-0.0001f);

        }

 
    public override void CollectObservations(VectorSensor sensor)
        {


            // Bit shift the index of the layer (8) to get a bit mask
            //int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.

            int layer1  = 8;
            int layer2  = 9;
            int layermask1 = 1 << layer1;
            int layermask2  = 1 << layer2;
            int layerMask = layermask1 | layermask2; // Or, (1 << layer1) | (1 << layer2)


            layerMask = ~layerMask;
            var sensorLength = 100.0f;


            //Quaternion downAng =  Quaternion.AngleAxis(-20f, thecar.transform.forward);
            //Quaternion straightAng =  Quaternion.AngleAxis(-90, thecar.transform.up);

            Vector3 ogAngle = thecar.transform.right;
            
           
            float[] angles = new float[] { 90f, -90f, 60f, -60f, -20f, 20f, 0f };
            float[] dangles = new float[] { -20f, -40f, -10f };
            int numDirections = 21;

            Vector3[] directions = new Vector3[numDirections];
            
            for (int i = 0; i < 7; i++)
            {
                Quaternion downAng = Quaternion.AngleAxis(dangles[0], thecar.transform.forward);
                Quaternion forwardAng = Quaternion.AngleAxis(-90 + angles[i], thecar.transform.up);
                Vector3 Dir = downAng * ogAngle;
                directions[i] = forwardAng * Dir;
            }
            for (int i = 7; i <14; i++)
            {
                Quaternion downAng = Quaternion.AngleAxis(dangles[1], thecar.transform.forward);
                Quaternion forwardAng = Quaternion.AngleAxis(-90 + angles[i-7], thecar.transform.up);
                Vector3 Dir = downAng * ogAngle;
                directions[i] = forwardAng * Dir;
            }
            for (int i = 14; i < 21; i++)
            {
                Quaternion downAng = Quaternion.AngleAxis(dangles[2], thecar.transform.forward);
                Quaternion forwardAng = Quaternion.AngleAxis(-90 + angles[i-14], thecar.transform.up);
                Vector3 Dir = downAng * ogAngle;
                directions[i] = forwardAng * Dir;
            }
            Vector3 offset = transform.up * 2f;
            RaycastHit[] hits = new RaycastHit[numDirections];
            

            //Debug.DrawRay(thecar.transform.position + offset, sDir * 100f, Color.green);

            for (int i = 0; i < numDirections; i++)
            {
                
                if (Physics.Raycast(thecar.transform.position + offset, directions[i], out hits[i], sensorLength, layerMask))
                {
                    Debug.DrawRay(thecar.transform.position + offset, directions[i] * hits[i].distance, Color.yellow);
                    sensor.AddObservation(hits[i].distance);
                    //Debug.DrawRay(thecar.transform.position , sDir * shit.distance, Color.green);
                   //Debug.Log(hits[i].distance);
                }
                else
                {
                    sensor.AddObservation(0f);
                }
            }
            sensor.AddObservation(thecar.GetComponent<Rigidbody>().velocity.magnitude);



        }






        public override void OnEpisodeBegin()
        {

           
            time = 0f;
           // Debug.Log("reset called");

            checkpointsHit = 0;
            thecar.transform.position = initPos;
            thecar.transform.rotation = initRot;
            Quaternion rotate180 = new Quaternion(0f, 180f, 0f, 0f);
            thecar.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            thecar.GetComponent<Rigidbody>().transform.rotation = initRot;
            points = 0;



            //Reset the parameters when the Agent is reset.
            //SetResetParameters();
        }
/*
        public override float[] Heuristic()
        {
            var action = new float[2];

            action[0] = -Input.GetAxis("Horizontal");
            action[1] = Input.GetAxis("Vertical");
            return action;
        }
*/

    }


}
