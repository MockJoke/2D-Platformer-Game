using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomEditor(typeof(Damager))]
public class DamagerEditor : Editor
{
    static readonly BoxBoundsHandle BoxBoundsHandle = new BoxBoundsHandle();
    static readonly Color EnabledColor = Color.green + Color.grey;

    SerializedProperty DamageProp;
    SerializedProperty OffsetProp;
    SerializedProperty SizeProp;
    SerializedProperty OffsetBasedOnSpriteFacingProp;
    SerializedProperty SpriteRendererProp;
    SerializedProperty CanHitTriggersProp;
    SerializedProperty DisableDamageAfterHitProp;
    SerializedProperty ForceRespawnProp;
    SerializedProperty IgnoreInvincibilityProp;
    SerializedProperty HittableLayersProp;
    SerializedProperty OnDamageableHitProp;
    SerializedProperty OnNonDamageableHitProp;

    void OnEnable ()
    {
        DamageProp = serializedObject.FindProperty ("damage");
        OffsetProp = serializedObject.FindProperty("offset");
        SizeProp = serializedObject.FindProperty("size");
        OffsetBasedOnSpriteFacingProp = serializedObject.FindProperty("offsetBasedOnSpriteFacing");
        SpriteRendererProp = serializedObject.FindProperty("spriteRenderer");
        CanHitTriggersProp = serializedObject.FindProperty("canHitTriggers");
        DisableDamageAfterHitProp = serializedObject.FindProperty("disableDamageAfterHit");
        ForceRespawnProp = serializedObject.FindProperty("forceRespawn");
        IgnoreInvincibilityProp = serializedObject.FindProperty("ignoreInvincibility");
        HittableLayersProp = serializedObject.FindProperty("hittableLayers");
        OnDamageableHitProp = serializedObject.FindProperty("OnDamageableHit");
        OnNonDamageableHitProp = serializedObject.FindProperty("OnNonDamageableHit");
    }

    public override void OnInspectorGUI ()
    {
        serializedObject.Update ();

        EditorGUILayout.PropertyField(DamageProp);
        EditorGUILayout.PropertyField(OffsetProp);
        EditorGUILayout.PropertyField(SizeProp);
        EditorGUILayout.PropertyField(OffsetBasedOnSpriteFacingProp);
        
        if(OffsetBasedOnSpriteFacingProp.boolValue)
            EditorGUILayout.PropertyField(SpriteRendererProp);
        
        EditorGUILayout.PropertyField(CanHitTriggersProp);
        EditorGUILayout.PropertyField(DisableDamageAfterHitProp);
        EditorGUILayout.PropertyField(ForceRespawnProp);
        EditorGUILayout.PropertyField(IgnoreInvincibilityProp);
        EditorGUILayout.PropertyField(HittableLayersProp);
        EditorGUILayout.PropertyField(OnDamageableHitProp);
        EditorGUILayout.PropertyField(OnNonDamageableHitProp);

        serializedObject.ApplyModifiedProperties ();
    }

    void OnSceneGUI ()
    {
        Damager damager = (Damager)target;

        if (!damager.enabled)
            return;

        Matrix4x4 handleMatrix = damager.transform.localToWorldMatrix;
        handleMatrix.SetRow(0, Vector4.Scale(handleMatrix.GetRow(0), new Vector4(1f, 1f, 0f, 1f)));
        handleMatrix.SetRow(1, Vector4.Scale(handleMatrix.GetRow(1), new Vector4(1f, 1f, 0f, 1f)));
        handleMatrix.SetRow(2, new Vector4(0f, 0f, 1f, damager.transform.position.z));
        
        using (new Handles.DrawingScope(handleMatrix))
        {
            BoxBoundsHandle.center = damager.offset;
            BoxBoundsHandle.size = damager.size;

            BoxBoundsHandle.SetColor(EnabledColor);
            EditorGUI.BeginChangeCheck();
            BoxBoundsHandle.DrawHandle();
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(damager, "Modify Damager");

                damager.size = BoxBoundsHandle.size;
                damager.offset = BoxBoundsHandle.center;
            }
        }
    }
}
