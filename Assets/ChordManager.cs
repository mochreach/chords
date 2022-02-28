using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MidiJack;

public class ChordManager : MonoBehaviour
{
    // {{{ Values
    public (string, int)[] keys =
        new (string, int)[]
        {
            ("C", 0),
            ("Db", 1),
            ("D", 2),
            ("Eb", 3),
            ("E", 4),
            ("F", 5),
            ("Gb", 6),
            ("G", 7),
            ("Ab", 8),
            ("A", 9),
            ("Bb", 10),
            ("B", 11),
        };

    public (string, int)[] octaveNumbers =
        new (string, int)[]
        {
            ("0", 0),
            ("1", 1),
            ("2", 2),
            ("3", 3),
            ("4", 4),
            ("5", 5),
            ("6", 6),
            ("7", 7),
        };

    public (string, (string, int[])[])[] types =
        new (string, (string, int[])[])[]
        {
            ("MAJ", new (string, int[])[] {
                ("I", new int[] {0, 4, 7}),
                ("ii", new int[] {2, 5, 9}),
                ("iii", new int[] {4, 7, 11}),
                ("IV", new int[] {5, 9, 12}),
                ("V", new int[] {7, 11, 14}),
                ("vi", new int[] {9, 12, 16}),
                ("vii°", new int[] {11, 14, 17}),
                ("I", new int[] {12, 16, 19}),
            }),
            ("Dor", new (string, int[])[] {
                ("i", new int[] {0, 3, 7}),
                ("ii", new int[] {2, 5, 9}),
                ("III", new int[] {3, 7, 10}),
                ("IV", new int[] {5, 9, 12}),
                ("v", new int[] {7, 10, 14}),
                ("vi°", new int[] {9, 12, 15}),
                ("VII", new int[] {10, 14, 17}),
                ("i", new int[] {12, 15, 19}),
            }),
            ("Phr", new (string, int[])[] {
                ("i", new int[] {0, 3, 7}),
                ("II", new int[] {1, 5, 8}),
                ("III", new int[] {3, 7, 10}),
                ("iv", new int[] {5, 8, 12}),
                ("v°", new int[] {7, 10, 13}),
                ("VI", new int[] {8, 12, 15}),
                ("vii", new int[] {10, 13, 17}),
                ("i", new int[] {12, 15, 19}),
            }),
            ("Lyd", new (string, int[])[] {
                ("I", new int[] {0, 4, 7}),
                ("II", new int[] {2, 6, 9}),
                ("iii", new int[] {4, 7, 11}),
                ("iv°", new int[] {6, 9, 12}),
                ("V", new int[] {7, 11, 14}),
                ("vi", new int[] {9, 12, 16}),
                ("vii", new int[] {11, 14, 18}),
                ("I", new int[] {12, 16, 19}),
            }),
            ("Mix", new (string, int[])[] {
                ("I", new int[] {0, 4, 7}),
                ("ii", new int[] {2, 5, 9}),
                ("iii°", new int[] {4, 7, 10}),
                ("IV", new int[] {5, 9, 12}),
                ("v", new int[] {7, 10, 14}),
                ("vi", new int[] {9, 12, 16}),
                ("VII", new int[] {10, 14, 17}),
                ("I", new int[] {12, 16, 19}),
            }),
            ("MIN", new (string, int[])[] {
                ("i", new int[] {0, 3, 7}),
                ("ii°", new int[] {2, 5, 8}),
                ("III", new int[] {3, 7, 10}),
                ("iv", new int[] {5, 8, 12}),
                ("v", new int[] {7, 10, 14}),
                ("VI", new int[] {8, 12, 15}),
                ("VII", new int[] {10, 14, 17}),
                ("i", new int[] {12, 15, 19}),
            }),
            ("Loc", new (string, int[])[] {
                ("i°", new int[] {0, 3, 6}),
                ("II", new int[] {1, 5, 8}),
                ("iii", new int[] {3, 6, 10}),
                ("iv", new int[] {5, 8, 12}),
                ("V", new int[] {6, 10, 13}),
                ("VI", new int[] {8, 12, 15}),
                ("vii", new int[] {10, 13, 17}),
                ("i°", new int[] {12, 15, 18}),
            }),
        };

    public (string, int[])[] forms =
        new (string, int[])[]
        {
            ("Root", new int[] {0, 0, 0}),
            ("1st", new int[] {12, 0, 0}),
            ("2nd", new int[] {12, 12, 0}),
        };

    public (string, MidiChannel)[] destinations =
        new (string, MidiChannel)[]
        {
            ("Ch1", MidiChannel.Ch1),
            ("Ch2", MidiChannel.Ch2),
            ("Ch3", MidiChannel.Ch3),
            ("Ch4", MidiChannel.Ch4),
            ("Ch5", MidiChannel.Ch5),
            ("Ch6", MidiChannel.Ch6),
            ("Ch7", MidiChannel.Ch7),
            ("Ch8", MidiChannel.Ch8),
            ("Ch14", MidiChannel.Ch14),
        };
    // }}}
    // {{{ Public Attributes
    public Text keyLabel;
    public Text typeLabel;
    public Text formLabel;
    public Text destinationLabel;

