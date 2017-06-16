using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;
[CustomEditor(typeof(AnimationClip))]  
public class MyTest : DecoratorEditor  
{  
    public MyTest(): base("AnimationClipEditor"){}  
    private FieldInfo previewObject = null;
    public override void OnInspectorGUI ()  
    {  
        base.OnInspectorGUI ();  
        AnimationClip clip = target as AnimationClip;
        string path = AssetDatabase.GetAssetPath(clip);
        string savePath = Path.GetDirectoryName(path);
        string fileName = Path.GetFileNameWithoutExtension(path);
        object tmp = base.GetValue("m_AvatarPreview");
        if (previewObject == null)
        {
            var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;  
            previewObject = tmp.GetType().GetField("m_PreviewInstance",flags);
        }
        GameObject targetPreview = previewObject.GetValue(tmp) as GameObject;
        if(GUILayout.Button("sample this animation"))  
        {  
            //sample frame 0
            clip.SampleAnimation(targetPreview, 0f);
            SkinnedMeshRenderer[] render = targetPreview.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int i = 0; i < render.Length; ++i)
            {
                Mesh inst = new Mesh();
                render[i].BakeMesh(inst);
                string fullpath = savePath + '/'+fileName + render[i].name + '0'+".asset";
                AssetDatabase.CreateAsset(inst, fullpath);
            }
            float timeLen = clip.length;
            int frame = (int)(timeLen * 30f);
            for (int i = 1; i < frame; ++i)
            {
                clip.SampleAnimation(targetPreview, i*1/30f);
                for (int j = 0; j < render.Length; ++j)
                {
                    Mesh inst = new Mesh();
                    render[j].BakeMesh(inst);
                    string fullpath = savePath + '/'+fileName + render[j].name + i+".asset";
                    AssetDatabase.CreateAsset(inst, fullpath);
                }
            }


            Debug.Log("Adding this button");  
        }  
    }  
}  
