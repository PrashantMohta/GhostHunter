
using UnityEngine;
namespace GhostHunter
{
    public class localGhost : MonoBehaviour{
        public string ghostId;
        private string lastUpdate = "";
        void Update(){
            var pos = transform.position;
            var update = $"{ghostId},{pos.x.ToString("0.00")},{pos.y.ToString("0.00")},{pos.z.ToString("0.0")},{transform.localScale.x.ToString("0.0")}";
            if(update != lastUpdate){
                GhostHunter.Instance.HkmpPipe.SendToAll(0,EVENT.UPDATE,update,false,true);
                lastUpdate = update;
            }
        }
        void OnDestroy(){
            GhostHunter.Instance.HkmpPipe.SendToAll(0,EVENT.DESTROY,ghostId,false,true);
        }
    }
}