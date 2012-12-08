using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JC2.Save
{
    internal interface IContainsSavedObjectInfo
    {
        List<SavedObjectInfo> GetSavedObjectInfo(Dictionary<string, int> countsbycategory);
    }
}
