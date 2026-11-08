using UnityEngine;
public class MatchManager:MonoBehaviour {
 public void Setup(GameObject p,GameObject e){var pc=p.GetComponent<WrestlerController>();var ec=e.GetComponent<WrestlerController>();pc.opponent=e.transform;ec.opponent=p.transform;p.AddComponent<WrestlerHealth>();e.AddComponent<WrestlerHealth>();}
}
