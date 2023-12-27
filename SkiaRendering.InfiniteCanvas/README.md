InfiniteCanvas is a control with the following feature set:

* Direct use of SkiaSharp drawing APIs
* Automatically resizes surface to bounds
* RenderTrigger for Continuous and on-Invalidation rendering
* Panning and Zooming with constraints

Areas to improve:
* Use of WriteableBitmap should allow oversized dimensions so that it's not continuously recreated upon resize events. Performance is still ok on 1080p though.
* Allow use of `ICustomDrawOperation`.

InfiniteCanvas is provided for educational purposes and not intended for direct production use.