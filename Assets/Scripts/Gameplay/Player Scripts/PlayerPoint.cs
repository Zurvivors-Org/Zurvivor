using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPoint : MonoBehaviour{
    [SerializeField] private long points = 0;
    [SerializeField] private TMP_Text pointText;

    public void AddPoints(long points) {
        this.points += points;
        pointText.SetText(this.points + "");
    }

    public void SubPoints(long points) {
        this.points -= points;
        pointText.SetText(this.points + "");
    }

    public long GetPoints() {
        return points;
    }

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.K)) 
        {
            points += 10000;
        }
	}
}
