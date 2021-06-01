using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiserEnemyEscape : StateMachineBehaviour
{
	private CruiserEnemy _cruiserEnemy;
	private Transform _transform;
	private Transform _player;

	private float _targetSpeed = 550;

	private List<Vector3> _waypoints;
	private Vector3 _targetPos;

	private bool _shieldUp = true;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_player = GameObject.FindGameObjectWithTag("Player").transform;

		_cruiserEnemy = animator.gameObject.GetComponent<CruiserEnemy>();
		_transform = animator.gameObject.transform;

		_cruiserEnemy._currentMinTurn = _cruiserEnemy._initialMinTurn;

		_waypoints = _cruiserEnemy.Waypoints;

		_targetPos = _transform.position;

		foreach (Vector3 pos in _waypoints) {
			if (Vector3.Distance(_transform.position, pos) > Vector3.Distance(_transform.position, _targetPos))
			{
				_targetPos = pos;
			}
		}
		_cruiserEnemy._currentMinTurn += Time.deltaTime * 0.01f;
		_cruiserEnemy.TargetPos = _targetPos;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_cruiserEnemy._currentSpeed = Mathf.Lerp(_cruiserEnemy._currentSpeed, _targetSpeed, Time.deltaTime * 0.5f);
		_cruiserEnemy._currentMinTurn += Time.deltaTime * 0.01f;
		_cruiserEnemy.Move(_targetPos);
		_cruiserEnemy.TargetPos = _targetPos;

		if (Vector3.Distance(_transform.position, _targetPos) < _cruiserEnemy.TargetDistanceToPoint)
		{
			animator.SetTrigger("Patrolling");
		}

		if (Vector3.Distance(_transform.position, _player.position) < 300)
		{
			if (_shieldUp == true)
			{
				// Shield Down
				_shieldUp = false;
				_cruiserEnemy.shieldDown();
			}
		}
		else
		{
			if (_shieldUp == false)
			{
				// Shield Up
				_shieldUp = true;
				_cruiserEnemy.shieldUp();
			}
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
