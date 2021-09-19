using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Aircraft
{
    public class AircraftArea : MonoBehaviour
    {
        //variables
        public CinemachineSmoothPath racePath; // the path the airplanes will take. 
        
        public GameObject checkPointPrefab; // reference to the created prefabs

        public GameObject finishPoinPrefab;

        public bool trainingMode; // a bool variable to check whether our system is in training mode or not. 

        public List<AircraftAgent> AircraftAgents { get; private set; }
        public List <GameObject> Checkpoints { get; private set; }


        private void Awake()
        {
            if (AircraftAgents == null)
            {
                FindAircraftAgents();
            }
        }
        private void Start()
        {
            if (Checkpoints == null)
            {
                CreateCheckPoints();
            }
        }
        private void FindAircraftAgents()
        {
            AircraftAgents = transform.GetComponentsInChildren<AircraftAgent>().ToList();
            Debug.Assert(AircraftAgents.Count > 0, "No Aircraft agent was found");
        }

        private void CreateCheckPoints()
        {
            Debug.Assert(racePath != null, "Race path was not found");
            Checkpoints = new List<GameObject>();
            int CheckpointNums = (int)racePath.MaxUnit(CinemachinePathBase.PositionUnits.PathUnits);

            for (int i = 0; i < CheckpointNums; i++)
            {
                GameObject checkpoints;

                if (i == CheckpointNums - 1)
                {
                    checkpoints = Instantiate<GameObject>(finishPoinPrefab);
                }
                else
                {
                    checkpoints = Instantiate<GameObject>(checkPointPrefab);
                }

                checkpoints.transform.SetParent(racePath.transform);
                checkpoints.transform.localPosition = racePath.m_Waypoints[i].position;
                checkpoints.transform.rotation = racePath.EvaluateOrientationAtUnit(i, CinemachinePathBase.PositionUnits.PathUnits);

                Checkpoints.Add(checkpoints);
            }
        }

        public void ResetAgentPosition(AircraftAgent agent, bool randomize= false)
        {
            if (AircraftAgents == null) FindAircraftAgents();
            if (Checkpoints == null) CreateCheckPoints();

            if (randomize)
            {
                agent.NextCheckPointIndex = Random.Range(0, Checkpoints.Count);

            }

            int previousCheckPointIndex = agent.NextCheckPointIndex - 1;

            if (previousCheckPointIndex == -1)
                previousCheckPointIndex = Checkpoints.Count -1;

            float startPosition = racePath.FromPathNativeUnits(previousCheckPointIndex, CinemachinePathBase.PositionUnits.PathUnits);

            Vector3 basePosition = racePath.EvaluatePosition(startPosition);

            Quaternion orientation = racePath.EvaluateOrientation(startPosition);

            Vector3 positionOffset = Vector3.right * (AircraftAgents.IndexOf(agent) - AircraftAgents.Count / 2f) * Random.Range(9f, 10f);

            agent.transform.position = basePosition + orientation * positionOffset;
            agent.transform.rotation = orientation;


        }

    }

}
