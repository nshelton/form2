using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackballControl : MonoBehaviour {

	[SerializeField]
	public SteamVR_TrackedController m_leftHand;

	[SerializeField]
    public SteamVR_TrackedController m_rightHand;

	public GameObject m_leftHandObject;

	[SerializeField]
    public GameObject m_rightHandObject;


	[SerializeField]
	public GameObject m_targetObject;

	[SerializeField]
	public GameObject m_rotateNode;

	[SerializeField]
	public GameObject m_TranslateNode;

	[SerializeField]
	public GameObject m_scaleNode;

	public bool m_isGripping = false;

	public Vector3 m_startVector;

	private void UpdateTrackball()
	{
		m_TranslateNode.transform.position	= (m_leftHandObject.transform.position + m_rightHandObject.transform.position) / 2f ;

		Vector3 endVector = (m_leftHandObject.transform.position - m_rightHandObject.transform.position);
			
		float currentScale = Vector3.Magnitude(endVector);
		

		m_rotateNode.transform.rotation = Quaternion.FromToRotation(m_startVector, endVector);
		m_scaleNode.transform.localScale = Vector3.one * currentScale / 2f;

	}

	private void StartGrip()
	{
		m_TranslateNode.SetActive(true);
		m_startVector = m_leftHandObject.transform.position - m_rightHandObject.transform.position;
		
		UpdateTrackball();

		m_targetObject.transform.SetParent(m_scaleNode.transform, true);
		m_isGripping = true;
	}

	private void EndGrip()
	{
		m_TranslateNode.SetActive(false);
		m_isGripping = false;
		m_targetObject.transform.SetParent(null, true);
	}

	void GrippedL(object sender, ClickedEventArgs e)
	{
		if (m_rightHand.gripped) StartGrip();
		
	}
	void GrippedR(object sender, ClickedEventArgs e)
	{
		if (m_leftHand.gripped) StartGrip();
	}

	void Ungripped(object sender, ClickedEventArgs e)
	{
		if (m_rightHand.gripped) EndGrip();
	}

	void Start () {
		m_leftHand.Gripped  += GrippedL;
		m_rightHand.Gripped += GrippedR;

		m_leftHand.Ungripped += Ungripped;
		m_rightHand.Ungripped += Ungripped;
	}
	
	void Update () {
		if ( m_isGripping )
		{
			if (!m_leftHand.gripped || !m_rightHand.gripped)
			{
				EndGrip();
				return;
			}

			UpdateTrackball();
		}
	}
}
