using UnityEngine;

public class regular_shot : entity
{
    [SerializeField] GameObject mainObject;

    public override void Damage(int dmg) {
        mainObject.transform.GetComponent<entity>().Damage(dmg);
    }
}
