using UnityEngine;
public class WrestlerController:MonoBehaviour {
 public bool isPlayer; public float moveSpeed=4.2f; public Transform opponent; float cooldown,stun;
 void Update(){cooldown-=Time.deltaTime;stun-=Time.deltaTime;if(stun>0)return;
  if(isPlayer){Vector3 d=new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));if(d.sqrMagnitude>1)d.Normalize();Move(d);moveSpeed=Input.GetKey(KeyCode.LeftShift)?6:4.2f;if(Input.GetKeyDown(KeyCode.J))Punch();if(Input.GetKeyDown(KeyCode.K))Kick();if(Input.GetKeyDown(KeyCode.L))Grapple();}
  else if(opponent){Vector3 d=opponent.position-transform.position;d.y=0;if(d.magnitude>1.65f)Move(d.normalized*.72f);else if(cooldown<=0)Punch();}
  Face();
 }
 void Move(Vector3 d){if(d.sqrMagnitude<.001f)return;transform.position+=d*moveSpeed*Time.deltaTime;transform.position=new Vector3(Mathf.Clamp(transform.position.x,-3.25f,3.25f),0,Mathf.Clamp(transform.position.z,-3.25f,3.25f));}
 void Face(){if(!opponent)return;Vector3 d=opponent.position-transform.position;d.y=0;if(d.sqrMagnitude>.01f)transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(d),Time.deltaTime*9);}
 public void Punch(){Hit(8,1.15f);} public void Kick(){Hit(12,1.45f);} public void Grapple(){Hit(18,1.8f);}
 void Hit(float damage,float range){if(cooldown>0||!opponent)return;cooldown=.65f;if(Vector3.Distance(transform.position,opponent.position)>range)return;var hp=opponent.GetComponent<WrestlerHealth>();if(hp)hp.Damage(damage);}
 public void Block(){var hp=GetComponent<WrestlerHealth>();if(hp)hp.guard=true;CancelInvoke();Invoke(nameof(StopBlock),.45f);}void StopBlock(){var hp=GetComponent<WrestlerHealth>();if(hp)hp.guard=false;}public void Stun(float t){stun=t;}
}
public class WrestlerHealth:MonoBehaviour{public float value=100;public bool guard;public void Damage(float d){value=Mathf.Max(0,value-(guard?d*.2f:d));var c=GetComponent<WrestlerController>();if(c)c.Stun(.35f);if(value<=0)value=100;}}
