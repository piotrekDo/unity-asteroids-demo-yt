using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UiSoundManipulator : PointerManipulator {

    private SoundEffectHandler m_focusSound;
    private SoundEffectHandler m_submitSound;

    public UiSoundManipulator(SoundEffectHandler focusSound, SoundEffectHandler submitSound) {
        this.m_focusSound = focusSound;
        this.m_submitSound = submitSound;
    }

    protected override void RegisterCallbacksOnTarget() {
        target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    protected override void UnregisterCallbacksFromTarget() {
        target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);

    }
    private void OnPointerDown(PointerDownEvent evt) {
        m_submitSound.Play();
    }

    private void OnPointerEnter(PointerEnterEvent evt) {
        m_focusSound.Play();
    }

}
