using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUITextControl : MonoBehaviour {

	private TextMesh  m_textMesh;

	[SerializeField]
	private MultibufferParticles m_target;

	[SerializeField]
	private SteamVR_TrackedController controller0;
	[SerializeField]
	private SteamVR_TrackedController controller1;

	void Save(object sender, ClickedEventArgs e)
	{
		float scale = m_target.transform.localToWorldMatrix.GetScale().x;
		string s = string.Format("$$$$$$ {0}, {1}, {2}, {3}", scale, m_target.AO1, m_target.AO2, m_target.AO3);
		Debug.Log(s);
	}

	void Start () {
		m_textMesh = GetComponent<TextMesh>();

		controller0.PadClicked += Save;
		controller1.PadClicked += Save;

	}

	void Update () {
		float scale = m_target.transform.localToWorldMatrix.GetScale().x;
		m_textMesh.text = string.Format("scale : {0}", scale);

	}


}
