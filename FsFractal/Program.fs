open System
open System.Diagnostics
open System.Windows.Forms
open Fractal

[<Literal>]
let maxIter = 255

[<Literal>]
let escapeRadius = 2.0

let rect = { XMin = -2.0; XMax = 1.0; YMin = -1.0; YMax = 1.0 }

type MainForm() as f =
    inherit Form()
    do
        f.ResizeRedraw <- true

[<STAThread>]
do
    let form = new MainForm(Width=1280, Height=800, Text="FsFractal", StartPosition = FormStartPosition.CenterScreen)
    form.Paint.Add(fun e ->
        form.Text <- "FsFractal - ..."
        let sw = Stopwatch.StartNew()
        let img = createImage form.ClientSize rect maxIter escapeRadius
        e.Graphics.DrawImage(img, 0, 0)
        form.Text <- sprintf "FsFractal - %A" sw.Elapsed)
    Application.Run(form)