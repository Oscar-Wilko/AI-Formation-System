using Palmmedia.ReportGenerator.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FormationEditor : MonoBehaviour
{
    public FormationType type;

    public UnityEvent<float> SetNoise;
    public UnityEvent<float> SetSpacingX;
    public UnityEvent<float> SetSpacingY;
    public UnityEvent<float> SetNthShift;
    public UnityEvent<float> SetEvenShift;
    public UnityEvent<float> SetSharpness;
    public UnityEvent<int> SetRows;
    public UnityEvent<int> SetIncPerRow;
    public UnityEvent<int> SetSizeX;
    public UnityEvent<int> SetSizeY;
    public UnityEvent<bool> SetHollow;
    public UnityEvent<bool> SetSupplier;

    public DragFormation curDrag;

    private CanvasGroup group;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
        ToggleState(false);
    }

    public void RefreshNoise(float val)
    {
        switch (type)
        {
            case FormationType.Box: curDrag.boxV.noise = val; break;
            case FormationType.Arrow: curDrag.arrV.noise = val; break;
            case FormationType.Triangle: curDrag.triV.noise = val; break;
        }
    }

    public void RefreshSpacingX(float val)
    {
        switch (type)
        {
            case FormationType.Box: curDrag.boxV.spacing.x = val; break;
            case FormationType.Arrow: curDrag.arrV.spacing.x = val; break;
            case FormationType.Triangle: curDrag.triV.spacing.x = val; break;
        }
    }

    public void RefreshSpacingY(float val)
    {
        switch (type)
        {
            case FormationType.Box: curDrag.boxV.spacing.y = val; break;
            case FormationType.Arrow: curDrag.arrV.spacing.y = val; break;
            case FormationType.Triangle: curDrag.triV.spacing.y = val; break;
        }
    }

    public void RefreshNthShift(float val)
    {
        switch (type)
        {
            case FormationType.Box: curDrag.boxV.nthShift = val; break;
            case FormationType.Arrow: curDrag.arrV.nthShift = val; break;
            case FormationType.Triangle: curDrag.triV.nthShift = val; break;
        }
    }

    public void RefreshEvenShift(float val)
    {
        switch (type)
        {
            case FormationType.Box: curDrag.boxV.evenShift = val; break;
            case FormationType.Arrow: curDrag.arrV.evenShift = val; break;
            case FormationType.Triangle: curDrag.triV.evenShift = val; break;
        }
    }

    public void RefreshSharpness(float val)
    {
        curDrag.arrV.sharpness = val;
    }

    public void RefreshRows(int val)
    {
        curDrag.triV.rows = val;
    }

    public void RefreshIncPerRow(int val)
    {
        curDrag.triV.incPerRow = val;
    }

    public void RefreshSizeX(int val)
    {
        switch (type)
        {
            case FormationType.Box: curDrag.boxV.size.x = val; break;
            case FormationType.Arrow: curDrag.arrV.size.x = val; break;
        }
    }

    public void RefreshSizeY(int val)
    {
        switch (type)
        {
            case FormationType.Box: curDrag.boxV.size.y = val; break;
            case FormationType.Arrow: curDrag.arrV.size.y = val; break;
        }
    }
    
    public void RefreshHollow(bool val)
    {
        switch (type)
        {
            case FormationType.Box: curDrag.boxV.hollow = val; break;
            case FormationType.Arrow: curDrag.arrV.hollow = val; break;
            case FormationType.Triangle: curDrag.triV.hollow = val; break;
        }
    }
    
    public void RefreshSupplier(bool val)
    {
        switch (type)
        {
            case FormationType.Box: curDrag.boxV.supplier = val; break;
            case FormationType.Arrow: curDrag.arrV.supplier = val; break;
            case FormationType.Triangle: curDrag.triV.supplier = val; break;
        }
    }

    public void ToggleState(bool state)
    {
        group.alpha = state ? 1 : 0;
        group.blocksRaycasts = state;
        group.interactable = state;
    }

    public void Init(DragFormation dragForm)
    {
        curDrag = dragForm;
        switch (dragForm.type)
        {
            case FormationType.Box:
                InitBox(dragForm.boxV);
                break;
            case FormationType.Arrow:
                InitArr(dragForm.arrV);
                break;
            case FormationType.Triangle:
                InitTri(dragForm.triV);
                break;
        }
    }

    private void InitBox(BoxValues vals)
    {
        SetNoise.Invoke(vals.noise);
        SetSpacingX.Invoke(vals.spacing.x);
        SetSpacingY.Invoke(vals.spacing.y);
        SetNthShift.Invoke(vals.nthShift);
        SetEvenShift.Invoke(vals.evenShift);
        SetSizeX.Invoke(vals.size.x);
        SetSizeY.Invoke(vals.size.y);
        SetHollow.Invoke(vals.hollow);
        SetSupplier.Invoke(vals.supplier);
    }

    private void InitArr(ArrowValues vals)
    {
        SetNoise.Invoke(vals.noise);
        SetSharpness.Invoke(vals.sharpness);
        SetSpacingX.Invoke(vals.spacing.x);
        SetSpacingY.Invoke(vals.spacing.y);
        SetNthShift.Invoke(vals.nthShift);
        SetEvenShift.Invoke(vals.evenShift);
        SetSizeX.Invoke(vals.size.x);
        SetSizeY.Invoke(vals.size.y);
        SetHollow.Invoke(vals.hollow);
        SetSupplier.Invoke(vals.supplier);
    }

    private void InitTri(TriangleValues vals)
    {
        SetNoise.Invoke(vals.noise);
        SetSpacingX.Invoke(vals.spacing.x);
        SetSpacingY.Invoke(vals.spacing.y);
        SetNthShift.Invoke(vals.nthShift);
        SetEvenShift.Invoke(vals.evenShift);
        SetRows.Invoke(vals.rows);
        SetIncPerRow.Invoke(vals.incPerRow);
        SetHollow.Invoke(vals.hollow);
        SetSupplier.Invoke(vals.supplier);
    }
}
