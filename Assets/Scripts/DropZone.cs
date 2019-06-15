using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO.Ports;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class DropZone : MonoBehaviour, IDropHandler{

    public Dictionary<int, string> dict = new Dictionary<int, string>() {
            {48, "0"},  // C
            {50, "1"},  // D
            {52, "2"},  // E
            {53, "3"},  // F
            {55, "4"},  // G
            {57, "5"},  // A
            {59, "6"},  // B
            {60, "7"},  // C
            {62, "8"},  // D
            {64, "9"},  // E
         def sumOfTheDigits(q):
    # Write your code here
    for idx in q:
        digit = 1
        val = [1]
        newval = []
        for i in range(idx-1):
            count = 0
            for j in range(len(val)):
                if val[j] == digit:
                    count += 1
                else:
                    newval.append(count)
                    newval.append(digit)
                    digit = val[j]
                    count = 0
            print(newval)
            val = newval
            newval = []
   {65, "a"},  // F
            {67, "b"},  // G
            {69, "c"},  // A
            {71, "d"},  // B
            {72, "e"}   // C
            };

    public List<Draggable> CdList;

    public string title;
    public Draggable d;
    public GameObject scroll;
    private bool canRotatePlayer = false;
    public SerialPort stream;

    void Start(){
        stream = new SerialPort("COM3", 9600);
        stream.Open();
    }

    public void OnDrop(PointerEventData eventData){
        d = eventData.pointerDrag.GetComponent<Draggable>();
        if(d != null){
            d.parentToReturnTo = this.transform;
        }
        canRotatePlayer = true;
        title = d.gameObject.name;
        print("Playing Track: " + title);
        StartCoroutine(SendArduino());
        scroll.GetComponent<ScrollRect>().enabled = false;
        foreach (Draggable cd in CdList){
            if (cd.name != title){
                cd.draggable = false;
            }
        }
    }


    void Update(){
         RotatePlayer();
     }

    private IEnumerator ReturnPlayer(float waitTime){
        yield return new WaitForSeconds(waitTime);
        canRotatePlayer = false;
        scroll.GetComponent<ScrollRect>().enabled = true;
        d.transform.SetParent(d.parent);
        d.parentToReturnTo = d.parent;
        d = null;
        foreach (Draggable cd in CdList){
            if (cd.name != title){
                cd.draggable = true;
            }
        }
    }

    private IEnumerator SendArduino(){
        yield return new WaitForSeconds(1.0f);  //wait awhile before playing song
        WaitForSeconds wait = new WaitForSeconds(0.5f);
        string[] lines = d.TextFile.text.Split('\n');
        foreach (string line in lines){
            if(!string.IsNullOrWhiteSpace(line)){    // beat contains notes
                print(line);
                foreach(string note in Regex.Split(line, " ")){
                    if(stream.IsOpen){
                        int key = int.Parse(note);
                        print("sending " + key + ": " + dict[key]);
                        stream.Write(dict[key]);
                    }
                }   
            }
            yield return wait; //tempo of song
        }
        StartCoroutine(ReturnPlayer(1.0f));
    }

    protected void RotatePlayer(){
        if(canRotatePlayer){
            d.transform.Rotate(Vector3.forward, -90.0f * Time.deltaTime);
        }
    }
}