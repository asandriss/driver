#light

open System.Net
open System.IO

let loadPrices ticker =
    let url = "http://chart.finance.yahoo.com/table.csv?s=" + ticker + "&a=6&b=22&c=2016&d=7&e=22&f=2016&g=d&ignore=.csv"

    let req = WebRequest.Create(url)
    let resp = req.GetResponse()
    let stream = resp.GetResponseStream()
    let reader = new StreamReader(stream)
    let csv = reader.ReadToEnd()

    let prices =
        csv.Split([|'\n'|])
        |> Seq.skip 1 // skip the first line that is the header in the file
        |> Seq.map (fun line -> line.Split([|','|]))   // split the lines on comma
        |> Seq.filter(fun values -> values |> Seq.length = 7)   // only get the lines that have length of 7 values
        |> Seq.map (fun values -> 
            System.DateTime.Parse(values.[0]), 
            float values.[6])   // convert the first value to datetime and add the the last (seventh) value in tuple
    prices
