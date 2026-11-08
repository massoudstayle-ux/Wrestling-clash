using System.Collections;
using UnityEngine;

public class PairedMoveController : MonoBehaviour
{
    private WrestlerAnimationPlayer performer, receiver;
    private WrestlerController attacker, defender;
    private bool busy;

    public void Setup(GameObject a, GameObject b)
    {
        attacker=a.GetComponent<WrestlerController>();
        defender=b.GetComponent<WrestlerController>();
        performer=a.GetComponent<WrestlerAnimationPlayer>();
        receiver=b.GetComponent<WrestlerAnimationPlayer>();
    }

    public bool TryPlay(string category,float damage,float range)
    {
        if (busy || attacker==null || defender==null || performer==null || receiver==null) return false;
        if (Vector3.Distance(attacker.transform.position,defender.transform.position)>range) return false;

        string[] words = category=="punch" ? new[]{"punch","jab","strike"}
            : category=="kick" ? new[]{"kick","boot"}
            : new[]{"rko","ddt","suplex","powerbomb","slam","grapple","clinch"};

        string chosen=null;
        foreach(string word in words)
        {
            chosen=performer.FindClipStartingWith("t_",word);
            if(!string.IsNullOrEmpty(chosen) && receiver.HasClip("d_"+chosen.Substring(2))) break;
            chosen=null;
        }
        if(string.IsNullOrEmpty(chosen)) return false;

        string receiverClip="d_"+chosen.Substring(2);
        if(!performer.PlayClip(chosen) || !receiver.PlayClip(receiverClip)) return false;
        float duration=performer.GetClipDuration(chosen);
        float impact=Mathf.Clamp(duration*0.72f,0.12f,1.8f);
        StartCoroutine(Impact(damage,impact));
        return true;
    }

    private IEnumerator Impact(float damage,float delay)
    {
        busy=true;
        yield return new WaitForSeconds(delay);
        if(defender!=null)
        {
            WrestlerHealth hp=defender.GetComponent<WrestlerHealth>();
            if(hp!=null) hp.Damage(damage);
        }
        yield return new WaitForSeconds(0.25f);
        busy=false;
    }
}