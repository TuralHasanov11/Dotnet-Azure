---
applyTo: "**/*.ts,**/*.vue,**/*.js,**/*.jsx"
---

# Project coding standards for TypeScript and Vue

Apply the [general coding guidelines](../copilot-instructions.md) and [TypeScript guidelines](./ts-coding.instructions.md) to all code.

## General Guidelines

- Use functional components with composables
- Follow the Vue.js Composition API rules
- Use TypeScript for all new code
- Keep components small and focused
- Use CSS modules for component styling
- AvoidDirectDOMManipulation: Use Vue.js virtual DOM instead of direct DOM manipulation to maintain security.
- UseVHTMLSparingly: If using v-html, ensure content is sanitized with DOMPurify to prevent XSS attacks.
- EscapeDynamicContent: Ensure dynamic content passed through props is properly escaped to prevent injection attacks and only render Props in SAFE HTML attributes.
- ValidateAllProps : Use TypeScript or PropTypes to validate all props. Only render Props in SAFE HTML attributes.
- LimitInlineStyles: Avoid using inline styles or ensure untrusted CSS values are escaped with CSS.escape() to prevent injection.
- UseStaticVueTemplates: Ensure that Vue.js templates are static and not dynamically created to avoid template injection.

## Error Handling

- Implement proper error boundaries in React components
