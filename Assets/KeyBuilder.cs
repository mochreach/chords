using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Klak;

public class KeyBuilder : MonoBehaviour
{
    public float keySpacing = 9000;
    public float xSpacing = 200;
    public float ySpacing = 500;
    public int firstNoteNumber = 24; // Low C on key board
    public int octaves = 6;

    public GameObject canvas;
    public GameObject chordButton;
    public GameObject modeLabel;

    public string[] keys =
        new string[]
        {
            "C",
            "Db",
            "D",
            "Eb",
            "E",
            "F",
            "Gb",
            "G",
            "Ab",
            "A",
            "Bb",
            "B",
        };

    public (string, (string, int[])[])[] modes =
        new (string, (string, int[])[])[]
        {
            ("Ionian", new (string, int[])[] {
                ("I", new int[] {0, 4, 7}),
                ("ii", new int[] {2, 5, 9}),
                ("iii", new int[] {4, 7, 11}),
                ("IV", new int[] {5, 9, 12}),
                ("V", new int[] {7, 11, 14}),
                ("vi", new int[] {9, 12, 16}),
                ("vii°", new int[] {11, 14, 17}),
            }),
            ("Dorian", new (string, int[])[] {
                ("i", new int[] {0, 3, 7}),
                ("ii", new int[] {2, 5, 9}),
                ("III", new int[] {3, 7, 10}),
                ("IV", new int[] {5, 9, 12}),
                ("v", new int[] {7, 10, 14}),
                ("vi°", new int[] {9, 12, 15}),
                ("VII", new int[] {10, 14, 17}),
            }),
            ("Phrygian", new (string, int[])[] {
                ("i", new int[] {0, 3, 7}),
                ("II", new int[] {1, 5, 8}),
                ("III", new int[] {3, 7, 10}),
                ("iv", new int[] {5, 8, 12}),
                ("v°", new int[] {7, 10, 13}),
                ("VI", new int[] {8, 12, 15}),
                ("vii", new int[] {10, 13, 17}),
            }),
            ("Lydian", new (string, int[])[] {
                ("I", new int[] {0, 4, 7}),
                ("II", new int[] {2, 6, 9}),
                ("iii", new int[] {4, 7, 11}),
                ("iv°", new int[] {6, 9, 12}),
                ("V", new int[] {7, 11, 14}),
                ("vi", new int[] {9, 12, 16}),
                ("vii", new int[] {11, 14, 18}),
            }),
            ("Mixolydian", new (string, int[])[] {
                ("I", new int[] {0, 4, 7}),
                ("ii", new int[] {2, 5, 9}),
                ("iii°", new int[] {4, 7, 10}),
                ("IV", new int[] {5, 9, 12}),
                ("v", new int[] {7, 10, 14}),
                ("vi", new int[] {9, 12, 16}),
                ("VII", new int[] {10, 14, 17}),
            }),
            ("Aolian", new (string, int[])[] {
                ("i", new int[] {0, 3, 7}),
                ("ii°", new int[] {2, 5, 8}),
                ("III", new int[] {3, 7, 10}),
                ("iv", new int[] {5, 8, 12}),
                ("v", new int[] {7, 10, 14}),
                ("VI", new int[] {8, 12, 15}),
                ("VII", new int[] {10, 14, 17}),
            }),
            ("Locrian", new (string, int[])[] {
                ("i°", new int[] {0, 3, 6}),
                ("II", new int[] {1, 5, 8}),
                ("iii", new int[] {3, 6, 10}),
                ("iv", new int[] {5, 8, 12}),
                ("V", new int[] {6, 10, 13}),
                ("VI", new int[] {8, 12, 15}),
                ("vii", new int[] {10, 13, 17}),
            }),
        };

    // Start is called before the first frame update
    void Start()
    {
        for (int keyIndex = 0; keyIndex < 1; keyIndex++)
        {
            CreateKey(keys[keyIndex], keyIndex);
        }
        
    }

    void CreateKey(string key, int keyIndex)
    {
        GameObject keyContainer = new GameObject(key);
        keyContainer.transform.SetParent(canvas.transform);

        for (int modeIndex = 0; modeIndex < modes.Length; modeIndex++)
        {
            (string, (string, int[])[]) mode = modes[modeIndex];
            CreateModeKeyboard(mode.Item1, mode.Item2, modeIndex, key, keyIndex, keyContainer);
        }
    }

    void CreateModeKeyboard(string modeName, (string, int[])[] chords, int yPositionIndex, string key, int keyIndex, GameObject keyContainer)
    {
        int xPositionIndex = 0;
        GameObject modeContainer = new GameObject(key + " " + modeName);
        modeContainer.transform.SetParent(keyContainer.transform);

        for (int octaveNumber = 0; octaveNumber < octaves; octaveNumber++)
        {
            int rootNoteNumber = firstNoteNumber + (12 * octaveNumber) + keyIndex;

            GameObject labelInstance = Instantiate(modeLabel, new Vector3((xPositionIndex * xSpacing) + (keySpacing * keyIndex) + 690, -(yPositionIndex * ySpacing), 0), Quaternion.identity);
            labelInstance.GetComponentInChildren<Text>().text = modeName;
            labelInstance.transform.SetParent(modeContainer.transform);

            for (int chordIndex = 0; chordIndex < 7; chordIndex++)
            {
                (string, int[]) chord = chords[chordIndex];
                string chordName = chord.Item1;
                int[] chordNumbers = chord.Item2;

                GameObject buttonInstance = Instantiate(chordButton, new Vector3((xPositionIndex * xSpacing) + (keySpacing * keyIndex), -(yPositionIndex * ySpacing), 0), Quaternion.identity);

                Text chordLabel = buttonInstance.transform.Find("ChordLabel").gameObject.GetComponent<Text>();
                chordLabel.text = chordName;
                if (chordIndex == 0)
                {
                    Text octaveLabel = buttonInstance.transform.Find("OctaveLabel").gameObject.GetComponent<Text>();
                    octaveLabel.enabled = true;
                    octaveLabel.text = string.Format("{0}{1}", key, octaveNumber + 2);
                }

                Klak.Wiring.FloatValue noteOne = buttonInstance.transform.Find("Patch/One").gameObject.GetComponent<Klak.Wiring.FloatValue>();
                Klak.Wiring.FloatValue noteTwo = buttonInstance.transform.Find("Patch/Two").gameObject.GetComponent<Klak.Wiring.FloatValue>();
                Klak.Wiring.FloatValue noteThree = buttonInstance.transform.Find("Patch/Three").gameObject.GetComponent<Klak.Wiring.FloatValue>();
                noteOne.floatValue = chordNumbers[0] + rootNoteNumber;
                noteTwo.floatValue = chordNumbers[1] + rootNoteNumber;
                noteThree.floatValue = chordNumbers[2] + rootNoteNumber;

                buttonInstance.transform.SetParent(modeContainer.transform);

                xPositionIndex += 1;
            }
        }
    }
}
