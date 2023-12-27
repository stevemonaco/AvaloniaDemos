using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace SkiaRendering.ExpandingCircles.ViewModels;

/// <summary>
/// Base class for elements to be rendered
/// Simplifies notifications when a drawable property is changed, though not used for this demo
/// </summary>
public abstract partial class ObservableElement : ObservableObject
{
    [ObservableProperty] private bool _isVisible = true;

    partial void OnIsVisibleChanged(bool value) => NotifyElementInvalidated();

    public bool IsDirty { get; protected set; }

    public void Clean()
    {
        if (!IsDirty)
            return;

        Invalidate();
        IsDirty = false;
    }

    public virtual void Invalidate()
    {
        NotifyElementInvalidated();
    }

    protected void NotifyElementInvalidated()
    {
        WeakReferenceMessenger.Default.Send(Messages.RenderInvalidatedMessage);
    }
}