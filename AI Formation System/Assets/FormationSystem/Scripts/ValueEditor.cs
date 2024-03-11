using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ValueEditor : MonoBehaviour
{
    public ValueType type;
    [Header("Int Constraints")]
    [SerializeField] private int imin_val;
    [SerializeField] private int imax_val;
    [SerializeField] private int idefault_value;
    private int ivalue = -1;
    [Header("Float Constraints")]
    [SerializeField] private float fmin_val;
    [SerializeField] private float fmax_val;
    [SerializeField] private float fdefault_value;
    private float fvalue = -1;
    [Header("Bool Constraints")]
    [SerializeField] private bool bdefault_value;
    private bool bvalue = false;
    [Header("References")]
    [SerializeField] private Slider val_slider;
    [SerializeField] private TMP_InputField val_input;
    [SerializeField] private Toggle val_toggle;
    [Space]
    public UnityEvent<int> IntValueChanged;
    public UnityEvent<float> FloatValueChanged;
    public UnityEvent<bool> BoolValueChanged;
    private bool initCheck = false;

    private void Start()
    {
        switch (type)
        {
            case ValueType.Float:
                UpdateFloatValue(fdefault_value);
                break;
            case ValueType.Int:
                UpdateIntValue(idefault_value);
                break;
            case ValueType.Bool:
                UpdateBoolValue(bdefault_value);
                break;
        }

        if (val_slider)
        {
            if (type == ValueType.Float)
            {
                val_slider.minValue = fmin_val;
                val_slider.maxValue = fmax_val;
            }
            else if (type == ValueType.Int)
            {
                val_slider.minValue = imin_val;
                val_slider.maxValue = imax_val;
            }
        }
        initCheck = true;
    }

    public void RefreshValue()
    {
        if (!initCheck) return;
        switch (type)
        {
            case ValueType.Float:
                float prev_fval = fvalue;
                if (val_slider.value != fvalue)
                    UpdateFloatValue(Mathf.Round(val_slider.value * 100) * 0.01f);
                else if (float.Parse(val_input.text) != fvalue)
                    UpdateFloatValue(Mathf.Round(float.Parse(val_input.text) * 100) * 0.01f);
                if (prev_fval != fvalue && prev_fval >= 0)
                    FloatValueChanged.Invoke(fvalue);
                break;
            case ValueType.Int:
                int prev_ival = ivalue;
                if ((int)val_slider.value != ivalue)
                    UpdateIntValue((int)val_slider.value);
                else if (int.Parse(val_input.text) != ivalue)
                    UpdateIntValue(int.Parse(val_input.text));
                if (prev_ival != ivalue && prev_ival >= 0)
                    IntValueChanged.Invoke(ivalue);
                break;
            case ValueType.Bool:
                bool prev_bval = bvalue;
                if (val_toggle.isOn != bvalue)
                    UpdateIntValue((int)val_slider.value);
                if (prev_bval != bvalue)
                    IntValueChanged.Invoke(ivalue);
                break;
        }
    }

    public void UpdateIntValue(int new_val) 
    {
        ivalue = Mathf.Clamp(new_val, imin_val, imax_val);
        if (val_slider)
            val_slider.SetValueWithoutNotify(ivalue);
        if (val_input)
            val_input.SetTextWithoutNotify(ivalue.ToString());
    }

    public void UpdateFloatValue(float new_val) 
    {
        fvalue = Mathf.Clamp(new_val, fmin_val, fmax_val);
        if (val_slider)
            val_slider.SetValueWithoutNotify(fvalue);
        if (val_input)
            val_input.SetTextWithoutNotify(fvalue.ToString());
    }

    public void UpdateBoolValue(bool new_val) 
    {
        bvalue = new_val;
        if (val_toggle)
            val_toggle.SetIsOnWithoutNotify(new_val);
    }
}
