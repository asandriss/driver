module StockAnalyzer
    open System.Net
    open System.IO

    let internal loadPrices ticker = async {
        let url = "http://chart.finance.yahoo.com/table.csv?s=" + ticker + "&a=6&b=22&c=2016&d=7&e=22&f=2016&g=d&ignore=.csv"

        let req = WebRequest.Create(url)
        let! resp = req.AsyncGetResponse()
        let stream = resp.GetResponseStream()
        let reader = new StreamReader(stream)
        let! csv = reader.AsyncReadToEnd()

        let prices =
            csv.Split([|'\n'|])
            |> Seq.skip 1 // skip the first line that is the header in the file
            |> Seq.map (fun line -> line.Split([|','|]))   // split the lines on comma
            |> Seq.filter(fun values -> values |> Seq.length = 7)   // only get the lines that have length of 7 values
            |> Seq.map (fun values -> 
                System.DateTime.Parse(values.[0]), 
                float values.[6])   // convert the first value to datetime and add the the last (seventh) value in tuple
        return prices }

    type StockAnalyzer (lprices, days) = 
        let prices =
            lprices 
            |> Seq.map snd // only return the second element from the tuple # same as fun (_, p) -> p
            |> Seq.take days

        static member GetAnalyzers(tickers, days) =
            tickers
            |> Seq.map loadPrices
            |> Async.Parallel
            |> Async.RunSynchronously
            |> Seq.map (fun prices -> new StockAnalyzer(prices, days))

        member s.Return = 
            let lastPrice = prices |> Seq.nth 0
            let startPrice = prices |> Seq.nth (days-1)

            lastPrice / startPrice - 1.

        member s.StdDev =
            let logRets =
                prices
                |> Seq.pairwise         // group elements in tuples (0,1),(1,2),(2,3) etc.
                |> Seq.map (fun(x,y) -> log(x/y))   // calculate logarithm of daily prices
            let mean = logRets |> Seq.average
            let sqr x = x*x
            let var = logRets |> Seq.averageBy (fun r -> sqr(r-mean))
        
            sqrt var