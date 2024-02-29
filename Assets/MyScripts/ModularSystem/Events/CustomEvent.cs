using System;
using System.Collections;
using UnityEngine;

public abstract class CustomEvent
{
    // When the event was created
    public DateTime Timestamp { get; private set; }

    public string Message { get; protected set; }

    protected CustomEvent(string message = "")
    {
        Timestamp = DateTime.Now;
        Message = message;
    }
}

public abstract class ModuleStateChangedEvent : CustomEvent
{
    public IModule SourceModule { get; private set; }

    public ModuleState NewState { get; private set; }

    public ModuleStateChangedEvent(IModule sourceModule, ModuleState newState, string message = "")
        : base(message)
    {
        SourceModule = sourceModule;
        NewState = newState;
    }
}

    public class BeforeModuleStateChangedEvent : ModuleStateChangedEvent
    {
        public BeforeModuleStateChangedEvent(IModule sourceModule, ModuleState newState, string message = "")
            : base(sourceModule, newState, message)
        {
        }
    }

    public class AfterModuleStateChangedEvent : ModuleStateChangedEvent
    {
        public AfterModuleStateChangedEvent(IModule sourceModule, ModuleState newState, string message = "")
            : base(sourceModule, newState, message)
        {
        }
    }

public class CameraSwitchEvent : CustomEvent
{
    public static event Action<Transform> OnCameraSwitch;

    public static void BroadcastCameraSwitch(Transform newCameraTransform)
    {
        OnCameraSwitch?.Invoke(newCameraTransform);
    }
}

public class WeaponEquippedEvent : CustomEvent
{
    public static event Action<AttackState> OnWeaponEquipped;

    public static void BroadcastWeaponEquipment(AttackState attackState)
    {
        OnWeaponEquipped?.Invoke(attackState);
    }
}

public class WeaponAimedEvent : CustomEvent
{
    public static event Action<AttackState> OnWeaponAimed;

    public static void BroadcastWeaponAim(AttackState attackState)
    {
        OnWeaponAimed?.Invoke(attackState);
    }
}

public class WeaponUnAimedEvent : CustomEvent
{
    public static event Action<AttackState> OnWeaponUnAimed;

    public static void BroadcastWeaponUnAim(AttackState attackState)
    {
        OnWeaponUnAimed?.Invoke(attackState);
    }
}

public class WeaponReloadingEvent : CustomEvent
{
    public static event Action<AttackState> OnWeaponReload;

    public static void BroadcastWeaponReload(AttackState attackState)
    {
        OnWeaponReload?.Invoke(attackState);
    }
}

public class PowerUpPickedUpEvent : CustomEvent
{
    public static event Action<PowerUp> OnPowerUpPickedUp;

    public static void BroadcastPowerUpPickedUp(PowerUp powerUp)
    {
        OnPowerUpPickedUp?.Invoke(powerUp);
    }
}


public class ActivateButtonShotEvent : CustomEvent
{
    public static event Action<string> OnActivateButtonShot;

    public static void BroadcastActivateButtonShot(string name)
    {
        Debug.Log($"Broadcasting Activate Button Shot for: {name}");
        OnActivateButtonShot?.Invoke(name);
    }
}

public class SauronEyeShotEvent : CustomEvent
{
    public static event Action<Transform> OnSauronEyeShot;

    public static void BroadcastSauronEyeShot(Transform hitTransform)
    {

        OnSauronEyeShot?.Invoke(hitTransform);
    }
}





