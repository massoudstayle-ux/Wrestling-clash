using UnityEngine;
public class MobileControls:MonoBehaviour {
 GUIStyle style;
 void OnGUI(){if(style==null){style=new GUIStyle(GUI.skin.button);style.fontSize=Mathf.Max(18,Screen.width/38);style.fontStyle=FontStyle.Bold;}
 float s=Mathf.Clamp(Screen.width*.14f,76,130), y=Screen.height-s-22;GUI.Box(new Rect(18,y-s-12,s*2.1f,s*2.1f),"MOVE");
 if(GUI.RepeatButton(new Rect(18+s*.7f,y-s,s*.7f,s*.7f),"↑",style))Move(Vector3.forward);
 if(GUI.RepeatButton(new Rect(18+s*.7f,y,s*.7f,s*.7f),"↓",style))Move(Vector3.back);
 if(GUI.RepeatButton(new Rect(18,y,s*.7f,s*.7f),"←",style))Move(Vector3.left);
 if(GUI.RepeatButton(new Rect(18+s*1.4f,y,s*.7f,s*.7f),"→",style))Move(Vector3.right);
 float x=Screen.width-s-20;if(GUI.Button(new Rect(x,y-s*2.1f,s,s*.72f),"PUNCH",style))Act(0);if(GUI.Button(new Rect(x,y-s*1.25f,s,s*.72f),"KICK",style))Act(1);if(GUI.Button(new Rect(x,y-s*.4f,s,s*.72f),"GRAPPLE",style))Act(2);if(GUI.Button(new Rect(x-s*1.1f,y-s*.4f,s,s*.72f),"BLOCK",style))Act(3);
 }
 WrestlerController Player(){foreach(var w in FindObjectsOfType<WrestlerController>())if(w.isPlayer)return w;return null;}
 void Move(Vector3 d){var p=Player();if(p)p.transform.position+=p.transform.TransformDirection(d)*p.moveSpeed*Time.deltaTime;}
 void Act(int a){var p=Player();if(!p)return;if(a==0)p.Punch();else if(a==1)p.Kick();else if(a==2)p.Grapple();else p.Block();}
}
