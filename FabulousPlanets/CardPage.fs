namespace FabulousPlanets

open Models
open Styles
open Fabulous.Core
open Fabulous.DynamicViews

module CardPage =
    open Xamarin.Forms

    type Model =
        {
            HasAppeared: bool
            Planet: Planet
            IncorrectGuess: bool
        }

    type Msg =
        | PageAppearing
        | UrhoAppCreated of PlanetVisualizerUrhoApp
        | GuessMade of string

    type ExternalMsg = 
        | NoOp
        | PlanetNamed of string

    let loadPlanetModel (app: PlanetVisualizerUrhoApp) (model: Model) =
        app.LoadPlanet(model.Planet)
        None

    let init planet =
        {
            HasAppeared = false
            Planet = planet
            IncorrectGuess = false
        }

    let update msg model =
        match msg with
        | PageAppearing -> { model with HasAppeared = true }, Cmd.none, ExternalMsg.NoOp
        | UrhoAppCreated app -> model, Cmd.ofMsgOption (loadPlanetModel app model), ExternalMsg.NoOp
        | GuessMade text -> 
            let planet = model.Planet.Info.Name
            let correct = (System.String.Compare(text, planet, true) = 0)
            if correct then
                model, Cmd.none, ExternalMsg.PlanetNamed planet
            else
                {model with IncorrectGuess = true}, Cmd.none, ExternalMsg.NoOp

    let mkInfoLabel title text =
        View.StackLayout(
            orientation=StackOrientation.Horizontal,
            children=[
                View.Label(text=title).WhiteText()
                View.Label(text=text, horizontalOptions=LayoutOptions.FillAndExpand, horizontalTextAlignment=TextAlignment.End).WhiteText()
            ]
        )

    let view (model: Model) dispatch =
        View.ContentPage(
            appearing=(fun () -> dispatch PageAppearing),
            title="Unknown Planet",
            backgroundColor=Color.Black,
            content=View.Grid(
                children=[
                    View.UrhoSurface<PlanetVisualizerUrhoApp>(
                        ?options=(match model.HasAppeared with false -> None | true -> Some (View.UrhoApplicationOptions(assetsFolder="Data"))),
                        created=(UrhoAppCreated >> dispatch)
                    )
                    View.StackLayout(
                        padding=20.,
                        //inputTransparent=true,
                        children=[
                            mkInfoLabel "Diameter" (kmToString model.Planet.Info.Diameter)
                            //mkInfoLabel "Temperature" (celsiusToString model.Planet.Info.Temperature)
                            //mkInfoLabel "Speed" (speedToString model.Planet.Info.Speed)
                            mkInfoLabel "Mass" (massToString model.Planet.Info.Mass)
                            mkInfoLabel "Year of Discovery" (intOptionToString "N/A" model.Planet.Info.YearOfDiscovery)
                            View.Entry(placeholder="??????", placeholderColor=Color.White, textColor=Color.White, 
                                  textChanged = (fun args -> dispatch (GuessMade args.NewTextValue)),
                                  completed = (fun text -> dispatch (GuessMade text)),
                                  created=(fun v -> v.Focus() |> ignore)).WhiteText()
                            View.Label((if model.IncorrectGuess then "Try again" else "Guess Planet Name")).WhiteText()
                        ]
                    )
                ]
            )
        )