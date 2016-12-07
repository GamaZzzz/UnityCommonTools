using UnityEngine;
using System.Collections;

public class RPG_Animation_CharacterFadeOnly : MonoBehaviour {

    public static RPG_Animation_CharacterFadeOnly instance;


    void Awake() {
        instance = this;
    }
}
