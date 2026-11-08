using UnityEngine;
public class WrestlingGameBootstrap : MonoBehaviour {
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
 static void Boot(){ if(!FindObjectOfType<WrestlingGameBootstrap>()) new GameObject("WrestlingGame").AddComponent<WrestlingGameBootstrap>(); }
 void Start(){
  Application.targetFrameRate=60; RenderSettings.ambientLight=new Color(.48f,.48f,.52f);
  var l=new GameObject("ArenaLight"); l.transform.rotation=Quaternion.Euler(50,-30,0); var light=l.AddComponent<Light>(); light.type=LightType.Directional; light.intensity=1.25f;
  Cube("ArenaFloor",new Vector3(0,-.35f,0),new Vector3(18,.5f,18),new Color(.08f,.08f,.1f));
  Cube("RingMat",new Vector3(0,.15f,0),new Vector3(8,.35f,8),new Color(.12f,.25f,.42f));
  foreach(float x in new float[]{-3.8f,3.8f}) foreach(float z in new float[]{-3.8f,3.8f}) Cube("RingPost",new Vector3(x,1.15f,z),new Vector3(.22f,2.3f,.22f),Color.gray);
  for(int i=0;i<3;i++){float y=.65f+i*.48f; Cube("Rope",new Vector3(0,y,3.8f),new Vector3(7.6f,.07f,.07f),Color.white); Cube("Rope",new Vector3(0,y,-3.8f),new Vector3(7.6f,.07f,.07f),Color.white); Cube("Rope",new Vector3(3.8f,y,0),new Vector3(.07f,.07f,7.6f),Color.white); Cube("Rope",new Vector3(-3.8f,y,0),new Vector3(.07f,.07f,7.6f),Color.white);}
  var p=Fighter("PLAYER",new Vector3(-1.25f,0,0),new Color(.12f,.35f,.85f)); var e=Fighter("RIVAL",new Vector3(1.25f,0,0),new Color(.8f,.12f,.12f));
  p.AddComponent<WrestlerController>().isPlayer=true; e.AddComponent<WrestlerController>().isPlayer=false;
  var c=new GameObject("MatchCamera"); var cam=c.AddComponent<Camera>(); cam.tag="MainCamera"; cam.fieldOfView=55; c.AddComponent<MatchCamera>().target=p.transform;
  var m=new GameObject("MatchManager").AddComponent<MatchManager>(); m.Setup(p,e); new GameObject("MobileControls").AddComponent<MobileControls>();
 }
 GameObject Fighter(string n,Vector3 pos,Color color){var root=new GameObject(n);root.transform.position=pos;var b=GameObject.CreatePrimitive(PrimitiveType.Capsule);b.transform.SetParent(root.transform,false);b.transform.localPosition=Vector3.up;b.transform.localScale=new Vector3(.72f,1,.72f);b.GetComponent<Renderer>().material.color=color;var h=GameObject.CreatePrimitive(PrimitiveType.Sphere);h.transform.SetParent(root.transform,false);h.transform.localPosition=Vector3.up*2.05f;h.transform.localScale=Vector3.one*.48f;h.GetComponent<Renderer>().material.color=new Color(.75f,.58f,.43f);root.AddComponent<Rigidbody>().isKinematic=true;root.AddComponent<CapsuleCollider>();return root;}
 void Cube(string n,Vector3 p,Vector3 s,Color color){var o=GameObject.CreatePrimitive(PrimitiveType.Cube);o.name=n;o.transform.position=p;o.transform.localScale=s;o.GetComponent<Renderer>().material.color=color;}
}
public class MatchCamera:MonoBehaviour {public Transform target;void LateUpdate(){if(!target)return;transform.position=Vector3.Lerp(transform.position,target.position+new Vector3(0,6.2f,-10.5f),Time.deltaTime*3);transform.LookAt(target.position+Vector3.up*1.1f);}}
