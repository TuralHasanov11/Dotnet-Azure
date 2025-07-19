---
applyTo: "**/*.ts,**/*.tsx,**/*.js,**/*.jsx"
---

# Project coding standards for React

Apply the [general coding guidelines](../copilot-instructions.md) and [TypeScript guidelines](./ts-coding.instructions.md) to all code.

## General Guidelines

- Use functional components with hooks
- Follow the React hooks rules (no conditional hooks)
- Use React.FC type for components with children
- Keep components small and focused
- Use CSS modules for component styling
- AvoidDirectDOMManipulation: Use React virtual DOM instead of direct DOM manipulation to maintain security.
- EscapeDynamicContent: Ensure dynamic content passed through props is properly escaped to prevent injection attacks and only render Props in SAFE HTML attributes.
- ValidateAllProps : Use TypeScript or PropTypes to validate all props. Only render Props in SAFE HTML attributes.
- LimitInlineStyles: Avoid using inline styles or ensure untrusted CSS values are escaped with CSS.escape() to prevent injection.

## Error Handling

- Implement proper error boundaries in React components
