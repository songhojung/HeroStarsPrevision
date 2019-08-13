using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIData
{
    public List<object> m_Datas { get; protected set; }

    public UIData(List<object> _Datas = null)
    {
        m_Datas = _Datas;
    }


}
