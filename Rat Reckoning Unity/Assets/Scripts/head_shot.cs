using UnityEngine;

public class head_shot : entity
{
    [SerializeField] GameObject mainObject;

    public override void Damage(int dmg) {
        mainObject.transform.GetComponent<entity>().Damage(dmg * 2);
    }
}
