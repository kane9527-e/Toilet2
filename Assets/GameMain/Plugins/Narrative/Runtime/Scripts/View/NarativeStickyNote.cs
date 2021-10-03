using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]

public class NarativeStickyNote
{
    public Rect position;
    public string title;
    public StickyNoteTheme theme;
    public string contents;
    public StickyNoteFontSize fontSize;
}