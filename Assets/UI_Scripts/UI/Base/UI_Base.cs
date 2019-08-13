using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Base : MonoBehaviour 
{

    protected UIData BaseData = new UIData();
    public virtual void Set_BaseData(UIData _Data)
    {
        if (_Data != null)
            BaseData = _Data;
    }
    public virtual void set_Open()
    {
    }

    public virtual void set_Close()
    {
    }

    public virtual void set_refresh()
    {
    }
}
