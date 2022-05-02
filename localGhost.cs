
using UnityEngine;
using System.Globalization;
namespace GhostHunter
{
    public class localGhost : MonoBehaviour{
        public string ghostId;
        private string lastUpdate = "";
        void Update(){
            var pos = transform.position;
            var x = pos.x.ToString("0.00",CultureInfo.InvariantCulture);
            var y = pos.y.ToString("0.00",CultureInfo.InvariantCulture);
            var z = pos.z.ToString("0.00",CultureInfo.InvariantCulture);
            var scalex = transform.localScale.x.ToString("0.0",CultureInfo.InvariantCulture);
            var update = $"{ghostId}|{x}|{y}|{z}|{scalex}";
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