using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class FileLoader : MonoBehaviour {

    private FileReader reader;
    private FileReader reader1;
    private Text _text;
    // Use this for initialization
    void Start () {
        _text = FindObjectOfType<Text>();
#if UNITY_EDITOR
        string basepath =  Application.dataPath + "/StreamingAssets/QCAR/VRGame.xml";
        string basepath1 = Application.dataPath + "/StreamingAssets/QCAR/Tarmac.xml";

#elif UNITY_IPHONE
	  string basepath = Application.dataPath +"/Raw/";
 
#elif UNITY_ANDROID
	  string basepath = "jar:file://" + Application.dataPath + "!/assets/QCAR/VRGame.xml";
      string basepath1 = "jar:file://" + Application.dataPath + "/StreamingAssets/QCAR/Tarmac.xml";
#endif
        reader = new FileReader();
        reader1 = new FileReader();
        reader1.LoadFile(basepath1);
        reader.LoadFile(basepath);
        StartCoroutine(WaitForLoadFile());
        StartCoroutine(WaitForLoadFile1());
    }
	
	// Update is called once per frame
	IEnumerator WaitForLoadFile () {
        yield return new WaitUntil(() => { return reader.LoadCompleted; });
        _text.text = Encoding.UTF8.GetString(reader.Buffer);
	}

    IEnumerator WaitForLoadFile1()
    {
        yield return new WaitUntil(() => { return reader1.LoadCompleted; });
        _text.text += Encoding.UTF8.GetString(reader1.Buffer);
    }
}
