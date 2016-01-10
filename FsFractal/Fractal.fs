module Fractal

open System
open System.Drawing
open System.Drawing.Imaging
open System.Numerics
open System.Threading.Tasks
open Bitmaps

type Rect = { XMin: float; XMax: float; YMin: float; YMax: float }

let createImage (imgSize:Size) (rect:Rect) (maxIter:int) (escapeRadius:float) =
    let width, height = imgSize.Width, imgSize.Height
    let inline scaleX x = float x * (rect.XMax - rect.XMin) / float width + rect.XMin
    let inline scaleY y = float y * (rect.YMax - rect.YMin) / float height - rect.YMax

    let inline fractalFunc (z:Complex) (c:Complex) = z * z + c

    let fractalIter z c =
        let rec fractalIter' (z:Complex) c i =
            if z.Magnitude > escapeRadius || i >= maxIter then z, i
            else fractalIter' (fractalFunc z c) c (i + 1)
        fractalIter' z c 0

    let inline resultToColor (z:Complex, i) =
        let mag = float (maxIter - i) / float maxIter
        let angle = z.Phase
        let g = byte (255.0 * mag * (Math.Sin(angle) / 2.0 + 0.5))
        let b = byte (255.0 * mag * (Math.Sin(angle + 2.0 * Math.PI / 3.0) / 2.0 + 0.5))
        let r = byte (255.0 * mag * (Math.Sin(angle + 4.0 * Math.PI / 3.0) / 2.0 + 0.5))
        r, g, b

    let bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb)
    use lockContext = new LockContext(bmp)
    Parallel.For(0, width - 1, (fun x ->
        for y = 0 to height - 1 do
            let z, c = Complex.Zero, Complex(scaleX x, scaleY y)
            let color = fractalIter z c |> resultToColor
            lockContext.SetPixel x y color))
    |> ignore
    bmp
    
