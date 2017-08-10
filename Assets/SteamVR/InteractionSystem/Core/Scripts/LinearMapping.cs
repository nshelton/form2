//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: A linear mapping value that is used by other components
//
//=============================================================================

using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[System.Serializable] public class FloatEvent : UnityEvent<float> {}

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	public class LinearMapping : MonoBehaviour
	{

		[SerializeField]
		private float min = 0;
    	[SerializeField]
		private float max = 1;
    	[SerializeField]
		private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

		[SerializeField]
		private FloatEvent floatTarget;



		private TextMesh m_mesh;		
		private float m_value;

		public float value;

		void Start()
		{
			m_mesh = transform.parent.GetComponentInChildren<TextMesh>();
		}

		void Update()
		{
			m_value = Mathf.Lerp(min, max, curve.Evaluate(value));
			m_mesh.text = string.Format("{0}", m_value);
			
            floatTarget.Invoke(m_value);

		}
	}
}
