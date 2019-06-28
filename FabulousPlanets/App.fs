namespace FabulousPlanets

open Models
open Styles
open Fabulous.Core
open Fabulous.DynamicViews
open Xamarin.Forms.PlatformConfiguration
open Xamarin.Forms.PlatformConfiguration.iOSSpecific
open Xamarin.Forms

module App =

    type Model =
        { CardPageModel: CardPage.Model option
          GuessedNames: string list}

    type Msg =
        | CardPageMsg of CardPage.Msg
        | SelectPlanet of int

    let init () = 
        { CardPageModel = None 
          GuessedNames = []}, Cmd.none

    let update (msg: Msg) (model: Model) =
        match msg with
        | CardPageMsg msg ->
            let m, cmd, externalMsg = CardPage.update msg model.CardPageModel.Value

            let cmd2 = Cmd.none

            let model =
                match externalMsg with
                | CardPage.ExternalMsg.NoOp -> { model with CardPageModel = Some m } 
                | CardPage.ExternalMsg.PlanetNamed name -> { model with GuessedNames = (name :: model.GuessedNames) ; CardPageModel = None }

            model, Cmd.batch [ Cmd.map CardPageMsg cmd; cmd2 ]

        | SelectPlanet i ->
            let cardPageModel = CardPage.init solarObjects.[i]
            { model with CardPageModel = Some cardPageModel }, Cmd.none

    let view (model: Model) dispatch =
        let mainPage =
            View.ContentPage(
                title="Choose a planet",
                backgroundColor=Color.Black,
                content=View.Grid(
                    padding=Thickness(20., 10.),
                    coldefs=[ "*"; "*" ],
                    rowdefs=[ "*"; "*"; "*"; "*" ],
                    children=[
                        for i in 0 .. 1 .. (solarObjects.Length - 1) do
                            let name = solarObjects.[i].Info.Name
                            let nametoshow = if model.GuessedNames |> List.contains name then name else "???"
                            yield View.Grid(
                                padding=10.,
                                verticalOptions=LayoutOptions.Fill,
                                horizontalOptions=LayoutOptions.Fill,
                                gestureRecognizers=[ View.TapGestureRecognizer(command=(fun () -> dispatch (SelectPlanet i))) ],
                                children=[
                                    View.Image(source=solarObjects.[i].Info.Name + ".jpg")
                                    View.Label(text=nametoshow, horizontalTextAlignment=TextAlignment.Center, verticalOptions=LayoutOptions.End).WhiteText()
                                ]
                            ).GridColumn(i % 2)
                             .GridRow(i / 2)
                    ]
                )
            )

        let planetPage =
            match model.CardPageModel with
            | None -> None
            | Some model -> Some (CardPage.view model (CardPageMsg >> dispatch))

        View.NavigationPage(
            appearing=(fun() -> (Xamarin.Forms.Application.Current.MainPage :?> Xamarin.Forms.NavigationPage).On<iOS>().SetPrefersLargeTitles(true) |> ignore),
            barBackgroundColor=Color.Black,
            barTextColor=Color.White,
            backgroundColor=Color.Black,
            pages=[
                yield mainPage
                match planetPage with None -> () | Some p -> yield p
            ]
        )

    let program = Program.mkProgram init update view

type App () as app = 
    inherit Xamarin.Forms.Application ()

    let runner = 
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> Program.runWithDynamicView app

#if DEBUG
    do runner.EnableLiveUpdate()
#endif    