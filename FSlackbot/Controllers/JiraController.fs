namespace FSlackbot.Controllers
open System
open System.Collections.Generic
open System.Linq
open System.Net.Http
open System.Web.Http
open System.Text.RegularExpressions
open Newtonsoft.Json

[<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
type SlackRequest() = 
    member val token = "" with get, set
    member val team_id = "" with get, set
    member val channel_id = "" with get, set
    member val channel_name = "" with get, set
    member val timestamp = "" with get, set
    member val user_id = "" with get, set
    member val user_name = "" with get, set
    member val text = "" with get, set
    member val trigger_word = "" with get, set

[<CLIMutable>]
[<JsonObject(MemberSerialization=MemberSerialization.OptOut)>]
type SlackResponse = {
    text : string
}

type JiraController() =
    inherit ApiController()

    member x.Post(jiraPath : string, jiraProjects : string, req : SlackRequest) =
        let projs = jiraProjects.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
                        |> Array.map (fun s -> s.Trim())
                        |> Array.filter (fun s -> Regex.IsMatch(s, "^[A-Z]{1,5}$"))
        // \b(ABC|DEF|EFG)\-\d+\b
        let projRegex = @"\b(" + String.Join("|", projs) + @")\-\d+\b"
        let links = Regex.Matches(req.text, projRegex)
                    |> Seq.cast
                    |> Seq.map (fun (m : Match) -> m.Value + ": " + jiraPath + m.Value)
                    |> Array.ofSeq

        {text = String.Join("\n", links)}
