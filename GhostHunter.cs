using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using GlobalEnums;
using Modding;
using UnityEngine;
using UnityEngine.SceneManagement;

using Newtonsoft.Json;
using Satchel;
using HkmpPouch;

namespace GhostHunter
{
    public static class EVENT{
       public static string UPDATE = "U";
       public static string DESTROY = "D";
    }
    public class GhostHunter : Mod
    {
        internal static GhostHunter Instance;
        internal HkmpPipe HkmpPipe;
        
        ushort ghostId = 1;
        string currentDirectory = Path.Combine(AssemblyUtils.getCurrentDirectory(),"Ghosts");
        private Dictionary<string,GameObject> Ghosts = new Dictionary<string,GameObject>();

        private Satchel.Animation ghostAnim;
        public override string GetVersion()
        {
            return "0.2";
        }

        public void ExtractAnimation(){
            IoUtils.EnsureDirectory(currentDirectory);
            var GhostHunterPath = Path.Combine(currentDirectory,"ghost.json");
            if(!File.Exists(GhostHunterPath)) {
                AssemblyUtils.ExtractFiles(currentDirectory,(res)=>{
                    if(res.EndsWith("1.png")) {
                        return "1.png";
                    }
                    if(res.EndsWith("2.png")) {
                        return "2.png";
                    }
                    if(res.EndsWith("ghost.json")) {
                        return "ghost.json";
                    }
                    return "";
                });
            }
        }
        public override void Initialize()
        {
            Instance = this;
            this.ExtractAnimation();
            ghostAnim = CustomAnimation.LoadAnimation(Path.Combine(currentDirectory,"ghost.json"));
            HkmpPipe = new HkmpPipe("ghostState",false);
            HkmpPipe.OnRecieve += (_,R) =>{
                var p = R.packet;
                if(p.eventName == EVENT.UPDATE){
                    var ghostData = p.eventData.Split(',');
                    var ghostId = ghostData[0];
                    var ghostPos = new Vector3(
                        float.Parse(ghostData[1]),
                        float.Parse(ghostData[2]),
                        float.Parse(ghostData[3])
                    );
                    var playerGhostId= p.fromPlayer.ToString() + "-" + ghostId;
                    if(!Ghosts.TryGetValue(playerGhostId,out var ghost) || ghost == null){
                        Ghosts[playerGhostId] = newRemoteGhost(p.fromPlayer.ToString(),ghostId);
                        Ghosts[playerGhostId].transform.position = ghostPos;
                    }
                }
            };
            On.HeroController.Start += HeroControllerStart;
        }

        internal static Texture2D LoadTexture(string currentDirectory,string name){
            return TextureUtils.LoadTextureFromFile(Path.Combine(currentDirectory,name));
        }
        internal GameObject newRemoteGhost(string playerId,string ghostId){
            var ghost = new GameObject($"ghost-{playerId}-{ghostId}");
            SpriteRenderer sr = ghost.AddComponent<SpriteRenderer>();

            remoteGhost gs = ghost.AddComponent<remoteGhost>();
            gs.playerId = playerId;
            gs.ghostId = ghostId;
            sr.color = new Color(1f, 1f, 1f, 1.0f);
            ghost.AddComponent<CustomAnimationController>().Init(ghostAnim);
            GhostHunter.Instance.LogDebug($"created remote player {playerId} ghost {gs.ghostId}");
            return ghost;
        }
        internal ushort getNextGhostId(){
            ghostId+=1;
            if(ghostId > 9999){
                ghostId = 1;
            } 
            return ghostId;
        }
        internal void addLocalGhostTracker(GameObject go){
            var localGhost = go.GetAddComponent<localGhost>();
            if(localGhost.ghostId == null || localGhost.ghostId == "0"){
                localGhost.ghostId = getNextGhostId().ToString();
                GhostHunter.Instance.LogDebug($"created local ghost {localGhost.ghostId}");
            }
        }
        internal static Vector3 getColliderCenter(GameObject Parent){
            var collider = Parent.GetComponent<Collider2D>();
            if(collider != null){
                return collider.bounds.center;
            } 
            return new Vector3(0f,0f,0f);
        }

        public void HeroControllerStart(On.HeroController.orig_Start orig,HeroController self){
            orig(self);
            HkmpPipe.startListening();
            ModHooks.HeroUpdateHook += update;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += activeSceneChanged;
            ModHooks.SlashHitHook += OnSlashHit;
            //ModHooks.ColliderCreateHook += colliderCreateHook;
            On.HealthManager.Hit += OnHit;
        }
        private bool isValidForSync(GameObject go){
            return (go.GetComponent<HealthManager>()||go.LocateMyFSM("health_manager_enemy") || go.LocateMyFSM("health_manager") );
        }
        public void colliderCreateHook(GameObject go){
            foreach (Collider2D col in go.GetComponentsInChildren<Collider2D>(true))
            {
                if(isValidForSync(col.gameObject)){
                    addLocalGhostTracker(col.gameObject);
                }
            }
        }
        public void OnHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance){
            orig(self, hitInstance);
            addLocalGhostTracker(self.gameObject);
        }
        public void OnSlashHit( Collider2D col, GameObject gameObject ){
            if(isValidForSync(col.gameObject)){
                addLocalGhostTracker(col.gameObject);
            }
        }
        public void activeSceneChanged(Scene from, Scene to){
            if(!GameManager.instance.IsGameplayScene()) { return; } 
            foreach(var kvp in Ghosts){
                GameObject.Destroy(kvp.Value.gameObject);
            }
        }
        public void update()
        {
            if(!GameManager.instance.IsGameplayScene()) { return; } 
        }

    }

}
