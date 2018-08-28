﻿namespace ElmishPlanets

open Models
open Styles
open Elmish.XamarinForms
open Elmish.XamarinForms.DynamicViews

module CardPage =
    open Xamarin.Forms

    type Model =
        {
            Planet: Planet
        }

    type Msg = NoMsg

    type ExternalMsg = NoOp

    let init planet =
        {
            Planet = planet
        }, Cmd.none

    let update msg model =
        match msg with
        | NoMsg -> model, Cmd.none, ExternalMsg.NoOp

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
            title=model.Planet.Info.Name,
            content=View.Grid(
                children=[
                    View.UrhoSurface<HelloWorldUrhoApp>(options=View.UrhoApplicationOptions(assetsFolder="Data"))
                    View.StackLayout(
                        padding=20.,
                        children=[
                            View.Label(text=model.Planet.Info.Name).TitleFontSize().WhiteText()
                            mkInfoLabel "Diameter" (kmToString model.Planet.Info.Diameter)
                            mkInfoLabel "Temperature" (celsiusToString model.Planet.Info.Temperature)
                            mkInfoLabel "Speed" (speedToString model.Planet.Info.Speed)
                            mkInfoLabel "Mass" (massToString model.Planet.Info.Mass)
                            mkInfoLabel "Year of Discovery" (intOptionToString "N/A" model.Planet.Info.YearOfDiscovery)
                            View.Label(text=model.Planet.Info.Description, horizontalTextAlignment=TextAlignment.Center, verticalOptions=LayoutOptions.FillAndExpand, verticalTextAlignment=TextAlignment.End).WhiteText()
                        ]
                    )
                ]
            )
        )