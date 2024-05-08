using SkiaSharp;

namespace SkiaSharpConsoleDemo
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // CREATING THE DRAWING SURFACE

            // constract the information describing the desired surface
            var info = new SKImageInfo(512, 512);
            // create the surface using the information
            var surface = SKSurface.Create(info);


            // CLEARING THE SURFACE

            // get the canvas that we can draw on
            var canvas = surface.Canvas;
            // clear the canvas / view
            canvas.Clear(SKColors.White);


            // DRAWING SHAPES

            // create the paint for the filled circle
            var circleFill = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColors.Blue
            };
            // draw the circle fill
            canvas.DrawCircle(100, 100, 40, circleFill);

            // create the paint for the circle border
            var circleBorder = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Red,
                StrokeWidth = 5
            };
            // draw the circle border
            canvas.DrawCircle(100, 100, 40, circleBorder);


            // DRAWING PATHS

            // create the paint for the path
            var pathStroke = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Green,
                StrokeWidth = 5
            };

            // create a path
            var path = new SKPath();
            path.MoveTo(160, 60);
            path.LineTo(240, 140);
            path.MoveTo(240, 60);
            path.LineTo(160, 140);

            // draw the path
            canvas.DrawPath(path, pathStroke);


            // DRAWING TEXT

            // create the paint for the text
            var textPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                Color = SKColors.Orange,
                TextSize = 80
            };
            // draw the text (from the baseline)
            canvas.DrawText("SkiaSharp", 60, 160 + 80, textPaint);


            // ENCODING THE SURFACE

            // create a raster instance of the surface
            var image = surface.Snapshot();
            // encode the image as a PNG
            var data = image.Encode(SKEncodedImageFormat.Png, 100);
            // save the PNG to disk
            using (var stream = File.OpenWrite("output.png"))
            {
                // copy the encoded image into the file stream
                data.SaveTo(stream);
            }
        }
    }
}