    public bool first = true;

    public Button[] chordButtons;
    // }}}
    // {{{ Private Attributes
    private int selectedKey;
    private int selectedOctave;
    private int selectedType;
    private int selectedForm;
    private int selectedDestination;

    private int keyKnobNumber = 1;
    private int typeKnobNumber = 2;
    private int formKnobNumber = 3;
    private int destinationKnobNumber = 4;
    // }}}

    // Start is called before the first frame update    void Start()
    void Update()
    {
        if (first)
        {
            selectedKey = 0;
            selectedOctave = 4;
            selectedType = 0;
            selectedForm = 0;
            selectedDestination = 7;

            SetButtonLabels();

            keyLabel.text = MakeKeyLabel();
            typeLabel.text = types[selectedForm].Item1;
            formLabel.text = forms[selectedForm].Item1;
            destinationLabel.text = destinations[selectedDestination].Item1;

            MidiMaster.noteOnDelegate += SetKey;
            MidiMaster.knobDelegate += OctaveKnob;
            MidiMaster.knobDelegate += TypeKnob;
            MidiMaster.knobDelegate += FormKnob;
            MidiMaster.knobDelegate += DestinationKnob;

            first = false;
        }
    }

    void SetButtonLabels()
    {
        (string, int[])[] chordType = types[selectedType].Item2;
        for (int i = 0; i < chordType.Length; i++) {
            Text buttonText = chordButtons[i].GetComponentInChildren<Text>();
            buttonText.text = chordType[i].Item1;
        }
    }

    string MakeKeyLabel()
    {
        string label = string.Format("{0}{1}", keys[selectedKey].Item1, octaveNumbers[selectedOctave].Item1);
        return label;
    }

    void SetKey(MidiChannel channel, int note, float velocity)
    {
        if (channel != MidiChannel.Ch16) { return; }
        selectedKey = note % 12;
        keyLabel.text = MakeKeyLabel();
    }

    void OctaveKnob(MidiChannel channel, int knobNumber, float knobValue)
    {
        // Only respond to motion channel
        if (channel != MidiChannel.Ch16) { return; }
        if (knobNumber != keyKnobNumber) { return; }

        selectedOctave = CalculateOptionIndex(knobValue, octaveNumbers.Length);
        keyLabel.text = MakeKeyLabel();
    }

    void TypeKnob(MidiChannel channel, int knobNumber, float knobValue)
    {
        // Only respond to motion channel
        if (channel != MidiChannel.Ch16) { return; }
        if (knobNumber != typeKnobNumber) { return; }

        selectedType = CalculateOptionIndex(knobValue, types.Length);
        typeLabel.text = types[selectedType].Item1;
        SetButtonLabels();
    }

    void FormKnob(MidiChannel channel, int knobNumber, float knobValue)
    {
        // Only respond to motion channel
        if (channel != MidiChannel.Ch16) { return; }
        if (knobNumber != formKnobNumber) { return; }

        selectedForm = CalculateOptionIndex(knobValue, forms.Length);
        formLabel.text = forms[selectedForm].Item1;

    }

    int CalculateOptionIndex(float knobValue, int numOfOptions)
    {
        float optionfraction = 1f / numOfOptions;
        int optionIndex = Mathf.FloorToInt(knobValue / optionfraction);
        if (optionIndex > (numOfOptions - 1))
        {
            optionIndex = numOfOptions- 1;
        }
        return optionIndex;
    }

    void DestinationKnob(MidiChannel channel, int knobNumber, float knobValue)
    {
        // Only respond to motion channel
        if (channel != MidiChannel.Ch16) { return; }
        if (knobNumber != destinationKnobNumber) { return; }
        
        selectedDestination = CalculateOptionIndex(knobValue, destinations.Length);
        destinationLabel.text = destinations[selectedDestination].Item1;

    }

    public void ChordPressed(int buttonNumber)
    {
        int[] notes = types[selectedType].Item2[buttonNumber].Item2;
        int[] form = forms[selectedForm].Item2;
        MidiChannel channel = destinations[selectedDestination].Item2;
        for (int i = 0; i < notes.Length; i++)
        {
            MidiMaster.SendKeyDown(channel, selectedKey + (12 * selectedOctave) + notes[i] + form[i], 1f);
        }
    }
    public void ChordReleased(int buttonNumber)
    {
        int[] notes = types[selectedType].Item2[buttonNumber].Item2;
        int[] form = forms[selectedForm].Item2;
        MidiChannel channel = destinations[selectedDestination].Item2;
        for (int i = 0; i < notes.Length; i++)
            {
                MidiMaster.SendKeyUp(channel, selectedKey + (12 * selectedOctave) + notes[i] + form[i]);
        }
    }
}
