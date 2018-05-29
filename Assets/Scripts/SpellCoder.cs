using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// responsible for connecting Spells with wand codes
// use fixed length codes to ensure that no code is the prefix of another code
public class SpellCoder : MonoBehaviour {

	public const int codeAlphabetLength = 3;
	public const int spellCodeLength = 4;
	private int[] receiverBuffer;
	private Dictionary<string, Spell> spellBook;
	public List<SpellBookEntry> spells = new List<SpellBookEntry>();

	public void Start () {
		receiverBuffer = new int[spellCodeLength];
		ClearReceiverBuffer ();
		PopulateDictionary ();
	}

	public void PopulateDictionary () {
		spellBook = new Dictionary<string, Spell> ();
		foreach (SpellBookEntry entry in spells) {
			spellBook.Add (entry.code, entry.GetSpell());
		}
		// maybe free the list? not too important probably
	}

	public void ClearReceiverBuffer () {
		for (int i = 0; i < spellCodeLength; i++) {
			receiverBuffer [i] = -1;
		}
	}

	public void ReceiveCode (int code) {
		string fullCode = "";

		for (int i = 0; i < spellCodeLength - 1; i++) {
			receiverBuffer [i] = receiverBuffer [i + 1];
			fullCode += receiverBuffer [i].ToString();
		}
		receiverBuffer [spellCodeLength - 1] = code;
		fullCode += code.ToString();

		Debug.Log (code);

		if (AttemptSpell (fullCode)) {
			ClearReceiverBuffer ();
		}
	}

	public bool AttemptSpell (string fullCode) {
		if (spellBook.ContainsKey(fullCode)) {
			spellBook [fullCode].Cast ();
			return true;
		}
		return false;
	}
}

[System.Serializable]
public class SpellBookEntry {
	public static int index = 0;

	public string code = "###";
	public MonoBehaviour spellScript;

	public Spell GetSpell () {
		return (Spell)spellScript;
	}

	public void incrementIndex (int numSpells) {
		index += 1;
		if (index == numSpells) {
			index = 0;
		}
	}
		
}


