using System.Collections;
using UnityEngine;

public class PairedMoveController : MonoBehaviour
{
    private static readonly string[] PunchMoves = { "p1", "p2", "p3", "p4" };
    private static readonly string[] KickMoves = { "k1", "k2", "k3", "k4" };
    private static readonly string[] GrappleMoves =
    {
        "t_ddt", "t_suplex", "t_slam", "t_powerbomb",
        "t_piledriver", "t_tiger", "t_powerslam", "t_emerald",
        "t_electric", "t_burning", "t_ribbreaker", "t_tombstone", "t_aa"
    };

    private WrestlerAnimationPlayer performer;
    private WrestlerAnimationPlayer receiver;
    private WrestlerController attacker;
    private WrestlerController defender;

    private bool busy;
    private int punchIndex;
    private int kickIndex;
    private int grappleIndex;
    private int reactionIndex;

    public void Setup(GameObject a, GameObject b)
    {
        attacker = a.GetComponent<WrestlerController>();
        defender = b.GetComponent<WrestlerController>();
        performer = a.GetComponent<WrestlerAnimationPlayer>();
        receiver = b.GetComponent<WrestlerAnimationPlayer>();
    }

    public bool TryPlay(string category, float damage, float range)
    {
        if (busy || attacker == null || defender == null ||
            performer == null || receiver == null)
            return false;

        if (Vector3.Distance(attacker.transform.position, defender.transform.position) > range)
            return false;

        if (category == "punch")
            return TryPlayStrike(PunchMoves, ref punchIndex, damage);
        if (category == "kick")
            return TryPlayStrike(KickMoves, ref kickIndex, damage);
        if (category == "grapple")
            return TryPlayGrapple(damage);

        return false;
    }

    private bool TryPlayStrike(
        string[] moves, ref int nextIndex, float damage)
    {
        string chosen = FindNextAvailable(moves, ref nextIndex, false);
        if (string.IsNullOrEmpty(chosen) || !receiver.HasClip("hit_a"))
            return false;

        string reaction = (reactionIndex++ % 2 == 0 && receiver.HasClip("hit_a"))
            ? "hit_a"
            : (receiver.HasClip("hit_b") ? "hit_b" : "hit_a");

        if (!performer.PlayClip(chosen) || !receiver.PlayClip(reaction))
            return false;

        float attackerDuration = performer.GetClipDuration(chosen);
        float receiverDuration = receiver.GetClipDuration(reaction);
        float impactDelay = Mathf.Clamp(attackerDuration * impactFraction, 0.12f, 1.0f);
        float totalDuration = Mathf.Max(attackerDuration, receiverDuration);

        busy = true;
        StartCoroutine(ResolveMove(damage, impactDelay, totalDuration));
        return true;
    }

    private bool TryPlayGrapple(float damage)
    {
        string chosen = FindNextAvailable(GrappleMoves, ref grappleIndex, true);
        if (string.IsNullOrEmpty(chosen))
            return false;

        string receiverClip = "d_" + chosen.Substring(2);
        if (!receiver.HasClip(receiverClip))
            return false;

        if (!performer.PlayClip(chosen) || !receiver.PlayClip(receiverClip))
            return false;

        float attackerDuration = performer.GetClipDuration(chosen);
        float receiverDuration = receiver.GetClipDuration(receiverClip);
        float totalDuration = Mathf.Max(attackerDuration, receiverDuration);
        // Prefer the exact impact timestamp stored in the source animation metadata.
        float impactDelay = performer.GetClipImpactTime(chosen);
        if (impactDelay < 0f)
            impactDelay = receiver.GetClipImpactTime(receiverClip);
        if (impactDelay < 0f)
            impactDelay = Mathf.Min(attackerDuration, receiverDuration) * 0.78f;
        impactDelay = Mathf.Clamp(
            impactDelay, 0.1f, Mathf.Max(0.1f, totalDuration - 0.05f));

        busy = true;
        StartCoroutine(ResolveMove(damage, impactDelay, totalDuration));
        return true;
    }

    private string FindNextAvailable(string[] candidates, ref int nextIndex, bool paired)
    {
        if (candidates == null || candidates.Length == 0)
            return null;

        for (int offset = 0; offset < candidates.Length; offset++)
        {
            int index = (nextIndex + offset) % candidates.Length;
            string candidate = candidates[index];
            if (!performer.HasClip(candidate))
                continue;

            if (paired && !receiver.HasClip("d_" + candidate.Substring(2)))
                continue;

            nextIndex = (index + 1) % candidates.Length;
            return candidate;
        }

        return null;
    }

    private IEnumerator ResolveMove(float damage, float impactDelay, float totalDuration)
    {
        yield return new WaitForSeconds(impactDelay);

        if (defender != null)
        {
            WrestlerHealth hp = defender.GetComponent<WrestlerHealth>();
            if (hp != null)
                hp.Damage(damage);
        }

        float recovery = Mathf.Max(0.15f, totalDuration - impactDelay);
        yield return new WaitForSeconds(recovery);
        busy = false;
    }
}
