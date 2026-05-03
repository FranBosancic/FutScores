---
name: semantic-model
description: Use this skill when creating, updating, or reviewing semantic-model.md for an ASP.NET Core MVC app that uses Entity Framework. It is intended for summarizing entities, main properties, foreign keys, and relationships between tables or classes. Keywords: semantic model, semantic-model.md, EF model, entity relationships, database model, entity summary, foreign keys, navigation properties, ASP.NET Core MVC, Entity Framework.
argument-hint: Describe whether you want to create, update, or audit semantic-model.md, and mention which entities or relationships changed.
---

# Semantic Model Documentation

Use this skill when the task is to describe the application's data model in a concise, lab-friendly Markdown document.

This skill is specifically for producing or updating `semantic-model.md` from the current Entity Framework model.

## Use This Skill When

- the user asks to create `semantic-model.md`
- the lab requires a semantic database model in Markdown
- entity classes were added, removed, or changed
- foreign keys or navigation properties changed
- the user wants a concise summary of tables, classes, and relationships
- the user wants to check whether the current semantic model document matches the code

## Do Not Use This Skill When

- the task is to implement EF classes, migrations, or DbContext configuration
- the task is mainly routing or `sitemap.md`
- the task is mainly UI, views, or CSS
- the user wants raw SQL schema generation rather than human-readable documentation

## Goal

Produce a clear, compact `semantic-model.md` that explains:
- which entities exist
- the main properties of each entity
- which properties are foreign keys
- how the entities relate to each other
- any important modeling notes, such as enums or special relationship handling

## Workflow

### 1. Inspect The Model Surface

Read the current EF model from:
- entity classes under `Models/Entities/`
- `DbContext` under `Data/`
- migrations if relationship intent is unclear

Confirm:
- entity names
- key properties
- required or important scalar properties
- foreign keys
- navigation properties
- enum types used by entities

### 2. Identify Documentation Scope

Decide whether the user needs:
- a brand new `semantic-model.md`
- an update to match code changes
- an audit of an existing semantic model document

Keep the document aligned to the current code, not to assumptions.

### 3. Summarize Each Entity

For each entity, include:
- a one-sentence purpose
- the primary key
- the main business properties
- foreign keys
- navigation properties when relevant

Do not list every trivial detail if it reduces clarity. Prefer the fields that explain the model structure.

### 4. Describe Relationships Explicitly

List the important relationships in simple terms, for example:
- `League` 1-N `Club`
- `Club` 1-N `Player`
- `Player` 1-N `Rating`

When a relationship is unusual, explain it explicitly.
Example:
- `Match` references `Club` twice through `HomeTeamId` and `AwayTeamId`

### 5. Note Modeling Details

Call out anything that helps the reader understand the model, such as:
- enum-backed properties
- multiple foreign keys to the same entity
- optional versus required text fields
- whether the document reflects EF classes rather than a separate handwritten database design

### 6. Keep The Output Submission-Friendly

The document should be:
- concise
- factual
- easy to scan
- suitable for lab submission

Avoid long theory sections. Focus on the actual project model.

## Completion Checks

Treat the task as complete only if:
- every current entity is represented
- the main properties are listed
- all important relationships are described
- special cases like enums or duplicated foreign-key targets are noted
- the content matches the current codebase

## Example Prompts

- Create semantic-model.md for this EF Core MVC project
- Update semantic-model.md after I changed the entity model
- Audit whether semantic-model.md matches the current entity classes
- Summarize my EF entities and relationships for lab submission