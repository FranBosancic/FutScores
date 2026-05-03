---
name: aspnet-routing
description: Use this skill when adding or changing routing in an ASP.NET Core MVC app, defining custom URLs with MapControllerRoute or Route attributes, documenting controller/action/view mappings, or creating sitemap.md for lab or project delivery. Keywords: routing, route, custom route, attribute routing, MapControllerRoute, Route attribute, sitemap, URL mapping, ASP.NET Core MVC, controller action.
argument-hint: Describe the routing goal, affected controllers/actions, and whether you need route implementation, analysis, or sitemap documentation.
---

# ASP.NET Core MVC Routing

Use this skill for routing-specific work in an ASP.NET Core MVC project: custom URLs, `MapControllerRoute(...)`, `[Route(...)]`, route analysis, and `sitemap.md` documentation.

## Use This Skill When

- the user wants a custom URL instead of the default `/Controller/Action/{id?}` pattern
- the user wants to add or refactor `MapControllerRoute(...)` definitions in `Program.cs`
- the user wants to use `[Route(...)]` on a controller or action
- the user wants to document all current URLs in `sitemap.md`
- the lab requires at least 4 custom-routed controller actions
- the user asks how a URL resolves to controller, action, and view

## Do Not Use This Skill When

- the task is mainly EF Core, migrations, or database work
- the task is mainly frontend Razor layout or CSS work
- the task is only about controller business logic with no URL change
- the user only wants a general explanation of MVC with no routing change or analysis

## Routing Workflow

### 1. Inspect Current Routing Surface

First inspect:
- `Program.cs`
- the target controller
- the target action signatures
- the relevant view paths under `Views/<Controller>/`

Determine:
- whether the app currently relies only on the default conventional route
- whether custom routes already exist
- whether the target action already has route attributes
- whether route parameters match the action parameters

### 2. Define the Routing Intent

Capture:
- target URL pattern
- which controller and action should handle it
- whether the route is public-facing, semantic, localized, or admin-oriented
- whether route parameters are optional, required, or constrained
- whether the route should coexist with the default route or replace it for that action

If details are vague, ask focused questions rather than guessing.

### 3. Choose the Routing Style

Use conventional routing in `Program.cs` when:
- several routes follow a shared pattern
- the route is app-level and easier to understand centrally
- the user wants a route table style setup

Use attribute routing when:
- the route is tightly coupled to one controller or action
- the URL should be obvious next to the action code
- different actions in the same controller need distinct semantic URLs

### 4. Design the Route Carefully

Before editing, check:
- parameter names match the action signature
- optional parameters are truly optional in the action
- constraints reflect real requirements
- route order will not shadow or break more specific routes
- controller and view naming still follow MVC conventions

When relevant, prefer semantic URLs such as:
- `/lige/premier-league`
- `/utakmice/123`
- `/igraci/8/ocjene`

### 5. Implement the Smallest Safe Change

- add the custom route with the narrowest needed scope
- preserve the default route unless the task explicitly removes it
- avoid broad rewrites of unrelated controllers
- keep action names, parameters, and views aligned
- update links or `asp-action`/`asp-controller` usage if the route strategy requires it

### 6. Validate the Route

- the project builds
- the intended URL resolves to the expected action
- parameters bind correctly
- no previously working route was broken unintentionally

### 7. Document the Routing Model

For each reachable URL include:
- URL pattern
- controller
- action
- route style: conventional or attribute
- expected view file
- key parameters and constraints if any

Keep the document concise and factual.

## Lab-Focused Completion Checks

For this lab, treat the routing task as complete only if these checks pass:
- at least 4 custom-routed controller actions exist
- those 4 are not counted only through the default route
- the route definitions and action signatures are consistent
- `sitemap.md` reflects the implemented URLs
- the app still has a valid fallback/default navigation path unless the user asked otherwise

## Example Prompts

- Add 4 custom routes to this ASP.NET Core MVC app for the lab
- Help me choose between attribute routing and MapControllerRoute
- Create sitemap.md for all current URLs in this project
- Change these details pages to semantic URLs
- Explain which controller and action handle each route in this MVC app
