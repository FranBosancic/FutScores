---
name: ui-ux-discovery
description: Use this skill when defining UI and UX direction for an ASP.NET Core MVC app, planning a design system, interviewing the user about layout and visual preferences, creating a UX brief, identifying reusable components, or deciding how list, details, and form pages should look and behave. Keywords: UI, UX, design system, layout, dashboard, admin panel, list page, details page, form, cards, tables, navigation, responsive, Razor views, MVC.
---

<!-- Tip: Use /create-skill in chat to generate content with agent assistance -->

# UI/UX Discovery

Use this skill before generating UI code when the project does not yet have a clear visual or interaction direction.

This skill helps the agent:
- interview the user about desired UI and UX direction
- inspect the existing MVC structure when needed
- identify reusable page patterns
- produce a concise UX brief
- prepare future UI skills for list, details, and form pages

This skill is for discovery and planning first. It should not jump straight into implementation unless the user explicitly asks for code.

## Use This Skill When

Use this skill when:
- the app does not yet have a clear UI direction
- the user says they want to "play with UI/UX ideas" before coding
- the user wants a design system or reusable page patterns
- the user wants help choosing between dashboard, admin, sports, editorial, or minimal styles
- the user wants to standardize list pages, details pages, or forms
- the user wants the agent to ask questions first instead of inventing a design

## Do Not Use This Skill When

Do not use this skill when:
- the user already gave a complete visual brief and wants direct implementation
- the task is purely backend, EF, routing, or database related
- the user only wants a one-off CSS tweak with no design discussion
- the user asks for debugging unrelated UI code behavior

## Primary Goal

Your job is to turn vague UI wishes into a practical design brief that fits the existing ASP.NET Core MVC application.

You must:
- ask a short, focused discovery interview
- separate confirmed preferences from your own recommendations
- keep recommendations grounded in the current project structure
- identify reusable components and page archetypes
- leave implementation for a later step unless explicitly requested

## Discovery Workflow

Follow this order.

### 1. Understand Product Intent

First determine what kind of application the user wants.

Ask about:
- who the main user is
- what the most important tasks are
- whether the app should feel like an admin panel, sports dashboard, data browser, or showcase experience
- whether speed of use or visual presentation matters more

### 2. Clarify Visual Direction

Ask about:
- dark, light, or mixed theme
- compact or spacious layout
- tables or cards for list pages
- minimal or expressive design
- subtle or strong domain branding
- reference apps, screenshots, or styles they like

### 3. Clarify Interaction Patterns

Ask about:
- mobile priority versus desktop priority
- whether details pages should be compact or richer and more visual
- whether filters, search, badges, breadcrumbs, and stat cards are desirable
- whether forms should be standard CRUD forms or more guided experiences

### 4. Inspect Existing Structure If Helpful

If the workspace already contains views, layout files, or CSS, inspect them and align your recommendations with reality.

Prefer checking:
- shared layout
- index pages
- details pages
- existing nav structure
- styling conventions
- current use of Bootstrap, Tailwind, CSS files, or partials

### 5. Synthesize a UX Brief

After the interview, produce a structured brief with these sections:
- Product Direction
- Users and Main Tasks
- Visual Direction
- Page Archetypes
- Reusable Components
- Interaction Principles
- Constraints
- Open Questions
- Recommended Next Step

## Output Requirements

Your output should be concise and structured.

Always distinguish:
- Confirmed Preferences
- Inferred Recommendations
- Open Questions

Avoid long generic design lectures.

Prefer actionable conclusions like:
- use card-based details pages with stat blocks
- keep list pages table-first on desktop
- use consistent action bars across index and details pages
- standardize badges for domain labels like league or player position

## Design Heuristics

When making recommendations:
- prefer consistency over novelty
- respect the constraints of Razor views and MVC page composition
- recommend reusable partials when the same UI block will appear on multiple pages
- avoid proposing SPA-like interaction unless the user clearly wants it
- do not invent animations, themes, or layouts without user intent
- do not overload CRUD pages with visual noise

## Expected Page Archetypes

When relevant, organize findings around these page types:
- Dashboard
- List or Index Page
- Details Page
- Create or Edit Form
- Shared Layout and Navigation

For each archetype, suggest:
- its job
- its information hierarchy
- the most suitable layout pattern
- which components should be reusable

## Reusable Component Inventory

When useful, identify a reusable component set such as:
- navbar
- page header
- action bar
- stat card
- data table
- entity card
- badge
- filter bar
- breadcrumb
- detail info grid
- form field group
- empty state
- error or success message

## Good Behavior

Good behavior looks like this:
- ask 6 to 8 focused questions before designing
- keep the questions practical and short
- summarize what the user wants before proposing structure
- adapt recommendations to the existing project
- make page-pattern recommendations that can be reused later in UI skills

## Bad Behavior

Bad behavior looks like this:
- immediately generating a full design without questions
- describing a generic trendy dashboard with no connection to the app
- mixing backend architecture advice into a UX discovery task
- over-prescribing exact colors and spacing before understanding the user
- pretending unclear preferences are already decided

## Example Prompts

Use this skill for prompts like:
- Help me define the UI direction for this MVC app before we build pages
- Ask me questions and create a UX brief for this football app
- I want to decide how list and details pages should look
- Help me design a reusable UI system for Razor views
- Figure out whether this app should feel more like an admin panel or a sports dashboard
- Interview me about UX preferences before generating frontend code

## Example Response Shape

A good result should look roughly like this:

- Product Direction:
	Football-focused data dashboard for browsing clubs, players, matches, and ratings.

- Confirmed Preferences:
	Dark theme, desktop-first, tables for lists, card-based details pages, restrained football branding.

- Recommended Page Archetypes:
	Dashboard with summary cards, table-driven index pages, details pages with key info sections and related links.

- Reusable Components:
	Shared navbar, page header, stat card, table shell, badges, breadcrumb, details info grid.

- Open Questions:
	Whether forms should stay simple admin CRUD or become more guided.

## Next-Step Guidance

At the end, recommend one next step only.

Examples:
- create a list-page skill from the approved brief
- create a details-page skill from the approved brief
- review existing views against the new UX brief
- define a shared component vocabulary before implementation