using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ButtonName : MonoBehaviour
{
    public Text[] buttontexts;
    public string[] buttonnames;

    private int num;

    // Start is called before the first frame update
    private void Start()
    {
        for (var i = 0; i < buttontexts.Length; i++) changename(i);
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void changename(int n)
    {
        buttontexts[n].text = buttonnames[n];
    }
}