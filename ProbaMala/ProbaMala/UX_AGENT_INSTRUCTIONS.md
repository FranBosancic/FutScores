# UX Agent Instructions – FutScores Football Platform

## Mission
Transform the FutScores web application into a polished, football-themed platform with a **dark modern aesthetic**. The UX sub-agent is responsible for all visual design, layout decisions, component styling, and ensuring a consistent, professional user experience across all pages.

---

## Design Philosophy

### Core Principles
- **Dark Modern**: Use dark backgrounds (charcoal, near-black) with bright, readable text (near-white, soft grays)
- **Professional & Minimal**: Avoid clutter. Football context should enhance, not overwhelm
- **Football-Inspired but Subtle**: Colors and imagery reference football without being cartoonish
- **Accessible & Readable**: High contrast ratios, clear typography, generous spacing
- **Consistent & Cohesive**: Same patterns repeat across all pages (players, clubs, matches, ratings)

### Target Aesthetic
Think modern sports dashboard (like Premier League website or ESPN's dark mode), not stadium-themed chaos.

---

## Color Palette

### Primary Colors
- **Background (Primary)**: `#0f172a` (dark slate, nearly black)
- **Background (Secondary)**: `#1e293b` (slightly lighter slate for cards/containers)
- **Accent Color**: `#10b981` (emerald green – represents football pitch)
- **Accent Hover**: `#059669` (darker emerald for interactivity)

### Text Colors
- **Primary Text**: `#f1f5f9` (soft white, very light slate)
- **Secondary Text**: `#cbd5e1` (muted gray for metadata/timestamps)
- **Muted Text**: `#94a3b8` (dimmer gray for labels)

### Supporting Colors
- **Success**: `#10b981` (emerald – matches accent)
- **Warning**: `#f59e0b` (amber – for scores/highlights)
- **Danger**: `#ef4444` (red – for red cards/errors)
- **Info**: `#3b82f6` (blue – for neutral information)

### Highlight/Focus
- **Focus State**: `#22d3ee` (cyan – for interactive elements)
- **Border Color**: `#334155` (dark gray for subtle dividers)

---

## Typography

### Font Stack
```css
font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Helvetica Neue', sans-serif;
```
(Tailwind default is fine)

### Hierarchy
- **Page Titles (h1)**: 
  - Size: `2.25rem` (36px) – Tailwind `text-4xl`
  - Weight: `700` (bold)
  - Color: `#f1f5f9`
  - Margin: `1.5rem bottom`

- **Section Titles (h2)**:
  - Size: `1.875rem` (30px) – Tailwind `text-3xl`
  - Weight: `600` (semibold)
  - Color: `#f1f5f9`
  - Margin: `1rem bottom`

- **Subsection Titles (h3)**:
  - Size: `1.5rem` (24px) – Tailwind `text-2xl`
  - Weight: `600`
  - Color: `#cbd5e1`

- **Body Text**:
  - Size: `1rem` (16px)
  - Weight: `400` (regular)
  - Color: `#f1f5f9`
  - Line Height: `1.6`

- **Small/Metadata**:
  - Size: `0.875rem` (14px) – Tailwind `text-sm`
  - Color: `#cbd5e1`

---

## Layout & Spacing

### Grid System
- Use Tailwind's **12-column grid** for consistency
- **Container max-width**: `max-w-7xl` (80rem)
- **Padding**: 
  - Page/container: `2rem` (32px) padding on all sides
  - Card internal: `1.5rem` (24px)
  - Between sections: `2-3rem` (32-48px gap)

### Responsive Breakpoints (use Tailwind defaults)
- Mobile: base (no prefix)
- Tablet: `md:` (768px)
- Desktop: `lg:` (1024px)
- Large: `xl:` (1280px)

### Spacing Unit
All spacing uses multiples of `0.25rem` (4px):
- `p-2` = 8px, `p-4` = 16px, `p-6` = 24px, `m-4` = 16px, etc.

---

## Components

### Navigation Bar (_Layout.cshtml)
- **Background**: `bg-slate-900` (`#0f172a`)
- **Height**: `h-16` (64px)
- **Border**: Subtle bottom border `border-b border-slate-700` 
- **Logo/Brand**: Left-aligned, `text-2xl font-bold text-emerald-400`
- **Nav Links**: 
  - Spacing: `mx-2` or `mx-3` between links
  - Color: `text-slate-200` (default)
  - Hover: `hover:text-emerald-400 cursor-pointer transition-colors`
  - Active: `text-emerald-400 border-b-2 border-emerald-400`
- **Dropdown/Mobile menu** (if responsive): Dark background, same color scheme

### Cards (for lists/index pages)
```css
.card {
  background: #1e293b;         /* bg-slate-800 */
  border: 1px solid #334155;   /* border-slate-700 */
  border-radius: 0.5rem;       /* rounded-lg */
  padding: 1.5rem;             /* p-6 */
  transition: all 0.3s ease;   /* smooth hover effect */
}

.card:hover {
  background: #0f172a;         /* slightly darker on hover */
  border-color: #10b981;       /* emerald accent */
  box-shadow: 0 10px 25px rgba(16, 185, 129, 0.1); /* subtle emerald glow */
}
```

### Buttons
- **Default Button**:
  - Background: `bg-emerald-600`
  - Text: `text-white`
  - Padding: `px-4 py-2` (16px × 8px)
  - Rounded: `rounded-md`
  - Hover: `hover:bg-emerald-700 transition-colors`
  - Focus: `ring-2 ring-emerald-500 ring-offset-2 ring-offset-slate-900`

- **Secondary Button**:
  - Background: `bg-slate-700`
  - Text: `text-slate-100`
  - Border: `border border-slate-600`
  - Hover: `hover:bg-slate-600`

- **Danger Button**:
  - Background: `bg-red-600`
  - Hover: `hover:bg-red-700`

### Tables (for index views)
```css
.table {
  width: 100%;
  border-collapse: collapse;
}

.table thead {
  background: #1e293b;    /* bg-slate-800 */
  border-bottom: 2px solid #334155; /* border-slate-700 */
}

.table th {
  padding: 1rem;          /* p-4 */
  text-align: left;
  color: #cbd5e1;         /* text-slate-300 */
  font-weight: 600;       /* font-semibold */
  text-transform: uppercase;
  font-size: 0.875rem;    /* text-sm */
}

.table tbody tr {
  border-bottom: 1px solid #334155; /* border-slate-700 */
  transition: background 0.2s;
}

.table tbody tr:hover {
  background: #1e293b;    /* hover effect */
}

.table td {
  padding: 1rem;
  color: #f1f5f9;         /* text-slate-50 */
}
```

### Badges (for player positions, match status)
```css
.badge {
  display: inline-block;
  padding: 0.25rem 0.75rem; /* px-3 py-1 */
  background: #3b82f6;   /* bg-blue-500 (default) */
  color: white;
  border-radius: 9999px; /* rounded-full */
  font-size: 0.875rem;   /* text-sm */
  font-weight: 500;      /* font-medium */
}

.badge.position-gk { background: #6366f1; }      /* Goalkeeper - indigo */
.badge.position-def { background: #10b981; }     /* Defender - emerald */
.badge.position-mid { background: #f59e0b; }     /* Midfielder - amber */
.badge.position-fwd { background: #ef4444; }     /* Forward - red */
```

### Forms & Inputs
- **Input Field**:
  - Background: `bg-slate-800`
  - Border: `border border-slate-700`
  - Text: `text-slate-50`
  - Padding: `px-3 py-2`
  - Rounded: `rounded-md`
  - Focus: `focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500`

### Alert/Message Boxes
- **Success**: `bg-emerald-900 text-emerald-100 border border-emerald-700`
- **Error**: `bg-red-900 text-red-100 border border-red-700`
- **Warning**: `bg-amber-900 text-amber-100 border border-amber-700`
- **Info**: `bg-blue-900 text-blue-100 border border-blue-700`

---

## Page-Specific Guidelines

### Dashboard / Home Page
- **Hero Section**: Large title with subtitle, background gradient (dark slate to slightly lighter)
- **Stats Cards**: 3-4 key stats (total matches, top player, recent score) in grid layout
- **Recent Activity**: Last 5-10 matches in a timeline or compact table
- **League Overview**: Small table or cards showing league standings
- **Call-to-Action**: Prominent link to browse players/matches

### Index Pages (Players, Clubs, Matches, Ratings, Users)
- **Page Header**: Title + count of items
- **Filter/Search Bar** (if added later): Above the list
- **Cards or Table**: Display all items, clickable rows/cards link to Details
- **Pagination** (optional): If more than 10 items

### Details Pages
- **Header Section**: Large title with badge/status
- **Key Information**: 2-3 columns of data with clear labels
- **Related Data**: Links to related entities (e.g., matches for a player, players for a club)
- **Breadcrumb Navigation**: `Home > Entities > Entity Name`
- **Action Links**: Link back to list, link to related entities

---

## Interaction & Transitions

- **Hover States**: Subtle color shift or border color change (300ms transition)
- **Focus States**: Ring outline (2px) in accent color
- **Active States**: Bolder text or underline for navigation
- **Loading States** (if applicable): Spinner or skeleton loaders with dark background
- **Smooth Transitions**: Use `transition-all duration-300` for most interactions

---

## Dark Mode Do's & Don'ts

### ✅ DO
- Use sufficient contrast (WCAG AA minimum 4.5:1 for normal text)
- Include subtle borders/dividers to separate sections
- Use color accents sparingly (emerald for primary actions)
- Make clickable elements obvious (hover/focus states)
- Keep backgrounds consistently dark (don't mix too many shades)

### ❌ DON'T
- Use pure black (`#000000`) – too harsh; use `#0f172a` or `#1e293b`
- Use pure white (`#ffffff`) – too bright; use `#f1f5f9`
- Overuse ornaments or images; keep it clean
- Hide interactive elements (buttons, links must be obvious)
- Use very small fonts; minimum `text-base` for body content
- Mix too many accent colors (emerald is primary; others are supporting)

---

## Implementation Strategy for Sub-Agent

When invoking this UX sub-agent, provide:

1. **Specific Page/Component**: "Update the player index view to use cards instead of a table"
2. **Context**: Include relevant controllers/view file paths
3. **Current State**: Share the existing CSS or HTML if modifications
4. **Expected Output**: Clarify if you want Tailwind utility classes or custom CSS
5. **Constraints**: Any restrictions on libraries or file locations

### Example Prompt for UX Sub-Agent:
```
Update Views/Shared/_Layout.cshtml:
- Replace Bootstrap navbar with a dark-themed Tailwind navbar
- Use the color palette: dark bg (#0f172a), text (#f1f5f9), accent emerald (#10b981)
- Add smooth hover transitions on nav links
- Keep responsive (mobile menu if needed)
Follow the UX_AGENT_INSTRUCTIONS.md file for exact color codes and component patterns.
```

---

## Tailwind Setup (CDN Method – Simplest)

Add to `_Layout.cshtml` `<head>`:
```html
<script src="https://cdn.tailwindcss.com"></script>
```

For custom colors, add a `<script>` block in `<head>` before Tailwind script (or use `tailwind.config.js` if building):
```html
<script>
  tailwind.config = {
    theme: {
      extend: {
        colors: {
          'slate': { 50: '#f8fafc', 100: '#f1f5f9', /* ... existing tailwind colors ... */, 900: '#0f172a' }
        }
      }
    }
  }
</script>
```

---

## Git & Documentation

This instruction file should be:
1. **Committed to Git** as `UX_AGENT_INSTRUCTIONS.md`
2. **Referenced in chat**: Link or quote relevant sections when asking for UI updates
3. **Iterated on**: Update if design needs change or new patterns emerge
4. **Logged**: Save conversation logs showing sub-agent invocation for Lab 2 proof

---

## Success Criteria

The UX agent has succeeded when:
- ✅ All pages use consistent dark color scheme
- ✅ Navigation is clear and intuitive
- ✅ Cards/tables are properly styled and highlighted
- ✅ Buttons and forms are accessible (high contrast, clear focus states)
- ✅ Text is readable (no low-contrast color combinations)
- ✅ Design feels modern and professional (not default Bootstrap)
- ✅ Football context is subtle but present (emerald pitch-inspired accent)
- ✅ All entity pages (Players, Clubs, Matches, Ratings, Users) follow the same pattern
- ✅ Dashboard home page showcases key information with visual hierarchy

