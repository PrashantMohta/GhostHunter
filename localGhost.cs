
using UnityEngine;
namespace GhostHunter
{
    public class localGhost : MonoBehaviour{
        public string ghostId;
        void Update(){
            var pos = transform.position;
            GhostHunter.Instance.HkmpPipe.SendToAll(0,$"update",$"{ghostId},{pos.x},{pos.y},{pos.z},{transform.localScale.x}",false,true);
        }
        void OnDestroy(){
            GhostHunter.Instance.HkmpPipe.SendToAll(0,$"destroy-{ghostId}","",false,true);
        }
    }
}