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
using Satchel.HkmpPipe;

namespace GhostHunter
{
    public static class Utils{
        internal static Vector3 ZeroVector = new Vector3(0,0,0);

        internal static void ExtractFile(string path,string file){
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (string res in asm.GetManifestResourceNames())
            {   
                if(res.EndsWith(file)) {
                    using (Stream s = asm.GetManifestResourceStream(res))
                    {
                            if (s == null) continue;
                            var buffer = new byte[s.Length];
                            s.Read(buffer, 0, buffer.Length);
                            File.WriteAllBytes(Path.Combine(path,file),buffer);
                            s.Dispose();
                    }
                } 
            }
        }
        internal static Vector3 getColliderCenter(this GameObject Parent){
            var collider = Parent.GetComponent<Collider2D>();
            if(collider != null){
                return collider.bounds.center;
            } 
            return ZeroVector;
        }

    }
}