using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Animator[] myanimator;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    public void Idle()
    {
        foreach (var x in myanimator) x.Play("Idle2");
    }

    public void Hitanim()
    {
        foreach (var x in myanimator) x.Play("Hit");
    }

    public void Lookrightanim()
    {
        foreach (var x in myanimator) x.Play("LookRight");
    }

    public void Lookleftanim()
    {
        foreach (var x in myanimator) x.Play("LookLeft");
    }

    public void Rotateleft()
    {
        foreach (var x in myanimator) x.Play("RoatetLeft");
    }

    public void Rotateright()
    {
        foreach (var x in myanimator) x.Play("RotateRight");
    }

    public void run()
    {
        foreach (var x in myanimator) x.Play("Run");
    }

    public void attack()
    {
        foreach (var x in myanimator) x.Play("Attack");
    }

    public void wierd()
    {
        foreach (var x in myanimator) x.Play("Weird");
    }
}