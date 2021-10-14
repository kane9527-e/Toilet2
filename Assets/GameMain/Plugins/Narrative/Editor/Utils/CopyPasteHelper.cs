using System;
using System.Collections.Generic;

[Serializable]
public class CopyPasteHelper
{
    public List<JsonElement> nodeclipboard = new List<JsonElement>();
    public List<JsonElement> groupclipboard = new List<JsonElement>();
    public List<JsonElement> stickyNoteclipboard = new List<JsonElement>();
}