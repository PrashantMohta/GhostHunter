
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static GhostHunter.Utils;

namespace GhostHunter
{
    public class localGhost : MonoBehaviour{
        public string ghostId;
        private string lastUpdate = "";
        private Dictionary<string, string> lastUpdateMap = new Dictionary<string, string>();
        private int counter = 0, fullupdatelimit = 30;
        void Awake()
        {
            lastUpdateMap["x"] = "";
            lastUpdateMap["y"] = "";
            lastUpdateMap["z"] = "";
            lastUpdateMap["scalex"] = "";
        }
        void Update(){
            var pos = transform.position;
            var x = Str(pos.x);
            var y = Str(pos.y);
            var z = Str(pos.z);
            var scalex = Str(transform.localScale.x,"0.0");
            var fullupdate = $"{ghostId}|{x}|{y}|{z}|{scalex}";

            var miniupdate = $"{ghostId}|{newOrEmpty(lastUpdateMap["x"],x)}|{newOrEmpty(lastUpdateMap["y"], y)}|{newOrEmpty(lastUpdateMap["z"], z)}|{newOrEmpty(lastUpdateMap["scalex"], scalex)}";

            lastUpdateMap["x"] = x;
            lastUpdateMap["y"] = y;
            lastUpdateMap["z"] = z; 
            lastUpdateMap["scalex"] = scalex;

            if (fullupdate != lastUpdate){
                if (counter < fullupdatelimit)
                {
                    GhostHunter.Instance.HkmpPipe.Broadcast(EVENT.UPDATE, miniupdate, true, false);
                }
                else {
                    GhostHunter.Instance.HkmpPipe.Broadcast(EVENT.UPDATE, fullupdate, true, false);
                    counter = 0;
                }
                counter++;
                lastUpdate = fullupdate;
            }
        }
        void OnDestroy(){
            GhostHunter.Instance.HkmpPipe.Broadcast(EVENT.DESTROY,ghostId);
        }
    }
}