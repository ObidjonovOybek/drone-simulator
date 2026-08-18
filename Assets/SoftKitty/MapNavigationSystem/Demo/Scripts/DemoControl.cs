using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace SoftKitty.MasterNavigationMap
{
    public class DemoControl : MonoBehaviour
    {
        public Transform Player;
        public Transform TestTarget;
        public GameObject WorldMap;
        public MapPoint QuestNPC;

        private void Awake()
        {
            MapManeger.SetPlayer(Player);
        }
        void Start()
        {
            Application.targetFrameRate = 60;
            QuestNPC.State = 1;
        }

         
        void Update()
        {
            if (InputProxy.GetKeyDown(KeyCode.Tab))WorldMap.SetActive(!WorldMap.activeSelf);
        }

    }
}
