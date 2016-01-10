module Bitmaps

open System
open System.Drawing
open System.Drawing.Imaging
open Microsoft.FSharp.NativeInterop

#nowarn "9"
type LockContext(bmp:Bitmap) =

    do if bmp.PixelFormat <> PixelFormat.Format24bppRgb then failwith "Supported only 24bpp format."

    let rect = new Rectangle(0, 0, bmp.Width, bmp.Height)
    let bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat)

    let getPixelAddress x y =
        NativePtr.add<byte> (NativePtr.ofNativeInt bmpData.Scan0) (y * bmpData.Stride + x * 3)

    member this.GetPixel x y =
        let address = getPixelAddress x y
        (NativePtr.get address 2, NativePtr.get address 1, NativePtr.read address)

    member this.SetPixel x y (r, g, b) =
        let address = getPixelAddress x y
        NativePtr.set address 2 r
        NativePtr.set address 1 g
        NativePtr.write address b

    interface IDisposable with
        member this.Dispose() = bmp.UnlockBits(bmpData)


