using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GunController))]
public class GunControllerEditor : ExtendedEditor
{
	GunController _gunController;

	private void OnEnable()
	{
		if (target.GetType() == typeof(GunController))
		{
			_gunController = (GunController)target;
		}
	}

	public override void OnInspectorGUI()
	{
		if (_gunController != null)
		{
			GUILayout.Space(10);

			SerializedObject GunControllerObject =  new SerializedObject(_gunController);

			SerializedProperty ammoCount = GunControllerObject.FindProperty("ammoCount");
			SerializedProperty ammoArray = ammoCount.FindPropertyRelative("ammo");

			DrawStandardShot(ammoArray);

			DrawEnergyShot(ammoArray);

			DrawMissileShot(ammoArray);

			DrawChargeShot(ammoArray);

			DrawLaserShot(ammoArray);

			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Animators", EditorStyles.boldLabel);
			DrawField("gunAnimator");
			DrawField("swapAnimator");
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Projectiles", EditorStyles.boldLabel);
			DrawField("standardShot");
			DrawField("energyShot");
			DrawField("missileShot");
			DrawField("chargedShot");
			GUILayout.Space(5);
			DrawField("laserShot");
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Ship Controller", EditorStyles.boldLabel);
			DrawField("_shipController");
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Barrels", EditorStyles.boldLabel);
			DrawField("barrelL");
			DrawField("barrelR");
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Shield Projector", EditorStyles.boldLabel);
			DrawField("shieldProjector");
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical("box");
			GUILayout.Label("Gun States", EditorStyles.boldLabel);
			DrawField("CanAttack");
			DrawField("CanSwap");
			EditorGUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();
			GunControllerObject.ApplyModifiedProperties();
		}
	}

	void DrawStandardShot(SerializedProperty ammoArray)
	{
		EditorGUILayout.BeginVertical("box");

		SerializedObject Damage = new SerializedObject(_gunController.standardShot.GetComponent<Damage>());

		GUILayout.Label("Standard Shot", EditorStyles.boldLabel);

		DrawField("FireRateStandard");

		GUILayout.Space(5);
		SerializedProperty ammoProperty = ammoArray.GetArrayElementAtIndex((int)SNSSTypes.WeaponType.Regular);
		EditorGUILayout.PropertyField(ammoProperty);

		GUILayout.Space(5);
		EditorGUILayout.PropertyField(Damage.FindProperty("kineticDamage"));
		EditorGUILayout.PropertyField(Damage.FindProperty("energyDamage"));

		Damage.ApplyModifiedProperties();

		EditorGUILayout.EndVertical();
	}

	void DrawEnergyShot(SerializedProperty ammoArray)
	{
		EditorGUILayout.BeginVertical("box");

		SerializedObject Damage = new SerializedObject(_gunController.energyShot.GetComponent<Damage>());

		GUILayout.Label("Energy Shot", EditorStyles.boldLabel);

		DrawField("FireRateEnergy");

		GUILayout.Space(5);
		EditorGUILayout.PropertyField(ammoArray.GetArrayElementAtIndex((int)SNSSTypes.WeaponType.Energy));

		GUILayout.Space(5);
		EditorGUILayout.PropertyField(Damage.FindProperty("kineticDamage"));
		EditorGUILayout.PropertyField(Damage.FindProperty("energyDamage"));

		Damage.ApplyModifiedProperties();

		EditorGUILayout.EndVertical();
	}

	void DrawMissileShot(SerializedProperty ammoArray)
	{
		EditorGUILayout.BeginVertical("box");

		SerializedObject Damage = new SerializedObject(_gunController.missileShot.GetComponent<ExplosionDamage>());

		GUILayout.Label("Missile Shot", EditorStyles.boldLabel);

		DrawField("FireRateMissile");

		GUILayout.Space(5);
		EditorGUILayout.PropertyField(ammoArray.GetArrayElementAtIndex((int)SNSSTypes.WeaponType.Missiles));

		GUILayout.Space(5);
		EditorGUILayout.PropertyField(Damage.FindProperty("kineticDamage"));
		EditorGUILayout.PropertyField(Damage.FindProperty("energyDamage"));

		GUILayout.Space(5);
		EditorGUILayout.PropertyField(Damage.FindProperty("radius"));

		Damage.ApplyModifiedProperties();

		EditorGUILayout.EndVertical();
	}

	void DrawChargeShot(SerializedProperty ammoArray)
	{
		EditorGUILayout.BeginVertical("box");

		SerializedObject Damage = new SerializedObject(_gunController.chargedShot.GetComponent<ChargedShotBehaviour>());

		GUILayout.Label("Charge Shot", EditorStyles.boldLabel);

		DrawField("FireRateCharge");

		GUILayout.Space(5);
		EditorGUILayout.PropertyField(ammoArray.GetArrayElementAtIndex((int)SNSSTypes.WeaponType.Charged));

		GUILayout.Space(5);
		EditorGUILayout.PropertyField(Damage.FindProperty("minDamage"));
		EditorGUILayout.PropertyField(Damage.FindProperty("maxDamage"));

		GUILayout.Space(5);
		EditorGUILayout.PropertyField(Damage.FindProperty("minScale"));
		EditorGUILayout.PropertyField(Damage.FindProperty("maxScale"));

		Damage.ApplyModifiedProperties();

		EditorGUILayout.EndVertical();
	}

	void DrawLaserShot(SerializedProperty ammoArray)
	{
		EditorGUILayout.BeginVertical("box");

		GUILayout.Label("Laser Shot", EditorStyles.boldLabel);

		DrawField("FireRateLaser");

		EditorGUILayout.PropertyField(ammoArray.GetArrayElementAtIndex((int)SNSSTypes.WeaponType.Laser));
		EditorGUILayout.HelpBox("This only affects  the speed of the animation", MessageType.Info);

		EditorGUILayout.EndVertical();
	}

}
