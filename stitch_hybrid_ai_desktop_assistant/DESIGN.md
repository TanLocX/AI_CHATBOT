---
name: Lumina Desktop
colors:
  surface: '#131313'
  surface-dim: '#131313'
  surface-bright: '#393939'
  surface-container-lowest: '#0e0e0e'
  surface-container-low: '#1b1b1c'
  surface-container: '#202020'
  surface-container-high: '#2a2a2a'
  surface-container-highest: '#353535'
  on-surface: '#e5e2e1'
  on-surface-variant: '#c0c7d4'
  inverse-surface: '#e5e2e1'
  inverse-on-surface: '#303030'
  outline: '#8a919e'
  outline-variant: '#404752'
  surface-tint: '#a3c9ff'
  primary: '#a3c9ff'
  on-primary: '#00315c'
  primary-container: '#0078d4'
  on-primary-container: '#ffffff'
  inverse-primary: '#0060ab'
  secondary: '#fff9ef'
  on-secondary: '#3a3000'
  secondary-container: '#ffdb3c'
  on-secondary-container: '#725f00'
  tertiary: '#79dd68'
  on-tertiary: '#003a01'
  tertiary-container: '#22881d'
  on-tertiary-container: '#ffffff'
  error: '#ffb4ab'
  on-error: '#690005'
  error-container: '#93000a'
  on-error-container: '#ffdad6'
  primary-fixed: '#d3e3ff'
  primary-fixed-dim: '#a3c9ff'
  on-primary-fixed: '#001c39'
  on-primary-fixed-variant: '#004883'
  secondary-fixed: '#ffe16d'
  secondary-fixed-dim: '#e9c400'
  on-secondary-fixed: '#221b00'
  on-secondary-fixed-variant: '#544600'
  tertiary-fixed: '#94fa81'
  tertiary-fixed-dim: '#79dd68'
  on-tertiary-fixed: '#002200'
  on-tertiary-fixed-variant: '#005303'
  background: '#131313'
  on-background: '#e5e2e1'
  surface-variant: '#353535'
typography:
  display-lg:
    fontFamily: Inter
    fontSize: 32px
    fontWeight: '700'
    lineHeight: 40px
    letterSpacing: -0.02em
  headline-md:
    fontFamily: Inter
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 28px
  body-md:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  body-sm:
    fontFamily: Inter
    fontSize: 13px
    fontWeight: '400'
    lineHeight: 18px
  code-block:
    fontFamily: JetBrains Mono
    fontSize: 13px
    fontWeight: '400'
    lineHeight: 22px
  label-caps:
    fontFamily: Inter
    fontSize: 11px
    fontWeight: '700'
    lineHeight: 16px
    letterSpacing: 0.05em
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  container-margin: 24px
  gutter: 12px
  chat-gap: 16px
  sidebar-width: 280px
  input-padding: 12px 16px
---

## Brand & Style
The design system focuses on a high-productivity AI environment for Windows Power Users. It prioritizes a sense of precision, technical reliability, and seamless OS integration. 

The aesthetic is **Modern Corporate with Glassmorphic accents**, heavily influenced by Windows 11 Fluent Design. It utilizes semi-transparent materials (Mica and Acrylic) to provide depth without sacrificing performance. The UI should feel like a native extension of the operating system—sophisticated, calm, and highly organized.

## Colors
The palette is centered around a deep charcoal foundation to minimize eye strain during long coding or writing sessions. 

- **Primary Action:** Vibrant Accent Blue (#0078D4) is used for system-level actions and "Offline" (Ollama) indicators.
- **Model Indicators:** A soft Gold gradient is reserved for "Online" (Gemini) states to signify premium cloud connectivity.
- **Surface Logic:** Backgrounds use `#0F0F0F`. Interactive surfaces use `#2D2D2D`. 
- **Transparency:** The sidebar utilizes an Acrylic-style blur (background-blur: 30px) over the `glass_sidebar` color.

## Typography
Since Segoe UI is the standard, we utilize **Inter** as the primary web-based equivalent for its superior rendering at small sizes in Electron/Desktop environments. 

- **Hierarchy:** Use `display-lg` for welcome screens and `headline-md` for sidebar category headers. 
- **Readability:** All chat messages use `body-md` with a slightly generous line height for markdown legibility. 
- **Monospace:** `jetbrainsMono` is strictly used for code blocks and technical metadata (token counts, model versions) to provide a "developer-tool" feel.

## Layout & Spacing
The layout follows a **Fixed Sidebar + Fluid Content** model. 

- **Sidebar:** Positioned to the left, using a 280px fixed width. It houses the model switcher and history.
- **Chat View:** A centered maximum width of 800px for the message thread to maintain readability.
- **Rhythm:** An 8px base grid is used. Spacing between user and AI bubbles is 16px (`chat-gap`), while groups of messages from the same participant use 4px.
- **Padding:** Use `container-margin` for the main window edges to give the UI room to breathe.

## Elevation & Depth
This design system avoids traditional heavy shadows, opting instead for **Tonal Elevation and Inner Glows**.

- **Level 0 (Background):** `#0F0F0F` - The deepest layer.
- **Level 1 (Sidebar):** Acrylic blur over `#1E1E1E` with a 1px right-side border of `rgba(255,255,255,0.08)`.
- **Level 2 (Chat Bubbles):** Surface Slate (`#2D2D2D`). User bubbles feature a subtle top-down 1px white highlight (opacity 0.05) to simulate a light source from above.
- **Level 3 (Popovers/Tooltips):** Solid `#3D3D3D` with a 4px drop shadow (0% offset, 8px blur, black 40% opacity).

## Shapes
In alignment with Windows 11 Fluent guidelines:
- **Standard Elements:** Buttons, input fields, and chat bubbles use `rounded` (8px).
- **Large Containers:** Sidebar panels and file preview cards use `rounded-lg` (16px).
- **Interactive States:** On hover, list items in the sidebar should reveal a `rounded-sm` (4px) background highlight.

## Components

### Chat Bubbles
- **User:** Right-aligned. Background: `surface_slate`. Border: Subtle outline.
- **AI:** Left-aligned. Background: Transparent. Accent: A thin vertical line on the far left using the current model's color (Blue for Ollama, Gold for Gemini).

### Model Toggle Switch
- A segmented control (pill-shaped). The active state should use a "high-plateau" effect—a slightly lighter grey with a subtle 1px border.
- **Online Badge:** Gold text on a 10% opacity gold background.
- **Offline Badge:** Blue text on a 10% opacity blue background.

### Sidebar Nav Items
- Subtle height (36px). Hover state uses `rgba(255, 255, 255, 0.04)`.
- Active state uses a 3px vertical "pill" indicator on the far left edge of the item.

### Input Field
- Fixed to the bottom of the chat view. 
- Uses a "Mica" style semi-transparent background. 
- Features an icon row for file attachments and model selection immediately above the text area.

### File Attachment Preview Cards
- Horizontal cards (approx 200px wide).
- Icon on the left, filename and size on the right.
- Border: `border_subtle`. 
- Background: `background_deep`.

### Markdown Containers
- **Code Blocks:** `#000000` background with a copy button in the top-right corner.
- **Inline Code:** `rgba(255,255,255,0.1)` background with 4px corner radius.