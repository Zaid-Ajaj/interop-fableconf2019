module App

open Fable.Core
open Fable.Core.JsInterop

printfn "Hello from Fable"

[<Emit("1")>]
let one : int = jsNative

[<Emit("console.log($0)")>]
let log (x: 'a) : unit = jsNative

[<Emit("$0 + $1")>]
let add (a: int) (b: int) : int = jsNative

type ICurrencyBuilder = interface end


[<StringEnum>]
type Currency =
    | [<CompiledName "EUR">] Euro
    | [<CompiledName "USD">] Dollar


module Currency =
    [<Emit("new Intl.NumberFormat($0, $1)")>]
    let private  createFormatterInternal (locale: string) (options: obj) : obj  = jsNative
    [<Emit("$2[$0] = $1")>]
    let private set (prop: string) (value: obj) (objectLiteral: obj) : unit = jsNative

    let createFormatter() : ICurrencyBuilder =
        let options = obj()
        options?style <- "currency"
        unbox(options)

    let currency (code: Currency) (builder: ICurrencyBuilder) =
        builder?currency <- code
        builder

    type locale =
        static member netherlands (builder: ICurrencyBuilder) =
            builder?locale <- "nl-NL"
            builder
        static member germany (builder: ICurrencyBuilder) =
            builder?locale <- "de-DE"
            builder

    let format (value: float) (builder: ICurrencyBuilder) : string =
        let formatter = createFormatterInternal (builder?locale) (builder)
        formatter?format(value)

Currency.createFormatter()
|> Currency.currency Euro
|> Currency.locale.germany
|> Currency.format 5000.0
|> log


type IInteropModule =
    abstract someValue : int
    abstract add : int -> int -> int

let interop : IInteropModule = importDefault "./interop.js"

let interopAdd (a: int) (b: int) : int = import "add" "./interop.js"

let addFive = interop.add 5

let filesize (input: int) : string = importDefault "filesize"

filesize 100000
|> log