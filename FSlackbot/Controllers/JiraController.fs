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

    let findLinks (jiraPath : string) (jiraProjects : string) (text : string) =
        let projs = jiraProjects.Split([| ',' |], StringSplitOptions.RemoveEmptyEntries)
                        |> Array.map (fun s -> s.Trim())
                        |> Array.filter (fun s -> Regex.IsMatch(s, "^[A-Z]{1,5}$"))
        if projs.Any() then
            // \b(ABC|DEF|EFG)\-\d+\b
            let projRegex = @"\b(" + String.Join("|", projs) + @")\-\d+\b"
            Regex.Matches(text, projRegex)
            |> Seq.cast
            |> Seq.map (fun (m : Match) -> m.Value + ": " + jiraPath + m.Value)
            |> Array.ofSeq
        else Array.empty

    member x.Post(jiraPath : string, jiraProjects : string, req : SlackRequest) =
        let noContent () = new HttpResponseMessage(Net.HttpStatusCode.NoContent)
        let linksToResponse (links : string array) = { text = String.Join("\n", links) }

        if  req.user_name.ToLower() = "webhook"
            || req.user_name.ToLower().EndsWith("bot")
            || req.text.Contains(jiraPath) //already has a link to jira
            then
            noContent()
        else
            let links = findLinks jiraPath jiraProjects req.text
            if links.Any() then
                x.Request.CreateResponse(Net.HttpStatusCode.OK, linksToResponse links)
            else
                noContent()
