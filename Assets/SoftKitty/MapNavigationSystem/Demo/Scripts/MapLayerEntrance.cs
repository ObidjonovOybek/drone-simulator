using UnityEngine;

namespace SoftKitty.MasterNavigationMap
{
    public class MapLayerEntrance : MonoBehaviour
    {
        public float TeleportDistance = 3F;
        bool teleported = false;
        MapPoint mPoint;
        void Start()
        {
            if (GetComponent<MapPoint>())
            {
                mPoint = GetComponent<MapPoint>();
            } 
            else if (GetComponentInParent<MapPoint>()) {
                mPoint = GetComponentInParent<MapPoint>();
            }
            else if (GetComponentInChildren<MapPoint>(true))
            {
                mPoint = GetComponentInChildren<MapPoint>(true);
            }
        }

         
        void Update()
        {
            if (MapManeger.Player != null)
            {
                if (Vector3.Distance(MapManeger.Player.position, transform.position) < TeleportDistance)
                {
                    if (teleported) return;
                    MapPoint _target = MapManeger.GetLayerCloestEntrance(mPoint.EntranceOfLayer, MapManeger.GetLayerByPosition(transform.position));
                    if (_target != null)
                    {
                        Vector3 _pos = _target.transform.position + _target.transform.forward * 15F;
                        RaycastHit hit;
                        if (Physics.Raycast(_pos + Vector3.up * 30F, Vector3.down, out hit, 20F, MapManeger.CurrentSetting.GroundLayer, QueryTriggerInteraction.Ignore))
                        {
                            _pos.y = hit.point.y + 0.1F;
                        }
                        if (MapManeger.Player.GetComponent<CharacterController>()) MapManeger.Player.GetComponent<CharacterController>().enabled = false;
                        MapManeger.Player.position = _pos;
                        if (MapManeger.Player.GetComponent<CharacterController>()) MapManeger.Player.GetComponent<CharacterController>().enabled = true;
                        teleported = true;
                    }
                }
                else
                {
                    teleported = false;
                }
            }
        }
    }
}
