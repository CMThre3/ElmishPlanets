namespace FabulousPlanets.Tests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open FabulousPlanets

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.``initial model is none`` () =
        let model,_ = App.init ()
        Assert.IsTrue(model.CardPageModel.IsNone)

    [<TestMethod>]
    member this.``planet model is some`` () =
        let model,_ = App.init ()
        let model2,_ = App.update (App.SelectPlanet 4) model
        Assert.IsTrue(model2.CardPageModel.IsSome)

    [<TestMethod>]
    member this.``Guessed names empty for initial model`` () =
        let model,_ = App.init ()
        Assert.IsTrue(model.GuessedNames.IsEmpty)

    [<TestMethod>]
    member this.``Selected planet name is empty`` () =
        let model,_ = App.init ()
        let model2,_ = App.update (App.SelectPlanet 4) model
        Assert.IsTrue(model2.GuessedNames.IsEmpty)

    [<TestMethod>]
    member this.``a bad guess is made`` () =
        let model,_ = App.init ()
        let model2,_ = App.update (App.SelectPlanet 4) model
        let model3,_ = App.update (App.CardPageMsg (CardPage.GuessMade "quack")) model2
        Assert.IsTrue(model3.GuessedNames.IsEmpty)
        Assert.IsTrue(model3.CardPageModel.IsSome)

    [<TestMethod>]
    member this.``a good guess is made`` () =
        let model,_ = App.init ()
        let model2,_ = App.update (App.SelectPlanet 4) model
        let model3,_ = App.update (App.CardPageMsg (CardPage.GuessMade "jupiter")) model2
        Assert.IsFalse(model3.GuessedNames.IsEmpty)
        Assert.IsFalse(model3.CardPageModel.IsSome)

    [<TestMethod>]
    member this.``a good guess is made (uppercase)`` () =
        let model,_ = App.init ()
        let model2,_ = App.update (App.SelectPlanet 4) model
        let model3,_ = App.update (App.CardPageMsg (CardPage.GuessMade "Jupiter")) model2
        Assert.IsTrue(model3.GuessedNames.Length = 1)
        Assert.IsFalse(model3.CardPageModel.IsSome)

    [<TestMethod>]
    member this.``2 good guesses are made (uppercase)`` () =
        let model,_ = App.init ()
        let model2,_ = App.update (App.SelectPlanet 4) model
        let model3,_ = App.update (App.CardPageMsg (CardPage.GuessMade "Jupiter")) model2
        let model4,_ = App.update (App.SelectPlanet 5) model3
        let model5,_ = App.update (App.CardPageMsg (CardPage.GuessMade "Saturn")) model4
        Assert.IsTrue(model5.GuessedNames.Length = 2)
        Assert.IsFalse(model5.CardPageModel.IsSome)

    [<TestMethod>]
    member this.``1 good and 1 bad guess is made (uppercase)`` () =
        let model,_ = App.init ()
        let model2,_ = App.update (App.SelectPlanet 4) model
        let model3,_ = App.update (App.CardPageMsg (CardPage.GuessMade "Jupiter")) model2
        let model4,_ = App.update (App.SelectPlanet 5) model3
        let model5,_ = App.update (App.CardPageMsg (CardPage.GuessMade "quack")) model4
        Assert.IsTrue(model5.GuessedNames.Length = 1)
        Assert.IsTrue(model5.CardPageModel.IsSome)