
using UnityEngine;
namespace GhostHunter
{
    public class localGhost : MonoBehaviour{
        public string ghostId;
        void Update(){
            var pos = transform.position;
            var center = Utils.getColliderCenter(gameObject);
            GhostHunter.Instance.HkmpPipe.SendToAll(0,$"update",$"{ghostId},{pos.x + center.x},{pos.y + center.y},{pos.z + center.z},{transform.localScale.x}",false,true);
        }
        void OnDestroy(){
            GhostHunter.Instance.HkmpPipe.SendToAll(0,$"destroy-{ghostId}","",false,true);
        }
    }
}