using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CruiserEnemyAttacking : StateMachineBehaviour
{
	private CruiserEnemy _cruiserEnemy;
	private Transform _transform;
	private Transform _player;

	private float _targetSpeed = 50;
	private float _targetAttackSpeed = 10;
	private float _attackTimer = 0;

	private bool _shieldUp = true;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_player = GameObject.FindGameObjectWithTag("Player").transform;

		_cruiserEnemy = animator.gameObject.GetComponent<CruiserEnemy>();
		_transform = animator.gameObject.transform;

		_cruiserEnemy._currentMinTurn = _cruiserEnemy._initialMinTurn;

		_attackTimer = 0;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Act(animator);
		Reason(animator);
	}

	// Act on the current states logic
	void Act(Animator animator)
	{
		_cruiserEnemy.TargetPos = _player.transform.position;
		_cruiserEnemy.Move(_player.transform.position);

		Vector3 toPosition = (_player.position - _transform.position).normalized;
		float angleToPosition = Vector3.Angle(_transform.forward, toPosition);

		_cruiserEnemy._currentMinTurn += Time.deltaTime * 0.005f;

		if (angleToPosition < 1)
		{
			if ((_attackTimer += Time.deltaTime) > 6)
			{
				_cruiserEnemy.CruiserAttack();
				animator.SetTrigger("Escape");
			}
			else
			{
				_cruiserEnemy._currentSpeed = Mathf.Lerp(_cruiserEnemy._currentSpeed, _targetAttackSpeed, Time.deltaTime * 0.8f);
			}
		}
		else
		{
			_attackTimer = 0.0f;
			_cruiserEnemy._currentSpeed = Mathf.Lerp(_cruiserEnemy._currentSpeed, _targetSpeed, Time.deltaTime * 0.8f);
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

	// Change / Update the ships state
	void Reason(Animator animator)
	{
		if (Vector3.Distance(_transform.position, _player.position) > _cruiserEnemy.EscapeRange)
		{
			animator.SetTrigger("Patrolling");
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

	}
}
