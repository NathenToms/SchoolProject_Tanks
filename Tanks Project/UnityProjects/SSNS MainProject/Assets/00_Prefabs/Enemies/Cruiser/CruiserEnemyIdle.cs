using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiserEnemyIdle : StateMachineBehaviour
{
	private CruiserEnemy _cruiserEnemy;
	private Transform _transform;
	private Transform _player;

	private float _targetSpeed = 0;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_player = GameObject.FindGameObjectWithTag("Player").transform;

		_cruiserEnemy = animator.gameObject.GetComponent<CruiserEnemy>();
		_transform = animator.gameObject.transform;

		_cruiserEnemy._currentMinTurn = _cruiserEnemy._initialMinTurn;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_cruiserEnemy._currentSpeed = Mathf.Lerp(_cruiserEnemy._currentSpeed, _targetSpeed, Time.deltaTime * 0.5f);
		_cruiserEnemy.Move(_transform.position);
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

	}
}
